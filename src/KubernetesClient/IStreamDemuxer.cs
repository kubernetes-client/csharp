using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace k8s
{
    /// <summary>
    /// <para>
    ///     The <see cref="IStreamDemuxer"/> interface allows you to interact with processes running in a container in a Kubernetes pod. You can start an exec or attach command
    ///     by calling <see cref="Kubernetes.WebSocketNamespacedPodExecAsync(string, string, IEnumerable{string}, string, bool, bool, bool, bool, Dictionary{string, List{string}}, CancellationToken)"/>
    ///     or <see cref="Kubernetes.WebSocketNamespacedPodAttachAsync(string, string, string, bool, bool, bool, bool, Dictionary{string, List{string}}, CancellationToken)"/>. These methods
    ///     will return you a <see cref="WebSocket"/> connection.
    /// </para>
    /// <para>
    ///     Kubernetes 'multiplexes' multiple channels over this <see cref="WebSocket"/> connection, such as standard input, standard output and standard error. The <see cref="StreamDemuxer"/>
    ///     allows you to extract individual <see cref="Stream"/>s from this <see cref="WebSocket"/> class. You can then use these streams to send/receive data from that process.
    /// </para>
    /// </summary>
    public interface IStreamDemuxer : IDisposable
    {
        /// <summary>
        /// Starts reading the data sent by the server.
        /// </summary>
        void Start();

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
        Stream GetStream(ChannelIndex? inputIndex, ChannelIndex? outputIndex);

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
        Stream GetStream(byte? inputIndex, byte? outputIndex);

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
        Task Write(ChannelIndex index, byte[] buffer, int offset, int count,
            CancellationToken cancellationToken = default);

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
        Task Write(byte index, byte[] buffer, int offset, int count,
            CancellationToken cancellationToken = default);
    }
}
