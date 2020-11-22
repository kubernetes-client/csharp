using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Net.WebSockets;
using System.Threading.Tasks;

namespace k8s.Tests.Mock.Server.Controllers
{
    /// <summary>
    ///     Controller for the mock Kubernetes pod-port-forward API.
    /// </summary>
    [Route("api/v1")]
    public class PodPortForwardController
        : Controller
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PodPortForwardController"/> class.
        ///     Create a new <see cref="PodPortForwardController"/>.
        /// </summary>
        /// <param name="webSocketTestAdapter">
        ///     The adapter used to capture sockets accepted by the test server and provide them to the calling test.
        /// </param>
        public PodPortForwardController(WebSocketTestAdapter webSocketTestAdapter)
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
        ///     Mock Kubernetes API: port-forward for pod.
        /// </summary>
        /// <param name="kubeNamespace">
        ///     The target pod's containing namespace.
        /// </param>
        /// <param name="podName">
        ///     The target pod's name.
        /// </param>
        /// <param name="ports">
        ///     The port(s) to forward to the pod.
        /// </param>
        [Route("namespaces/{kubeNamespace}/pods/{podName}/portforward")]
        public async Task<IActionResult> Exec(string kubeNamespace, string podName, IEnumerable<string> ports)
        {
            if (!HttpContext.WebSockets.IsWebSocketRequest)
            {
                return BadRequest("PortForward requires WebSockets");
            }

            var webSocket = await HttpContext.WebSockets.AcceptWebSocketAsync(
                WebSocketProtocol.ChannelWebSocketProtocol).ConfigureAwait(false);

            WebSocketTestAdapter.AcceptedPodPortForwardV1Connection.AcceptServerSocket(webSocket);

            await WebSocketTestAdapter.TestCompleted.ConfigureAwait(false);

            return Ok();
        }
    }
}
