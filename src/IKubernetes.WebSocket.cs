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
    }
}
