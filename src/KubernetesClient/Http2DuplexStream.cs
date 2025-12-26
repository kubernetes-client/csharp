using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.WebSockets;
using System.Threading;

namespace k8s
{
    internal sealed class ProducerConsumerStream : Stream
    {
        private readonly Queue<byte[]> queue = new Queue<byte[]>();
        private readonly SemaphoreSlim dataAvailable = new SemaphoreSlim(0);
        private readonly object gate = new object();

        private byte[] currentBuffer;
        private int currentOffset;
        private bool isCompleted;
        private bool disposed;

        public override bool CanRead => !disposed;
        public override bool CanSeek => false;
        public override bool CanWrite => !disposed && !isCompleted;
        public override long Length => throw new NotSupportedException();
        public override long Position { get => throw new NotSupportedException(); set => throw new NotSupportedException(); }

        public void Complete()
        {
            lock (gate)
            {
                if (isCompleted)
                {
                    return;
                }

                isCompleted = true;
                dataAvailable.Release();
            }
        }

        public override void Flush()
        {
        }

        public override Task FlushAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            return ReadAsync(buffer.AsMemory(offset, count)).GetAwaiter().GetResult();
        }

        public override async ValueTask<int> ReadAsync(Memory<byte> buffer, CancellationToken cancellationToken = default)
        {
            if (disposed)
            {
                throw new ObjectDisposedException(nameof(ProducerConsumerStream));
            }

            while (true)
            {
                if (currentBuffer != null && currentOffset < currentBuffer.Length)
                {
                    var toCopy = Math.Min(buffer.Length, currentBuffer.Length - currentOffset);
                    currentBuffer.AsMemory(currentOffset, toCopy).CopyTo(buffer);
                    currentOffset += toCopy;

                    if (currentOffset >= currentBuffer.Length)
                    {
                        currentBuffer = null;
                        currentOffset = 0;
                    }

                    return toCopy;
                }

                await dataAvailable.WaitAsync(cancellationToken).ConfigureAwait(false);

                lock (gate)
                {
                    if (queue.Count > 0)
                    {
                        currentBuffer = queue.Dequeue();
                        currentOffset = 0;
                    }
                    else if (isCompleted)
                    {
                        return 0;
                    }
                }
            }
        }

        public override long Seek(long offset, SeekOrigin origin)
        {
            throw new NotSupportedException();
        }

        public override void SetLength(long value)
        {
            throw new NotSupportedException();
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            WriteAsync(buffer.AsMemory(offset, count)).GetAwaiter().GetResult();
        }

        public override async ValueTask WriteAsync(ReadOnlyMemory<byte> buffer, CancellationToken cancellationToken = default)
        {
            if (disposed)
            {
                throw new ObjectDisposedException(nameof(ProducerConsumerStream));
            }

            if (isCompleted)
            {
                throw new InvalidOperationException("Stream already completed.");
            }

            var copy = buffer.ToArray();

            lock (gate)
            {
                queue.Enqueue(copy);
            }

            dataAvailable.Release();
            await Task.CompletedTask.ConfigureAwait(false);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposed)
            {
                return;
            }

            disposed = true;
            Complete();
            dataAvailable.Dispose();
            base.Dispose(disposing);
        }
    }

    internal sealed class DuplexStreamContent : HttpContent
    {
        private readonly ProducerConsumerStream source;

        public DuplexStreamContent(ProducerConsumerStream source)
        {
            this.source = source ?? throw new ArgumentNullException(nameof(source));
        }

        protected override Task SerializeToStreamAsync(Stream stream, TransportContext context)
        {
            return SerializeToStreamAsync(stream, context, CancellationToken.None);
        }

        protected override async Task SerializeToStreamAsync(Stream stream, TransportContext context, CancellationToken cancellationToken)
        {
            await source.CopyToAsync(stream, cancellationToken).ConfigureAwait(false);
        }

        protected override bool TryComputeLength(out long length)
        {
            length = 0;
            return false;
        }
    }

    internal sealed class Http2WebSocket : WebSocket
    {
        private readonly ProducerConsumerStream requestStream;
        private readonly Stream responseStream;
        private readonly HttpResponseMessage response;
        private WebSocketCloseStatus? closeStatus;
        private string closeStatusDescription;
        private WebSocketState state = WebSocketState.Open;

        public Http2WebSocket(ProducerConsumerStream requestStream, Stream responseStream, HttpResponseMessage response)
        {
            this.requestStream = requestStream ?? throw new ArgumentNullException(nameof(requestStream));
            this.responseStream = responseStream ?? throw new ArgumentNullException(nameof(responseStream));
            this.response = response ?? throw new ArgumentNullException(nameof(response));
        }

        public override WebSocketCloseStatus? CloseStatus => closeStatus;

        public override string CloseStatusDescription => closeStatusDescription;

        public override WebSocketState State => state;

        public override string SubProtocol => null;

        public override void Abort()
        {
            state = WebSocketState.Aborted;
            requestStream.Complete();
            response.Dispose();
        }

        public override async Task CloseAsync(WebSocketCloseStatus closeStatus, string statusDescription, CancellationToken cancellationToken)
        {
            this.closeStatus ??= closeStatus;
            closeStatusDescription ??= statusDescription;
            state = WebSocketState.Closed;
            requestStream.Complete();
            await responseStream.FlushAsync(cancellationToken).ConfigureAwait(false);
            response.Dispose();
        }

        public override Task CloseOutputAsync(WebSocketCloseStatus closeStatus, string statusDescription, CancellationToken cancellationToken)
        {
            return CloseAsync(closeStatus, statusDescription, cancellationToken);
        }

        public override void Dispose()
        {
            if (state != WebSocketState.Closed && state != WebSocketState.Aborted)
            {
                requestStream.Complete();
            }

            response.Dispose();
            responseStream.Dispose();
            state = WebSocketState.Closed;
            base.Dispose();
        }

        public override async Task<WebSocketReceiveResult> ReceiveAsync(ArraySegment<byte> buffer, CancellationToken cancellationToken)
        {
            var bytesRead = await responseStream.ReadAsync(buffer, cancellationToken).ConfigureAwait(false);

            if (bytesRead == 0)
            {
                closeStatus ??= WebSocketCloseStatus.NormalClosure;
                state = WebSocketState.CloseReceived;
                return new WebSocketReceiveResult(0, WebSocketMessageType.Close, true, closeStatus, closeStatusDescription);
            }

            return new WebSocketReceiveResult(bytesRead, WebSocketMessageType.Binary, true);
        }

        public override async Task SendAsync(ArraySegment<byte> buffer, WebSocketMessageType messageType, bool endOfMessage, CancellationToken cancellationToken)
        {
            if (state == WebSocketState.Closed || state == WebSocketState.Aborted)
            {
                throw new WebSocketException(WebSocketError.InvalidState);
            }

            if (messageType == WebSocketMessageType.Close)
            {
                await CloseAsync(WebSocketCloseStatus.NormalClosure, string.Empty, cancellationToken).ConfigureAwait(false);
                return;
            }

            await requestStream.WriteAsync(buffer.AsMemory(), cancellationToken).ConfigureAwait(false);
        }
    }
}
