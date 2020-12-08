using Microsoft.AspNetCore.Mvc;
using System;
using System.Net.WebSockets;
using System.Threading.Tasks;

namespace k8s.Tests.Mock.Server.Controllers
{
    /// <summary>
    ///     Controller for the mock Kubernetes exec-in-pod API.
    /// </summary>
    [Route("api/v1")]
    public class PodExecController
        : Controller
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PodExecController"/> class.
        ///     Create a new <see cref="PodExecController"/>.
        /// </summary>
        /// <param name="webSocketTestAdapter">
        ///     The adapter used to capture sockets accepted by the test server and provide them to the calling test.
        /// </param>
        public PodExecController(WebSocketTestAdapter webSocketTestAdapter)
        {
            if (webSocketTestAdapter == null)
            {
                throw new ArgumentNullException(nameof(webSocketTestAdapter));
            }

            WebSocketTestAdapter = webSocketTestAdapter;
        }

        /// <summary>
        ///     The adapter used to capture sockets accepted by the test server and provide them to the calling test.
        /// </summary>
        private WebSocketTestAdapter WebSocketTestAdapter { get; }

        /// <summary>
        ///     Mock Kubernetes API: exec-in-pod.
        /// </summary>
        /// <param name="kubeNamespace">
        ///     The target pod's containing namespace.
        /// </param>
        /// <param name="podName">
        ///     The target pod's name.
        /// </param>
        [Route("namespaces/{kubeNamespace}/pods/{podName}/exec")]
        public async Task<IActionResult> Exec(string kubeNamespace, string podName)
        {
            if (!HttpContext.WebSockets.IsWebSocketRequest)
            {
                return BadRequest("Exec requires WebSockets");
            }

            var webSocket = await HttpContext.WebSockets.AcceptWebSocketAsync(
                WebSocketProtocol.ChannelWebSocketProtocol).ConfigureAwait(false);

            WebSocketTestAdapter.AcceptedPodExecV1Connection.AcceptServerSocket(webSocket);

            await WebSocketTestAdapter.TestCompleted.ConfigureAwait(false);

            return Ok();
        }
    }
}
