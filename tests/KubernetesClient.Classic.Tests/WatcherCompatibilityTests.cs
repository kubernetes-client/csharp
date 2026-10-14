using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using k8s.Models;
using Xunit;

namespace k8s.Tests
{
    public class WatcherCompatibilityTests
    {
        [Theory]
        [InlineData(false)]
        [InlineData(true)]
        public async Task CancellationStopsPendingRead(bool usePeekableReader)
        {
            using var cts = new CancellationTokenSource();
            using var stream = new PendingReadStream();
            using TextReader reader = usePeekableReader
                ? new LineSeparatedHttpContent.PeekableStreamReader(stream)
                : new StreamReader(stream);
            await using var enumerator = Watcher<V1Pod>.CreateWatchEventEnumerator(
                () => Task.FromResult(reader), cancellationToken: cts.Token).GetAsyncEnumerator();

            var moveNext = enumerator.MoveNextAsync().AsTask();
            await WithTimeout(stream.ReadStarted.Task).ConfigureAwait(true);
            cts.Cancel();

            await Assert.ThrowsAnyAsync<OperationCanceledException>(
                () => WithTimeout(moveNext)).ConfigureAwait(true);
            Assert.True(stream.IsDisposed);
        }

        [Theory]
        [InlineData(false)]
        [InlineData(true)]
        public async Task DisposeWatcherStopsPendingRead(bool useCustomReader)
        {
            using var stream = new PendingReadStream();
            using var customReader = new PendingReader();
            using TextReader reader = useCustomReader
                ? customReader
                : new LineSeparatedHttpContent.PeekableStreamReader(stream);
            var closed = new TaskCompletionSource<bool>(TaskCreationOptions.RunContinuationsAsynchronously);
            var errors = new List<Exception>();
            using var watcher = new Watcher<V1Pod>(
                () => Task.FromResult(reader), null, errors.Add, () => closed.SetResult(true));

            await WithTimeout(useCustomReader ? customReader.ReadStarted.Task : stream.ReadStarted.Task).ConfigureAwait(true);
            watcher.Dispose();
            await WithTimeout(closed.Task).ConfigureAwait(true);

            Assert.True(useCustomReader ? customReader.IsDisposed : stream.IsDisposed);
            Assert.Empty(errors);
            Assert.False(watcher.Watching);
            customReader.Completion.TrySetResult(null);
        }

        [Fact]
        public async Task CancellationDuringReaderCreation()
        {
            using var cts = new CancellationTokenSource();
            using TextReader reader = new StringReader("");
            var created = new TaskCompletionSource<TextReader>(TaskCreationOptions.RunContinuationsAsynchronously);
            await using var enumerator = Watcher<V1Pod>.CreateWatchEventEnumerator(
                () => created.Task, cancellationToken: cts.Token).GetAsyncEnumerator();

            var moveNext = enumerator.MoveNextAsync().AsTask();
            cts.Cancel();
            try
            {
                await Assert.ThrowsAnyAsync<OperationCanceledException>(
                    () => WithTimeout(moveNext)).ConfigureAwait(true);
            }
            finally
            {
                created.SetResult(reader);
            }
        }

        [Fact]
        public async Task PeekableReaderPreservesBufferedLines()
        {
            using var stream = new MemoryStream(Encoding.UTF8.GetBytes("first\nsecond\n"));
            using var reader = new LineSeparatedHttpContent.PeekableStreamReader(stream);

            Assert.Equal("first", await reader.PeekLineAsync().ConfigureAwait(true));
            Assert.Equal("first", await reader.ReadLineAsync().ConfigureAwait(true));
            Assert.Equal("second", await reader.ReadLineAsync().ConfigureAwait(true));
            Assert.Null(await reader.ReadLineAsync().ConfigureAwait(true));
        }

        [Theory]
        [InlineData(false)]
        [InlineData(true)]
        public async Task WatchPropagatesReadFailure(bool completedBeforeRead)
        {
            using var reader = new PendingReader();
            var error = new IOException("The request was aborted.");
            if (completedBeforeRead)
            {
                reader.Completion.SetException(error);
            }

            await using var enumerator = Watcher<V1Pod>.CreateWatchEventEnumerator(
                () => Task.FromResult<TextReader>(reader)).GetAsyncEnumerator();
            var moveNext = enumerator.MoveNextAsync().AsTask();
            if (!completedBeforeRead)
            {
                reader.Completion.SetException(error);
            }

            var observed = await Assert.ThrowsAsync<IOException>(
                () => WithTimeout(moveNext)).ConfigureAwait(true);
            Assert.Same(error, observed);
        }

