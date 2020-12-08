using System.Collections.Generic;
using System.Net.WebSockets;
using System.Threading;
using System.Threading.Tasks;

namespace k8s
{
    public partial interface IKubernetes
    {
        /// <summary>
        /// Executes a command in a pod.
        /// </summary>
        /// <param name='name'>
        /// name of the Pod
        /// </param>
        /// <param name='namespace'>
        /// object name and auth scope, such as for teams and projects
        /// </param>
        /// <param name='command'>
        /// Command is the remote command to execute. argv array. Not executed within a
        /// shell.
        /// </param>
        /// <param name='container'>
        /// Container in which to execute the command. Defaults to only container if
        /// there is only one container in the pod.
        /// </param>
        /// <param name='stderr'>
        /// Redirect the standard error stream of the pod for this call. Defaults to
        /// <see langword="true"/>.
        /// </param>
        /// <param name='stdin'>
        /// Redirect the standard input stream of the pod for this call. Defaults to
        /// <see langword="true"/>.
        /// </param>
        /// <param name='stdout'>
        /// Redirect the standard output stream of the pod for this call. Defaults to
        /// <see langword="true"/>.
        /// </param>
        /// <param name='tty'>
        /// TTY if true indicates that a tty will be allocated for the exec call.
        /// Defaults to <see langword="true"/>.
        /// </param>
        /// <param name="webSocketSubProtocol">
        /// The Kubernetes-specific WebSocket sub protocol to use. See <see cref="WebSocketProtocol"/> for a list of available
        /// protocols.
        /// </param>
        /// <param name='customHeaders'>
        /// Headers that will be added to request.
        /// </param>
        /// <param name='cancellationToken'>
        /// The cancellation token.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// Thrown when a required parameter is null
        /// </exception>
        /// <returns>
        /// A <see cref="ClientWebSocket"/> which can be used to communicate with the process running in the pod.
        /// </returns>
        Task<WebSocket> WebSocketNamespacedPodExecAsync(string name, string @namespace = "default",
            string command = null, string container = null, bool stderr = true, bool stdin = true, bool stdout = true,
            bool tty = true, string webSocketSubProtocol = null, Dictionary<string, List<string>> customHeaders = null,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Executes a command in a pod.
        /// </summary>
        /// <param name='name'>
        /// name of the Pod
        /// </param>
        /// <param name='namespace'>
        /// object name and auth scope, such as for teams and projects
        /// </param>
        /// <param name='command'>
        /// Command is the remote command to execute. argv array. Not executed within a
        /// shell.
        /// </param>
        /// <param name='container'>
        /// Container in which to execute the command. Defaults to only container if
        /// there is only one container in the pod.
        /// </param>
        /// <param name='stderr'>
        /// Redirect the standard error stream of the pod for this call. Defaults to
        /// <see langword="true"/>.
        /// </param>
        /// <param name='stdin'>
        /// Redirect the standard input stream of the pod for this call. Defaults to
        /// <see langword="true"/>.
        /// </param>
        /// <param name='stdout'>
        /// Redirect the standard output stream of the pod for this call. Defaults to
        /// <see langword="true"/>.
        /// </param>
        /// <param name='tty'>
        /// TTY if true indicates that a tty will be allocated for the exec call.
        /// Defaults to <see langword="true"/>.
        /// </param>
        /// <param name="webSocketSubProtocol">
        /// The Kubernetes-specific WebSocket sub protocol to use. See <see cref="WebSocketProtocol"/> for a list of available
        /// protocols.
        /// </param>
        /// <param name='customHeaders'>
        /// Headers that will be added to request.
        /// </param>
        /// <param name='cancellationToken'>
        /// The cancellation token.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// Thrown when a required parameter is null
        /// </exception>
        /// <returns>
        /// A <see cref="ClientWebSocket"/> which can be used to communicate with the process running in the pod.
        /// </returns>
        Task<WebSocket> WebSocketNamespacedPodExecAsync(string name, string @namespace = "default",
            IEnumerable<string> command = null, string container = null, bool stderr = true, bool stdin = true,
            bool stdout = true, bool tty = true, string webSocketSubProtocol = null,
            Dictionary<string, List<string>> customHeaders = null,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Executes a command in a pod.
        /// </summary>
        /// <param name='name'>
        /// name of the Pod
        /// </param>
        /// <param name='namespace'>
        /// object name and auth scope, such as for teams and projects
        /// </param>
        /// <param name='command'>
        /// Command is the remote command to execute. argv array. Not executed within a
        /// shell.
        /// </param>
        /// <param name='container'>
        /// Container in which to execute the command. Defaults to only container if
        /// there is only one container in the pod.
        /// </param>
        /// <param name='stderr'>
        /// Redirect the standard error stream of the pod for this call. Defaults to
        /// <see langword="true"/>.
        /// </param>
        /// <param name='stdin'>
        /// Redirect the standard input stream of the pod for this call. Defaults to
        /// <see langword="true"/>.
        /// </param>
        /// <param name='stdout'>
        /// Redirect the standard output stream of the pod for this call. Defaults to
        /// <see langword="true"/>.
        /// </param>
        /// <param name='tty'>
        /// TTY if true indicates that a tty will be allocated for the exec call.
        /// Defaults to <see langword="true"/>.
        /// </param>
        /// <param name="webSocketSubProtocol">
        /// The Kubernetes-specific WebSocket sub protocol to use. See <see cref="WebSocketProtocol"/> for a list of available
        /// protocols.
        /// </param>
        /// <param name='customHeaders'>
        /// Headers that will be added to request.
        /// </param>
        /// <param name='cancellationToken'>
        /// The cancellation token.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// Thrown when a required parameter is null
        /// </exception>
        /// <returns>
        /// A <see cref="IStreamDemuxer"/> which can be used to communicate with the process running in the pod.
        /// </returns>
        Task<IStreamDemuxer> MuxedStreamNamespacedPodExecAsync(string name, string @namespace = "default",
            IEnumerable<string> command = null, string container = null, bool stderr = true, bool stdin = true,
            bool stdout = true, bool tty = true,
            string webSocketSubProtocol = WebSocketProtocol.V4BinaryWebsocketProtocol,
            Dictionary<string, List<string>> customHeaders = null,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Start port forwarding one or more ports of a pod.
        /// </summary>
        /// <param name='name'>
        /// The name of the Pod
        /// </param>
        /// <param name='namespace'>
        /// The object name and auth scope, such as for teams and projects
        /// </param>
        /// <param name='ports'>
        /// List of ports to forward.
        /// </param>
        /// <param name="webSocketSubProtocol">
        /// The Kubernetes-specific WebSocket sub protocol to use. See <see cref="WebSocketProtocol"/> for a list of available
        /// protocols.
        /// </param>
        /// <param name='customHeaders'>
        /// The headers that will be added to request.
        /// </param>
        /// <param name='cancellationToken'>
        /// The cancellation token.
        /// </param>
        /// <returns>
        /// A <see cref="ClientWebSocket"/> which can be used to communicate with the process running in the pod.
        /// </returns>
        Task<WebSocket> WebSocketNamespacedPodPortForwardAsync(string name, string @namespace, IEnumerable<int> ports,
            string webSocketSubProtocol = null, Dictionary<string, List<string>> customHeaders = null,
            CancellationToken cancellationToken = default);


        /// <summary>
        /// connect GET requests to attach of Pod
        /// </summary>
        /// <param name='name'>
        /// name of the Pod
        /// </param>
        /// <param name='namespace'>
        /// object name and auth scope, such as for teams and projects
        /// </param>
        /// <param name='container'>
        /// The container in which to execute the command. Defaults to only container
        /// if there is only one container in the pod.
        /// </param>
        /// <param name='stderr'>
        /// Stderr if true indicates that stderr is to be redirected for the attach
        /// call. Defaults to true.
        /// </param>
        /// <param name='stdin'>
        /// stdin if true, redirects the standard input stream of the pod for this
        /// call. Defaults to false.
        /// </param>
        /// <param name='stdout'>
        /// Stdout if true indicates that stdout is to be redirected for the attach
        /// call. Defaults to true.
        /// </param>
        /// <param name='tty'>
        /// TTY if true indicates that a tty will be allocated for the attach call.
        /// This is passed through the container runtime so the tty is allocated on the
        /// worker node by the container runtime. Defaults to false.
        /// </param>
        /// <param name="webSocketSubProtocol">
        /// The Kubernetes-specific WebSocket sub protocol to use. See <see cref="WebSocketProtocol"/> for a list of available
        /// protocols.
        /// </param>
        /// <param name='customHeaders'>
        /// Headers that will be added to request.
        /// </param>
        /// <param name='cancellationToken'>
        /// The cancellation token.
        /// </param>
        /// <returns>A response object containing the response body and response headers.</returns>
        Task<WebSocket> WebSocketNamespacedPodAttachAsync(string name, string @namespace,
            string container = default, bool stderr = true, bool stdin = false, bool stdout = true,
            bool tty = false, string webSocketSubProtocol = null, Dictionary<string, List<string>> customHeaders = null,
            CancellationToken cancellationToken = default);
    }
}
