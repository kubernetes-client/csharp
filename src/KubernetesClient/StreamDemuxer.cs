using System;
using System.Buffers;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net.WebSockets;
using System.Threading;
using System.Threading.Tasks;

namespace k8s
{
    /// <summary>
    /// <para>
    ///     The <see cref="StreamDemuxer"/> allows you to interact with processes running in a container in a Kubernetes pod. You can start an exec or attach command
    ///     by calling <see cref="Kubernetes.WebSocketNamespacedPodExecAsync(string, string, IEnumerable{string}, string, bool, bool, bool, bool, Dictionary{string, List{string}}, CancellationToken)"/>
    ///     or <see cref="Kubernetes.WebSocketNamespacedPodAttachAsync(string, string, string, bool, bool, bool, bool, Dictionary{string, List{string}}, CancellationToken)"/>. These methods
    ///     will return you a <see cref="WebSocket"/> connection.
    /// </para>
    /// <para>
    ///     Kubernetes 'multiplexes' multiple channels over this <see cref="WebSocket"/> connection, such as standard input, standard output and standard error. The <see cref="StreamDemuxer"/>
    ///     allows you to extract individual <see cref="Stream"/>s from this <see cref="WebSocket"/> class. You can then use these streams to send/receive data from that process.
    /// </para>
    /// </summary>
    public class StreamDemuxer : IDisposable
    {
        private readonly WebSocket webSocket;
        private readonly Dictionary<byte, ByteBuffer> buffers = new Dictionary<byte, ByteBuffer>();
        private readonly CancellationTokenSource cts = new CancellationTokenSource();
        private Task runLoop;

        /// <summary>
        /// Initializes a new instance of the <see cref="StreamDemuxer"/> class.
        /// </summary>
        /// <param name="webSocket">
        /// A <see cref="WebSocket"/> which contains a multiplexed stream, such as the <see cref="WebSocket"/> returned by the exec or attach commands.
        /// </param>
        public StreamDemuxer(WebSocket webSocket)
        {
            this.webSocket = webSocket ?? throw new ArgumentNullException(nameof(webSocket));
        }

        public event EventHandler ConnectionClosed;

        /// <summary>
        /// Starts reading the data sent by the server.
        /// </summary>
        public void Start()
        {
            this.runLoop = this.RunLoop(this.cts.Token);
        }

        /// <inheritdoc/>
        public void Dispose()
        {
            try
            {
                if (this.runLoop != null)
                {
                    this.cts.Cancel();
                    this.runLoop.Wait();
                }
            }
            catch (Exception ex)
            {
                // Dispose methods can never throw.
                Debug.Write(ex);
            }
        }

        /// <summary>
        /// Gets a <see cref="Stream"/> which allows you to read to and/or write from a remote channel.
        /// </summary>
        /// <param name="inputIndex">
        /// The index of the channel from which to read.
        /// </param>
        /// <param name="outputIndex">
        /// The index of the channel to which to write.
        /// </param>
        /// <returns>
        /// A <see cref="Stream"/> which allows you to read/write to the requested channels.
        /// </returns>
        public Stream GetStream(ChannelIndex? inputIndex, ChannelIndex? outputIndex)
        {
            return GetStream((byte?)inputIndex, (byte?)outputIndex);
        }

        /// <summary>
        /// Gets a <see cref="Stream"/> which allows you to read to and/or write from a remote channel.
        /// </summary>
        /// <param name="inputIndex">
        /// The index of the channel from which to read.
        /// </param>
        /// <param name="outputIndex">
        /// The index of the channel to which to write.
        /// </param>
        /// <returns>
        /// A <see cref="Stream"/> which allows you to read/write to the requested channels.
        /// </returns>
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

        /// <summary>
        /// Directly writes data to a channel.
        /// </summary>
        /// <param name="index">
        /// The index of the channel to which to write.
        /// </param>
        /// <param name="buffer">
        /// The buffer from which to read data.
        /// </param>
        /// <param name="offset">
        /// The offset at which to start reading.
        /// </param>
        /// <param name="count">
        /// The number of bytes to read.
        /// </param>
        /// <param name="cancellationToken">
        /// A <see cref="CancellationToken"/> which can be used to cancel the asynchronous operation.
        /// </param>
        /// <returns>
        /// A <see cref="Task"/> which represents the asynchronous operation.
        /// </returns>
        public Task Write(ChannelIndex index, byte[] buffer, int offset, int count, CancellationToken cancellationToken = default(CancellationToken))
        {
            return Write((byte)index, buffer, offset, count, cancellationToken);
        }

        /// <summary>
        /// Directly writes data to a channel.
        /// </summary>
        /// <param name="index">
        /// The index of the channel to which to write.
        /// </param>
        /// <param name="buffer">
        /// The buffer from which to read data.
        /// </param>
        /// <param name="offset">
        /// The offset at which to start reading.
        /// </param>
        /// <param name="count">
        /// The number of bytes to read.
        /// </param>
        /// <param name="cancellationToken">
        /// A <see cref="CancellationToken"/> which can be used to cancel the asynchronous operation.
        /// </param>
        /// <returns>
        /// A <see cref="Task"/> which represents the asynchronous operation.
        /// </returns>
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
