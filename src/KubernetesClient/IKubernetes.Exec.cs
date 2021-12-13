using System.Threading;
using System.Threading.Tasks;

namespace k8s
{
    public partial interface IKubernetes
    {
        /// <summary>
        /// Executes a command in a container in a pod.
        /// </summary>
        /// <param name="name">
        /// The name of the pod which contains the container in which to execute the command.
        /// </param>
        /// <param name="namespace">
        /// The namespace of the container.
        /// </param>
        /// <param name="container">
        /// The container in which to run the command.
        /// </param>
        /// <param name="command">
        /// The command to execute.
        /// </param>
        /// <param name="tty">
        /// if allocate a pseudo-TTY
        /// </param>
        /// <param name="action">
        /// A callback which processes the standard input, standard output and standard error.
        /// </param>
        /// <param name="cancellationToken">
        /// A <see cref="CancellationToken"/> which can be used to cancel the asynchronous operation.
        /// </param>
        /// <returns>
        /// A <see cref="Task"/> which represents the asynchronous operation.
        /// </returns>
        Task<int> NamespacedPodExecAsync(string name, string @namespace, string container, IEnumerable<string> command,
            bool tty, ExecAsyncCallback action, CancellationToken cancellationToken);
    }
}
