namespace k8s
{
    /// <summary>
    ///     Well-known WebSocket sub-protocols used by the Kubernetes API.
    /// </summary>
    public static class K8sProtocol
    {
        /// <summary>
        ///     Version 1 of the Kubernetes channel WebSocket protocol.
        /// </summary>
        /// <remarks>
        ///     This protocol prepends each binary message with a byte indicating the channel number (zero indexed) that the message was sent on.
        ///     Messages in both directions should prefix their messages with this channel byte.
        ///
        ///     When used for remote execution, the channel numbers are by convention defined to match the POSIX file-descriptors assigned to STDIN, STDOUT, and STDERR (0, 1, and 2).
        ///     No other conversion is performed on the raw subprotocol - writes are sent as they are received by the server.
        ///
        ///     Example client session:
        ///
        ///        CONNECT http://server.com with subprotocol "channel.k8s.io"
        ///        WRITE []byte{0, 102, 111, 111, 10} # send "foo\n" on channel 0 (STDIN)
        ///        READ  []byte{1, 10}                # receive "\n" on channel 1 (STDOUT)
        ///        CLOSE
        /// </remarks>
        public static readonly string ChannelV1 = "channel.k8s.io";

        /// <summary>
        ///     Version 1 of the Kubernetes Base64-encoded channel WebSocket protocol.
        /// </summary>
        /// <remarks>
        ///     This protocol base64 encodes each message with a character representing the channel number (zero indexed) the message was sent on (if the channel number is 1, then the character is '1', i.e. a byte value of 49).
        ///     Messages in both directions should prefix their messages with this character.
        ///
        ///     When used for remote execution, the channel numbers are by convention defined to match the POSIX file-descriptors assigned to STDIN, STDOUT, and STDERR ('0', '1', and '2').
        ///     The data received on the server is base64 decoded (and must be be valid) and data written by the server to the client is base64 encoded.
        ///
        ///     Example client session:
        ///
        ///        CONNECT http://server.com with subprotocol "base64.channel.k8s.io"
        ///        WRITE []byte{48, 90, 109, 57, 118, 67, 103, 111, 61} # send "foo\n" (base64: "Zm9vCgo=") on channel '0' (STDIN)
        ///        READ  []byte{49, 67, 103, 61, 61} # receive "\n" (base64: "Cg==") on channel '1' (STDOUT)
        ///        CLOSE
        /// </remarks>
        public static readonly string ChannelBase64V1 = "base64.channel.k8s.io";
    }
}
