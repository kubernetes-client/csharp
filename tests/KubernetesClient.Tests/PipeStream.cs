using System;
using System.Buffers;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace k8s.Tests
{
    #region AsyncAutoResetEvent
    /// <summary>Implements a synchronization event that, when signaled, resets automatically after releasing a single waiting thread
    /// or task.
    /// </summary>
    sealed class AsyncAutoResetEvent : IDisposable
    {
        public AsyncAutoResetEvent() { }
        public AsyncAutoResetEvent(bool initialState) => set = initialState;

        public void Dispose() => e.Dispose(); // NOTE: waiters are not triggered when it's disposed, like the normal AutoResetEvent

        /// <summary>Sets the event, releasing a waiting thread or task.</summary>
        public void Set()
        {
            lock(e)
            {
                set = true;
                e.Set();
            }
        }

        /// <summary>Waits for the event to be set.</summary>
        public void Wait()
        {
            while(true)
            {
                lock(e)
                {
                    if(set)
                    {
                        set = false;
                        break;
                    }
                }

                e.WaitOne();
            }
        }

        /// <summary>Returns a task that waits for the event to be set.</summary>
        /// <param name="cancelToken">A <see cref="CancellationToken"/> that can be used to cancel the wait.</param>
        public Task WaitAsync(CancellationToken cancelToken)
        {
            lock(e)
            {
                if(set)
                {
                    set = false;
                    return Task.CompletedTask;
                }
            }

            if(cancelToken.IsCancellationRequested) return Task.FromCanceled(cancelToken);

            // if we need to wait, we'll register a callback on the thread pool that will attempt to complete the task
            var tcs = new TaskCompletionSource<object>(TaskCreationOptions.RunContinuationsAsynchronously);
            RegisteredWaitHandle tpreg;
            void callback(object ctx, bool timedOut)
            {
                var task = (TaskCompletionSource<object>)ctx;
                lock(e)
                {
                    if(set && task.TrySetResult(null)) // if we consumed the event...
                    {
                        set = false; // reset it
                    }
                    else if(!task.Task.IsCanceled) // otherwise, if we haven't already been canceled, reregister for the event
                    {
                        tpreg = ThreadPool.UnsafeRegisterWaitForSingleObject(e, callback, ctx, Timeout.Infinite, true);
                    }
                }
            }
            tpreg = ThreadPool.UnsafeRegisterWaitForSingleObject(e, callback, tcs, Timeout.Infinite, true);
            if(cancelToken.CanBeCanceled) // if the token can be canceled...
            {
                var ctreg = cancelToken.Register(ctx => // register a callback that unregisters our thread pool callback
                {
                    lock(e)
                    {
                        if(((TaskCompletionSource<object>)ctx).TrySetCanceled()) tpreg.Unregister(null);
                    }
                }, tcs);
                tcs.Task.ContinueWith((_, r) => ((CancellationTokenRegistration)r).Dispose(), ctreg);
            }
            return tcs.Task;
        }

        readonly AutoResetEvent e = new AutoResetEvent(false);
        bool set;
    }
    #endregion

    #region Pipe
    /// <summary>Represents a pipe that blocks on reads when empty.</summary>
    sealed class Pipe : IDisposable
    {
        /// <summary>Gets the number of bytes available to be read. If not zero, the read methods will not block.</summary>
        public int DataAvailable => buffer.DataAvailable;

        /// <summary>Removes all data from the pipe.</summary>
        public void Clear()
        {
            lock(buffer) buffer.Clear();
        }

        /// <inheritdoc/>
        public void Dispose()
        {
            if(!disposed)
            {
                disposed = true;
                lock(buffer) buffer.Dispose();
                readEvent.Set();
                readEvent.Dispose();
            }
        }

        /// <summary>Called when there will be no more writes to the pipe, this method will unblock waiting readers.</summary>
        public void Finish()
        {
            if(!finished)
            {
                finished = true;
                readEvent.Set();
            }
        }

        /// <summary>Reads some data and returns the number of bytes read. The method will block until data is available or
        /// <see cref="Finish"/> is called.
        /// </summary>
        public int Read(byte[] buffer, int offset, int count) => Read(new Span<byte>(buffer, offset, count));

        /// <summary>Reads some data and returns the number of bytes read. The method will block until data is available or
        /// <see cref="Finish"/> is called.
        /// </summary>
        public int Read(Span<byte> buffer)
        {
            if(buffer.Length == 0) return 0;
            while(!disposed)
            {
                int read;
                lock(this.buffer) read = this.buffer.Read(buffer);
                if(read != 0 || finished) return read;
                readEvent.Wait();
            }
            throw new ObjectDisposedException(GetType().FullName);
        }

        /// <summary>Reads some data and returns the number of bytes read. The task will wait until data is available,
        /// <see cref="Finish"/> is called, or the task is canceled.
        /// </summary>
        public ValueTask<int> ReadAsync(byte[] buffer, int offset, int count, CancellationToken cancelToken = default) =>
            ReadAsync(new Memory<byte>(buffer, offset, count), cancelToken);

        /// <summary>Reads some data and returns the number of bytes read. The task will wait until data is available,
        /// <see cref="Finish"/> is called, or the task is canceled.
        /// </summary>
        public async ValueTask<int> ReadAsync(Memory<byte> buffer, CancellationToken cancelToken = default)
        {
            if(buffer.Length == 0) return 0;
            while(!disposed)
            {
                cancelToken.ThrowIfCancellationRequested();
                int read;
                lock(this.buffer) read = this.buffer.Read(buffer.Span);
                if(read != 0 || finished) return read;
                await readEvent.WaitAsync(cancelToken).ConfigureAwait(false);
            }
            throw new ObjectDisposedException(GetType().FullName);
        }

        /// <summary>Writes data into the pipe. This method will not block.</summary>
        public void Write(byte[] data, int offset, int count) => Write(new ReadOnlySpan<byte>(data, offset, count));

        /// <summary>Writes data into the pipe. This method will not block.</summary>
        public void Write(ReadOnlySpan<byte> data)
        {
            if(disposed) throw new ObjectDisposedException(GetType().FullName);
            if(finished) throw new InvalidOperationException("The pipe is draining.");
            if(data.Length != 0)
            {
                lock(buffer) buffer.Write(data);
                readEvent.Set();
            }
        }

        readonly StreamBuffer buffer = new StreamBuffer();
        readonly AsyncAutoResetEvent readEvent = new AsyncAutoResetEvent();
        bool disposed, finished;
    }
    #endregion

    #region StreamBuffer
    /// <summary>Implements a buffer into which data can be written and from which data can be read.</summary>
    sealed class StreamBuffer : IDisposable
    {
        /// <summary>Gets the number of bytes available in the buffer.</summary>
        public int DataAvailable { get; private set; }

        /// <summary>Removes all data from the buffer.</summary>
        public void Clear()
        {
            foreach(Chunk chunk in dataList) chunk.Dispose();
            dataList.Clear();
            DataAvailable = 0;
        }

        /// <inheritdoc/>
        public void Dispose() => Clear();

        /// <summary>Reads data from the buffer and returns the number of bytes read. This method will not block.</summary>
        public int Read(byte[] buffer, int offset, int count) => Read(new Span<byte>(buffer, offset, count));

        /// <summary>Reads data from the buffer and returns the number of bytes read. This method will not block.</summary>
        public int Read(Span<byte> buffer)
        {
            int read = 0;
            while(buffer.Length != 0) // while there's space left in the buffer...
            {
                Chunk chunk = dataList.First?.Value; // get the first data chunk in the list
                if(chunk == null || chunk.AvailableData == 0) break; // if it's nonexistent or empty, we're done
                int toCopy = Math.Min(chunk.AvailableData, buffer.Length); // otherwise, see how much we should get out of it
                chunk.Read(toCopy, buffer); // read the data into the buffer
                if(chunk.AvailableData == 0 && chunk.AvailableSpace == 0) // if the chunk became empty...
                {
                    if(chunk.Buffer.Length > DropThreshold || dataList.Count > 1) // if the chunk is extra large or we have others...
                    {
                        dataList.RemoveFirst(); // remove this one
                        chunk.Dispose();
                    }
                    else // otherwise, reset it. (we'll keep one normal-sized chunk around so we don't have to create a new one)
                    {
                        chunk.Reset();
                    }
                }
                read += toCopy;
                buffer = buffer.Slice(toCopy);
            }
            DataAvailable -= read;
            return read;
        }

        /// <summary>Writes data into the buffer. This method will not block.</summary>
        public void Write(byte[] data, int offset, int count) => Write(new ReadOnlySpan<byte>(data, offset, count));

        /// <summary>Writes data into the buffer. This method will not block.</summary>
        public void Write(ReadOnlySpan<byte> data)
        {
            int length = data.Length;
            if(length != 0)
            {
                if(dataList.Count == 0) dataList.AddLast(new Chunk(length)); // if the chunk list is empty, add a chunk
                do // while there's still data left to write...
                {
                    Chunk chunk = dataList.Last.Value; // get the last chunk
                    if(chunk.AvailableSpace == 0) // if it's full...
                    {
                        chunk = new Chunk(data.Length); // add a new chunk
                        dataList.AddLast(chunk);
                    }
                    int toCopy = Math.Min(chunk.AvailableSpace, data.Length); // now copy as much as we can into the chunk
                    chunk.Write(data.Slice(0, toCopy));
                    data = data.Slice(toCopy);
                } while(data.Length != 0);
                DataAvailable += length;
            }
        }

        const int MinChunkSize = 32*1024, DropThreshold = 64*1024;

        #region Chunk
        /// <summary>Represents a chunk of data. We maintain multiple chunks of data rather than a single large buffer to avoid having
        /// to reallocate arrays or copy data.
        /// </summary>
        sealed class Chunk
        {
            public Chunk(int minSize) => Buffer = ArrayPool<byte>.Shared.Rent(Math.Max(MinChunkSize, minSize));
            public int AvailableData => WriteIndex - ReadIndex;
            public int AvailableSpace => Buffer.Length - WriteIndex;
            public int ReadIndex { get; private set; }
            public int WriteIndex { get; private set; }

            public void Dispose() => ArrayPool<byte>.Shared.Return(Buffer);

            public void Read(int byteCount, Span<byte> dest)
            {
                new Span<byte>(Buffer, ReadIndex, byteCount).CopyTo(dest);
                ReadIndex += byteCount;
            }

            public void Reset() => ReadIndex = WriteIndex = 0;

            public void Write(ReadOnlySpan<byte> span)
            {
                span.CopyTo(new Span<byte>(Buffer, WriteIndex, AvailableSpace));
                WriteIndex += span.Length;
            }

            public readonly byte[] Buffer;
        }
        #endregion

        readonly LinkedList<Chunk> dataList = new LinkedList<Chunk>();
    }
    #endregion

    #region PipeStream
    /// <summary>Represents a read-only stream backed by a <see cref="Pipe"/>.</summary>
    sealed class PipeStream : Stream
    {
        public PipeStream() : this(new Pipe(), true) { }
        public PipeStream(Pipe pipe, bool ownPipe) => (this.readPipe, this.ownPipe) = (pipe, ownPipe);

        /// <inheritdoc/>
        public override bool CanRead => true;

        /// <inheritdoc/>
        public override bool CanSeek => false;

        /// <inheritdoc/>
        public override bool CanWrite => false;

        /// <inheritdoc/>
        /// <remarks>This property is not supported and throws <see cref="NotSupportedException"/>.</remarks>
        public override long Length => throw new NotSupportedException();

        /// <inheritdoc/>
        /// <remarks>This property is not supported and throws <see cref="NotSupportedException"/>.</remarks>
        public override long Position { get => throw new NotSupportedException(); set => throw new NotSupportedException(); }

        /// <inheritdoc/>
        public override void Flush() => AssertNotDisposed(); // TODO: should we make Flush wait until data has been sent over the wire?

        /// <summary>Queues data that can later be read from the stream.</summary>
        public void QueueData(byte[] data, int count) => readPipe.Write(data, 0, count);

        /// <summary>Queues data that can later be read from the stream.</summary>
        public void QueueData(ReadOnlySpan<byte> data) => readPipe.Write(data);

        /// <inheritdoc/>
        public override int Read(byte[] buffer, int offset, int count)
        {
            AssertNotDisposed();
            return readPipe.Read(buffer, offset, count);
        }

#if NETCOREAPP2_1
        /// <inheritdoc/>
        public override int Read(Span<byte> buffer)
        {
            AssertNotDisposed();
            return readPipe.Read(buffer);
        }

        /// <inheritdoc/>
        public override ValueTask<int> ReadAsync(Memory<byte> buffer, CancellationToken cancellationToken = default)
        {
            AssertNotDisposed();
            return readPipe.ReadAsync(buffer, cancellationToken);
        }
#endif

        /// <inheritdoc/>
        public override Task<int> ReadAsync(byte[] buffer, int offset, int count, CancellationToken cancelToken)
        {
            AssertNotDisposed();
            return readPipe.ReadAsync(buffer, offset, count, cancelToken).AsTask();
        }

        /// <inheritdoc/>
        /// <remarks>This method is not supported and throws <see cref="NotSupportedException"/>.</remarks>
        public override long Seek(long offset, SeekOrigin origin) => throw new NotSupportedException();

        /// <inheritdoc/>
        /// <remarks>This method is not supported and throws <see cref="NotSupportedException"/>.</remarks>
        public override void SetLength(long value) => throw new NotSupportedException();

        /// <inheritdoc/>
        public override void Write(byte[] buffer, int offset, int count) => throw new NotSupportedException();

#if NETCOREAPP2_1
        /// <inheritdoc/>
        public override void Write(ReadOnlySpan<byte> buffer) => throw new NotSupportedException();
#endif

        /// <inheritdoc/>
        protected override void Dispose(bool manuallyDisposed)
        {
            disposed = true;
            if(ownPipe) readPipe.Dispose();
        }

        void AssertNotDisposed()
        {
            if(disposed) throw new ObjectDisposedException(GetType().Name);
        }

        readonly Pipe readPipe;
        bool disposed, ownPipe;
    }
    #endregion
}
