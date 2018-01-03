using System;
using System.Buffers;
using System.Collections.Generic;
using System.IO;
using System.Net.WebSockets;
using System.Threading;
using System.Threading.Tasks;

namespace k8s
{
    public class StreamDemuxer : IDisposable
    {
        private readonly WebSocket webSocket;
        private readonly Dictionary<byte, ByteBuffer> buffers = new Dictionary<byte, ByteBuffer>();
        private readonly CancellationTokenSource cts = new CancellationTokenSource();
        private Task runLoop;

        public StreamDemuxer(WebSocket webSocket)
        {
            this.webSocket = webSocket ?? throw new ArgumentNullException(nameof(webSocket));
        }

        public event EventHandler ConnectionClosed;

        public void Start()
        {
            this.runLoop = this.RunLoop(this.cts.Token);
        }

        public void Dispose()
        {
            if (this.runLoop != null)
            {
                this.cts.Cancel();
                this.runLoop.Wait();
            }
        }

        public Stream GetStream(byte? inputIndex, byte? outputIndex)
        {
            if (inputIndex != null && !this.buffers.ContainsKey(inputIndex.Value))
            {
                lock (this.buffers)
                {
                    var buffer = new ByteBuffer();
                    this.buffers.Add(inputIndex.Value, buffer);
                }
            }

            var inputBuffer = inputIndex == null ? null : this.buffers[inputIndex.Value];
            return new MuxedStream(this, inputBuffer, outputIndex);
        }

        public async Task Write(byte index, byte[] buffer, int offset, int count, CancellationToken cancellationToken = default(CancellationToken))
        {
            byte[] writeBuffer = ArrayPool<byte>.Shared.Rent(count + 1);

            try
            {
                writeBuffer[0] = (byte)index;
                Array.Copy(buffer, offset, writeBuffer, 1, count);
                ArraySegment<byte> segment = new ArraySegment<byte>(writeBuffer, 0, count + 1);
                await this.webSocket.SendAsync(segment, WebSocketMessageType.Binary, false, cancellationToken).ConfigureAwait(false);
            }
            finally
            {
                ArrayPool<byte>.Shared.Return(writeBuffer);
            }
        }

        protected async Task RunLoop(CancellationToken cancellationToken)
        {
            // Get a 1KB buffer
            byte[] buffer = ArrayPool<byte>.Shared.Rent(1024 * 1024);

            try
            {
                var segment = new ArraySegment<byte>(buffer);

                while (!cancellationToken.IsCancellationRequested && this.webSocket.CloseStatus == null)
                {
                    // We always get data in this format:
                    // [stream index] (1 for stdout, 2 for stderr)
                    // [payload]
                    var result = await this.webSocket.ReceiveAsync(segment, cancellationToken).ConfigureAwait(false);

                    // Ignore empty messages
                    if (result.Count < 2)
                    {
                        continue;
                    }

                    var streamIndex = buffer[0];
                    var extraByteCount = 1;

                    while (true)
                    {
                        if (this.buffers.ContainsKey(streamIndex))
                        {
                            this.buffers[streamIndex].Write(buffer, extraByteCount, result.Count - extraByteCount);
                        }

                        if (result.EndOfMessage == true)
                        {
                            break;
                        }

                        extraByteCount = 0;
                        result = await this.webSocket.ReceiveAsync(segment, cancellationToken).ConfigureAwait(false);
                    }
                }

            }
            finally
            {
                ArrayPool<byte>.Shared.Return(buffer);
                this.runLoop = null;

                foreach (var b in this.buffers.Values)
                {
                    b.WriteEnd();
                }

                this.ConnectionClosed?.Invoke(this, EventArgs.Empty);
            }
        }
    }
}
