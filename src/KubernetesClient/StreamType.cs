using System;

namespace k8s
{
    /// <summary>
    /// When creating a <see cref="StreamDemuxer"/> object, specify <see cref="StreamType"/> to properly handle
    /// the underlying communication.
    /// </summary>
    public enum StreamType
    {
        /// <summary>
        /// This <see cref="StreamDemuxer"/> object is used to stream a remote command or attach to a remote
        /// container.
        /// </summary>
        RemoteCommand,

        /// <summary>
        /// This <see cref="StreamDemuxer"/> object is used in port forwarding.
        /// </summary>
        PortForward
    }
}
