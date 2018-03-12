using System;
using System.Net.WebSockets;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace k8s.Tests.Mock.Server
{
    /// <summary>
    ///     Adapter used to capture WebSockets accepted by the test server and provide them to calling test.
    /// </summary>
    /// <remarks>
    ///     Each AcceptedXXXConnection property returns an awaitable object that yields a the server-side WebSocket once a connection has been accepted.
    ///
    ///     All server-side WebSockets will be closed when <see cref="CompleteTest"/> is called.
    /// </remarks>
    public class WebSocketTestAdapter
    {
        /// <summary>
        ///     Completion source for the <see cref="TestCompleted"/> task.
        /// </summary>
        readonly TaskCompletionSource<object> _testCompletion = new TaskCompletionSource<object>();

        /// <summary>
        ///     A <see cref="Task"/> that completes when the test is complete (providing <see cref="CompleteTest"/> is called).
        /// </summary>
        public Task TestCompleted => _testCompletion.Task;

        /// <summary>
        ///     <c>await</c> server-side acceptance of a WebSocket connection for the exec-in-pod (v1) API.
        /// </summary>
        public ServerSocketAcceptance AcceptedPodExecV1Connection { get; } = new ServerSocketAcceptance();

        /// <summary>
        ///     <c>await</c> server-side acceptance of a WebSocket connection for the pod-port-forward (v1) API.
        /// </summary>
        public ServerSocketAcceptance AcceptedPodPortForwardV1Connection { get; } = new ServerSocketAcceptance();

        /// <summary>
        ///     Mark the current test as complete, closing all server-side sockets.
        /// </summary>
        public void CompleteTest() => _testCompletion.SetResult(true);

        /// <summary>
        ///     An object that enables awaiting server-side acceptance of a WebSocket connection.
        /// </summary>
        /// <remarks>
        ///     Simply <c>await</c> this object to wait for the server socket to be accepted.
        /// </remarks>
        public class ServerSocketAcceptance
        {
            /// <summary>
            ///     Completion source for the <see cref="ServerSocketAccepted"/> task.
            /// </summary>
            readonly TaskCompletionSource<WebSocket> _completion = new TaskCompletionSource<WebSocket>();

            /// <summary>
            ///     A <see cref="Task"/> that completes when the server accepts a WebSocket connection (i.e. when <see cref="AcceptServerSocket"/> or <see cref="RejectServerSocket"/> is called).
            /// </summary>
            public Task<WebSocket> Task => _completion.Task;

            /// <summary>
            ///     Notify the calling test that the server has accepted a WebSocket connection.
            /// </summary>
            /// <param name="serverSocket">
            ///     The server-side <see cref="WebSocket"/>.
            /// </param>
            public void AcceptServerSocket(WebSocket serverSocket)
            {
                if (serverSocket == null)
                    throw new ArgumentNullException(nameof(serverSocket));

                _completion.SetResult(serverSocket);
            }

            /// <summary>
            ///     Notify the calling test that the server has rejected a WebSocket connection.
            /// </summary>
            /// <param name="reason">
            ///     An <see cref="Exception"/> representing the reason that the connection was rejected.
            /// </param>
            public void RejectServerSocket(Exception reason)
            {
                if (reason == null)
                    throw new ArgumentNullException(nameof(reason));

                _completion.SetException(reason);
            }

            /// <summary>
            ///     Get an awaiter for the socket-acceptance task.
            /// </summary>
            /// <returns>
            ///     The <see cref="TaskAwaiter{TResult}"/>.
            /// </returns>
            public TaskAwaiter<WebSocket> GetAwaiter() => Task.GetAwaiter();
        }
    }
}
