namespace k8s
{
    /// <summary>
    ///     Well-known WebSocket sub-protocols used by the Kubernetes API.
    /// </summary>
    public static class WebSocketProtocol
    {
        // The protocols are defined here:
        // https://github.com/kubernetes/kubernetes/blob/master/pkg/kubelet/server/remotecommand/websocket.go#L36
        // and here:
        // https://github.com/kubernetes/kubernetes/blob/714f97d7baf4975ad3aa47735a868a81a984d1f0/staging/src/k8s.io/apiserver/pkg/util/wsstream/conn.go
        //
        // For a description of what's different in v4:
        // https://github.com/kubernetes/kubernetes/blob/317853c90c674920bfbbdac54fe66092ddc9f15f/pkg/kubelet/server/remotecommand/httpstream.go#L203

        /// <summary>
        /// <para>
        /// The Websocket subprotocol "channel.k8s.io" prepends each binary message with a byte indicating
        /// the channel number (zero indexed) the message was sent on. Messages in both directions should
        /// prefix their messages with this channel byte. When used for remote execution, the channel numbers
        /// are by convention defined to match the POSIX file-descriptors assigned to STDIN, STDOUT, and STDERR
        /// (0, 1, and 2). No other conversion is performed on the raw subprotocol - writes are sent as they
        /// are received by the server.
        /// </para>
        ///
        /// <para>
        /// Example client session:
        /// <code>
        ///    CONNECT http://server.com with subprotocol "channel.k8s.io"
        ///    WRITE []byte{0, 102, 111, 111, 10} # send "foo\n" on channel 0 (STDIN)
        ///    READ  []byte{1, 10}                # receive "\n" on channel 1 (STDOUT)
        ///    CLOSE
        /// </code>
        /// </para>
        /// </summary>
        public const string ChannelWebSocketProtocol = "channel.k8s.io";

        /// <summary>
        /// <para>
        /// The Websocket subprotocol "base64.channel.k8s.io" base64 encodes each message with a character
        /// indicating the channel number (zero indexed) the message was sent on. Messages in both directions
        /// should prefix their messages with this channel char. When used for remote execution, the channel
        /// numbers are by convention defined to match the POSIX file-descriptors assigned to STDIN, STDOUT,
        /// and STDERR ('0', '1', and '2'). The data received on the server is base64 decoded (and must be
        /// be valid) and data written by the server to the client is base64 encoded.
        /// </para>
        ///
        /// <para>
        /// Example client session:
        /// <code>
        ///    CONNECT http://server.com with subprotocol "base64.channel.k8s.io"
        ///    WRITE []byte{48, 90, 109, 57, 118, 67, 103, 111, 61} # send "foo\n" (base64: "Zm9vCgo=") on channel '0' (STDIN)
        ///    READ  []byte{49, 67, 103, 61, 61} # receive "\n" (base64: "Cg==") on channel '1' (STDOUT)
        ///    CLOSE
        /// </code>
        /// </para>
        /// </summary>
        public const string Base64ChannelWebSocketProtocol = "base64.channel.k8s.io";

        /// <summary>
        /// v4ProtocolHandler implements the V4 protocol version for streaming command execution. It only differs
        /// in from v3 in the error stream format using an json-marshaled metav1.Status which carries
        /// the process' exit code.
        /// </summary>
        public const string V4BinaryWebsocketProtocol = "v4." + ChannelWebSocketProtocol;

        /// <summary>
        /// v4ProtocolHandler implements the V4 protocol version for streaming command execution. It only differs
        /// in from v3 in the error stream format using an json-marshaled metav1.Status which carries
        /// the process' exit code.
        /// </summary>
        public const string V4Base64WebsocketProtocol = "v4." + Base64ChannelWebSocketProtocol;
    }
}
