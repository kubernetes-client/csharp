using k8s.Models;
using System.Threading;
using System.Threading.Tasks;

namespace k8s
{
    public partial interface IKubernetes
    {
        /// <summary>
        /// Copy a file from a node into the local machine.
        /// </summary>
        /// <param name="pod">
        /// The pod object which contains the container in which the source file exist.
        /// </param>
        /// <param name="container">
        /// The container in which the source file exist.
        /// </param>
        /// <param name="sourcePath">
        /// The source file path.
        /// </param>
        /// <param name="destinationPath">
        /// The destination file path.
        /// </param>
        /// <param name="cancellationToken">
        /// A <see cref="CancellationToken"/> which can be used to cancel the asynchronous operation.
        /// </param>
        /// <returns>
        /// A <see cref="Task"/> which represents the asynchronous operation.
        /// </returns>
        Task CopyFileFromPod(V1Pod pod, string container, string sourcePath, string destinationPath, CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// Copy a file from a node into the local machine.
        /// </summary>
        /// <param name="name">
        /// The name of the pod which contains the container in which the source file exist.
        /// </param>
        /// <param name="namespace">
        /// The namespace of the container.
        /// </param>
        /// <param name="container">
        /// The container in which the source file exist.
        /// </param>
        /// <param name="sourcePath">
        /// The source file path.
        /// </param>
        /// <param name="destinationPath">
        /// The destination file path.
        /// </param>
        /// <param name="cancellationToken">
        /// A <see cref="CancellationToken"/> which can be used to cancel the asynchronous operation.
        /// </param>
        /// <returns>
        /// A <see cref="Task"/> which represents the asynchronous operation.
        /// </returns>
        Task CopyFileFromPod(string name, string @namespace, string container, string sourceFilePath, string destinationFilePath, CancellationToken cancellationToken = default(CancellationToken));
    }
}
