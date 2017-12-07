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
        /// <param name='customHeaders'>
        /// Headers that will be added to request.
        /// </param>
        /// <param name='cancellationToken'>
        /// The cancellation token.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// Thrown when a required parameter is null
        /// </exception>
        /// <return>
        /// A <see cref="ClientWebSocket"/> which can be used to communicate with the process running in the pod.
        /// </return>
        Task<WebSocket> WebSocketNamespacedPodExecAsync(string name, string @namespace = "default", string command = "/bin/bash", string container = null, bool stderr = true, bool stdin = true, bool stdout = true, bool tty = true, Dictionary<string, List<string>> customHeaders = null, CancellationToken cancellationToken = default(CancellationToken));

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
        /// <param name='customHeaders'>
        /// The headers that will be added to request.
        /// </param>
        /// <param name='cancellationToken'>
        /// The cancellation token.
        /// </param>
        Task<WebSocket> WebSocketNamespacedPodPortForwardAsync(string name, string @namespace, IEnumerable<int> ports, Dictionary<string, List<string>> customHeaders = null, CancellationToken cancellationToken = default(CancellationToken));

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
        /// Stdin if true, redirects the standard input stream of the pod for this
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
        /// <param name='customHeaders'>
        /// Headers that will be added to request.
        /// </param>
        /// <param name='cancellationToken'>
        /// The cancellation token.
        /// </param>
        /// <exception cref="HttpOperationException">
        /// Thrown when the operation returned an invalid status code
        /// </exception>
        /// <exception cref="SerializationException">
        /// Thrown when unable to deserialize the response
        /// </exception>
        /// <exception cref="ValidationException">
        /// Thrown when a required parameter is null
        /// </exception>
        /// <exception cref="System.ArgumentNullException">
        /// Thrown when a required parameter is null
        /// </exception>
        /// <return>
        /// A response object containing the response body and response headers.
        /// </return>
        Task<WebSocket> WebSocketNamespacedPodAttachAsync(string name, string @namespace, string container = default(string), bool stderr = true, bool stdin = false, bool stdout = true, bool tty = false, Dictionary<string, List<string>> customHeaders = null, CancellationToken cancellationToken = default(CancellationToken));
    }
}