        [Theory]
        [InlineData(false)]
        [InlineData(true)]
        public async Task CancellationObservesLateFault(bool duringReaderCreation)
        {
            var error = new IOException("The request was aborted after cancellation.");
            var unobserved = false;
            void OnUnobservedException(object sender, UnobservedTaskExceptionEventArgs args)
            {
                if (args.Exception.Flatten().InnerExceptions.Contains(error))
                {
                    unobserved = true;
                    args.SetObserved();
                }
            }

            TaskScheduler.UnobservedTaskException += OnUnobservedException;
            try
            {
                var task = await CreateCancelledOperation(duringReaderCreation, error).ConfigureAwait(true);
                for (var attempt = 0; attempt < 10 && task.IsAlive; attempt++)
                {
                    GC.Collect();
                    GC.WaitForPendingFinalizers();
                    GC.Collect();
                    await Task.Delay(10).ConfigureAwait(true);
                }

                Assert.False(task.IsAlive, "The faulted task must be collected to check for unobserved exceptions.");
                Assert.False(unobserved, "Cancellation left an unobserved task exception.");
            }
            finally
            {
                TaskScheduler.UnobservedTaskException -= OnUnobservedException;
            }
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        private static async Task<WeakReference> CreateCancelledOperation(bool duringReaderCreation, IOException error)
        {
            using var cts = new CancellationTokenSource();
            using var reader = new PendingReader();
            var created = new TaskCompletionSource<TextReader>();
            Task pending = duringReaderCreation ? (Task)created.Task : reader.Completion.Task;
            var enumerator = Watcher<V1Pod>.CreateWatchEventEnumerator(
                () => duringReaderCreation ? created.Task : Task.FromResult<TextReader>(reader),
                cancellationToken: cts.Token).GetAsyncEnumerator();
            await using var enumeratorScope = enumerator.ConfigureAwait(false);

            var moveNext = enumerator.MoveNextAsync().AsTask();
            cts.Cancel();
            await Assert.ThrowsAnyAsync<OperationCanceledException>(
                () => WithTimeout(moveNext)).ConfigureAwait(false);
            if (duringReaderCreation)
            {
                created.SetException(error);
            }
            else
            {
                Assert.True(reader.IsDisposed);
                reader.Completion.SetException(error);
            }

            return new WeakReference(pending);
        }

        private static async Task WithTimeout(Task task)
        {
            using var cts = new CancellationTokenSource();
            var timeout = Task.Delay(TimeSpan.FromSeconds(5), cts.Token);
            Assert.Same(task, await Task.WhenAny(task, timeout).ConfigureAwait(false));
            cts.Cancel();
            await task.ConfigureAwait(false);
        }

        private sealed class PendingReader : TextReader
        {
            public TaskCompletionSource<string> Completion { get; } = new TaskCompletionSource<string>();

            public TaskCompletionSource<bool> ReadStarted { get; } =
                new TaskCompletionSource<bool>(TaskCreationOptions.RunContinuationsAsynchronously);

            public bool IsDisposed { get; private set; }

            public override Task<string> ReadLineAsync()
            {
                ReadStarted.SetResult(true);
                return Completion.Task;
            }

            protected override void Dispose(bool disposing)
            {
                IsDisposed = true;
                base.Dispose(disposing);
            }
        }

        private sealed class PendingReadStream : Stream
        {
            private readonly TaskCompletionSource<int> completion =
                new TaskCompletionSource<int>(TaskCreationOptions.RunContinuationsAsynchronously);

            public TaskCompletionSource<bool> ReadStarted { get; } =
                new TaskCompletionSource<bool>(TaskCreationOptions.RunContinuationsAsynchronously);

            public bool IsDisposed { get; private set; }

            public override bool CanRead => true;

            public override bool CanSeek => false;

            public override bool CanWrite => false;

            public override long Length => throw new NotSupportedException();

            public override long Position
            {
                get => throw new NotSupportedException();
                set => throw new NotSupportedException();
            }

            public override async Task<int> ReadAsync(byte[] buffer, int offset, int count, CancellationToken cancellationToken)
            {
                using var registration = cancellationToken.Register(() => completion.TrySetCanceled());
                ReadStarted.SetResult(true);
                return await completion.Task.ConfigureAwait(false);
            }

            public override void Flush() => throw new NotSupportedException();

            public override int Read(byte[] buffer, int offset, int count) => throw new NotSupportedException();

            public override long Seek(long offset, SeekOrigin origin) => throw new NotSupportedException();

            public override void SetLength(long value) => throw new NotSupportedException();

            public override void Write(byte[] buffer, int offset, int count) => throw new NotSupportedException();

            protected override void Dispose(bool disposing)
            {
                if (disposing)
                {
                    IsDisposed = true;
                    completion.TrySetResult(0);
                }

                base.Dispose(disposing);
            }
        }
    }
}
