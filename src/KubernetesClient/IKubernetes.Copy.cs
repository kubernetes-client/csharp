using System.Threading;
using System.Threading.Tasks;

namespace k8s
{
    public partial interface IKubernetes
    {
        /// <summary>
        /// Copy a file from pod to the local file system.
        /// </summary>
        /// <param name="name">
        /// The name of the pod which contains the container in which to copy from.
        /// </param>
        /// <param name="namespace">
        /// The namespace of the container.
        /// </param>
        /// <param name="container">
        /// The container in which to copy from.
        /// </param>
        /// <param name="srcPath">
        /// The pod file path to copy from.
        /// </param>
        /// <param name="destPath">
        /// The local file path to copy to.
        /// </param>
        /// <param name="cancellationToken">
        /// A <see cref="CancellationToken"/> which can be used to cancel the asynchronous operation.
        /// </param>
        /// <returns>
        /// A <see cref="Task"/> which represents the asynchronous operation.
        /// </returns>
        Task<int> CopyFileFromPodAsync(string name, string @namespace, string container, string srcPath, string destPath, CancellationToken cancellationToken);
    
        /// <summary>
        /// Copy a file from the local file system to a container in a pod.
        /// </summary>
        /// <param name="name">
        /// The name of the pod which contains the container in which to save the file.
        /// </param>
        /// <param name="namespace">
        /// The namespace of the container.
        /// </param>
        /// <param name="container">
        /// The container in which to save the file.
        /// </param>
        /// <param name="srcPath">
        /// The local file path to copy from.
        /// </param>
        /// <param name="destPath">
        /// The pod file path to copy to.
        /// </param>
        /// <param name="cancellationToken">
        /// A <see cref="CancellationToken"/> which can be used to cancel the asynchronous operation.
        /// </param>
        /// <returns>
        /// A <see cref="Task"/> which represents the asynchronous operation.
        /// </returns>
        Task<int> CopyFileToPodAsync(string name, string @namespace, string container, string srcPath, string destPath, CancellationToken cancellationToken);
    }
}
