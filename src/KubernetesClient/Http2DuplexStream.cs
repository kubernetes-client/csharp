using System;
using System.Buffers;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.WebSockets;
using System.Threading;
using System.Threading.Tasks;

namespace k8s
{
    internal sealed class ProducerConsumerStream : Stream
    {
        private readonly struct BufferSegment
        {
            public BufferSegment(byte[] buffer, int length, bool rented)
            {
                Buffer = buffer;
                Length = length;
                Rented = rented;
            }

            public byte[] Buffer { get; }
            public int Length { get; }
            public bool Rented { get; }
        }

        private readonly Queue<BufferSegment> queue = new Queue<BufferSegment>();
        private readonly SemaphoreSlim dataAvailable = new SemaphoreSlim(0);
        private readonly object gate = new object();

        private BufferSegment currentSegment;
        private bool hasSegment;
        private int currentOffset;
        private bool isCompleted;
        private bool disposed;
        private const int BufferPoolThreshold = 1024;

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
            return ReadAsync(buffer.AsMemory(offset, count), CancellationToken.None).GetAwaiter().GetResult();
        }

        public override async ValueTask<int> ReadAsync(Memory<byte> buffer, CancellationToken cancellationToken = default)
        {
            if (disposed)
            {
                throw new ObjectDisposedException(nameof(ProducerConsumerStream));
            }

            while (true)
            {
                if (hasSegment && currentOffset < currentSegment.Length)
                {
                    var toCopy = Math.Min(buffer.Length, currentSegment.Length - currentOffset);
                    currentSegment.Buffer.AsMemory(currentOffset, toCopy).CopyTo(buffer);
                    currentOffset += toCopy;

                    if (currentOffset >= currentSegment.Length)
                    {
                        if (currentSegment.Rented)
                        {
                            ArrayPool<byte>.Shared.Return(currentSegment.Buffer);
                        }

                        currentSegment = default;
                        hasSegment = false;
                        currentOffset = 0;
                    }

                    return toCopy;
                }

                await dataAvailable.WaitAsync(cancellationToken).ConfigureAwait(false);

                lock (gate)
                {
                    if (queue.Count > 0)
                    {
                        currentSegment = queue.Dequeue();
                        hasSegment = true;
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
            WriteAsync(buffer.AsMemory(offset, count), CancellationToken.None).GetAwaiter().GetResult();
        }

        public override ValueTask WriteAsync(ReadOnlyMemory<byte> buffer, CancellationToken cancellationToken = default)
        {
            if (cancellationToken.IsCancellationRequested)
            {
                return ValueTask.FromCanceled(cancellationToken);
            }

            if (disposed)
            {
                throw new ObjectDisposedException(nameof(ProducerConsumerStream));
            }

            if (isCompleted)
            {
                throw new InvalidOperationException("Stream already completed.");
            }

            var dataLength = buffer.Length;
            var usePool = dataLength > BufferPoolThreshold;
            var rented = usePool ? ArrayPool<byte>.Shared.Rent(dataLength) : new byte[dataLength];
            try
            {
                buffer.Span.CopyTo(rented.AsSpan(0, dataLength));

                var segment = new BufferSegment(rented, dataLength, usePool);

                lock (gate)
                {
                    queue.Enqueue(segment);
                }

                dataAvailable.Release();
                return ValueTask.CompletedTask;
            }
            catch
            {
                if (usePool)
                {
                    ArrayPool<byte>.Shared.Return(rented);
                }
                throw;
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (disposed)
            {
                return;
            }

            lock (gate)
            {
                if (hasSegment && currentSegment.Rented)
                {
                    ArrayPool<byte>.Shared.Return(currentSegment.Buffer);
                }

                hasSegment = false;
                currentSegment = default;

                while (queue.Count > 0)
                {
                    var segment = queue.Dequeue();
                    if (segment.Rented)
                    {
                        ArrayPool<byte>.Shared.Return(segment.Buffer);
                    }
                }
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
        private readonly string subProtocol;
        private readonly object closeGate = new object();
        private WebSocketCloseStatus? closeStatus;
        private string closeStatusDescription;
        private WebSocketState state = WebSocketState.Open;

        public Http2WebSocket(ProducerConsumerStream requestStream, Stream responseStream, HttpResponseMessage response, string subProtocol)
        {
            this.requestStream = requestStream ?? throw new ArgumentNullException(nameof(requestStream));
            this.responseStream = responseStream ?? throw new ArgumentNullException(nameof(responseStream));
            this.response = response ?? throw new ArgumentNullException(nameof(response));
            this.subProtocol = subProtocol;
        }

        public override WebSocketCloseStatus? CloseStatus
        {
            get
            {
                lock (closeGate)
                {
                    return closeStatus;
                }
            }
        }

        public override string CloseStatusDescription
        {
            get
            {
                lock (closeGate)
                {
                    return closeStatusDescription;
                }
            }
        }

        public override WebSocketState State => state;

        public override string SubProtocol => subProtocol;

        public override void Abort()
        {
            state = WebSocketState.Aborted;
            requestStream.Complete();
            response.Dispose();
        }

        public override async Task CloseAsync(WebSocketCloseStatus closeStatus, string statusDescription, CancellationToken cancellationToken)
        {
            lock (closeGate)
            {
                if (this.closeStatus == null)
                {
                    this.closeStatus = closeStatus;
                }

                if (this.closeStatusDescription == null)
                {
                    this.closeStatusDescription = statusDescription;
                }
            }
            state = WebSocketState.Closed;
            requestStream.Complete();
            try
            {
                await responseStream.FlushAsync(cancellationToken).ConfigureAwait(false);
            }
            finally
            {
                response.Dispose();
            }
        }

        public override Task CloseOutputAsync(WebSocketCloseStatus closeStatus, string statusDescription, CancellationToken cancellationToken)
        {
            lock (closeGate)
            {
                if (this.closeStatus == null)
                {
                    this.closeStatus = closeStatus;
                }

                if (this.closeStatusDescription == null)
                {
                    this.closeStatusDescription = statusDescription;
                }
            }

            requestStream.Complete();
            state = WebSocketState.CloseSent;
            return Task.CompletedTask;
        }

        public override void Dispose()
        {
            if (state != WebSocketState.Closed && state != WebSocketState.Aborted)
            {
                requestStream.Complete();
            }

            response.Dispose();
            state = WebSocketState.Closed;
            base.Dispose();
        }

        public override async Task<WebSocketReceiveResult> ReceiveAsync(ArraySegment<byte> buffer, CancellationToken cancellationToken)
        {
            var memory = buffer.Array == null && buffer.Count == 0 ? Memory<byte>.Empty : buffer.AsMemory();
            var bytesRead = await responseStream.ReadAsync(memory, cancellationToken).ConfigureAwait(false);

            if (bytesRead == 0)
            {
                lock (closeGate)
                {
                    if (closeStatus == null)
                    {
                        closeStatus = WebSocketCloseStatus.NormalClosure;
                    }
                }
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
