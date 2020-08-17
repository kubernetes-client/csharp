using k8s.Models;
using System.Threading;
using System.Threading.Tasks;

namespace k8s
{
    public partial interface IKubernetes
    {
        /// <summary>
        /// Copy a file from a pod's container into the local machine.
        /// </summary>
        /// <param name="pod">
        /// The pod object which contains the container in which the source file exist.
        /// </param>
        /// <param name="container">
        /// The container in which the source file exist.
        /// </param>
        /// <param name="sourceFilePath">
        /// The source file path.
        /// </param>
        /// <param name="destinationFilePath">
        /// The destination file path.
        /// </param>
        /// <param name="cancellationToken">
        /// A <see cref="CancellationToken"/> which can be used to cancel the asynchronous operation.
        /// </param>
        /// <returns>
        /// A <see cref="Task"/> which represents the asynchronous operation.
        /// </returns>
        Task<int> CopyFileFromPodAsync(V1Pod pod, string container, string sourceFilePath, string destinationFilePath, CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// Copy a file from a pod's container into the local machine.
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
        /// <param name="sourceFilePath">
        /// The source file path.
        /// </param>
        /// <param name="destinationFilePath">
        /// The destination file path.
        /// </param>
        /// <param name="cancellationToken">
        /// A <see cref="CancellationToken"/> which can be used to cancel the asynchronous operation.
        /// </param>
        /// <returns>
        /// A <see cref="Task"/> which represents the asynchronous operation.
        /// </returns>
        Task<int> CopyFileFromPodAsync(string name, string @namespace, string container, string sourceFilePath, string destinationFilePath, CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// Copy a file from the local machine into a pod's container.
        /// </summary>
        /// <param name="pod">
        /// The pod object which contains the container in which the file will be copy.
        /// </param>
        /// <param name="container">
        /// The container in which the file will be copy.
        /// </param>
        /// <param name="sourceFilePath">
        /// The source file path.
        /// </param>
        /// <param name="destinationFilePath">
        /// The destination file path.
        /// </param>
        /// <param name="cancellationToken">
        /// A <see cref="CancellationToken"/> which can be used to cancel the asynchronous operation.
        /// </param>
        /// <returns>
        /// A <see cref="Task"/> which represents the asynchronous operation.
        /// </returns>
        Task<int> CopyFileToPodAsync(V1Pod pod, string container, string sourceFilePath, string destinationFilePath, CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// Copy a file from the local machine into a pod's container.
        /// </summary>
        /// <param name="name">
        /// The name of the pod which contains the container in which the file will be copy.
        /// </param>
        /// <param name="namespace">
        /// The namespace of the container.
        /// </param>
        /// <param name="container">
        /// The container in which the file will be copy.
        /// </param>
        /// <param name="sourceFilePath">
        /// The source file path.
        /// </param>
        /// <param name="destinationFilePath">
        /// The destination file path.
        /// </param>
        /// <param name="cancellationToken">
        /// A <see cref="CancellationToken"/> which can be used to cancel the asynchronous operation.
        /// </param>
        /// <returns>
        /// A <see cref="Task"/> which represents the asynchronous operation.
        /// </returns>
        Task<int> CopyFileToPodAsync(string name, string @namespace, string container, string sourceFilePath, string destinationFilePath, CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// Copy a directory from a pod's container into the local machine.
        /// </summary>
        /// <param name="pod">
        /// The pod object which contains the container in which the source directory exist.
        /// </param>
        /// <param name="container">
        /// The container in which the source directory exist.
        /// </param>
        /// <param name="sourceFolderPath">
        /// The source directory path.
        /// </param>
        /// <param name="destinationFolderPath">
        /// The destination directory path.
        /// </param>
        /// <param name="cancellationToken">
        /// A <see cref="CancellationToken"/> which can be used to cancel the asynchronous operation.
        /// </param>
        /// <returns>
        /// A <see cref="Task"/> which represents the asynchronous operation.
        /// </returns>
        Task<int> CopyDirectoryFromPodAsync(V1Pod pod, string container, string sourceFolderPath, string destinationFolderPath, CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// Copy a directory from a pod's container into the local machine.
        /// </summary>
        /// <param name="name">
        /// The name of the pod which contains the container in which the source directory exist.
        /// </param>
        /// <param name="namespace">
        /// The namespace of the container.
        /// </param>
        /// <param name="container">
        /// The container in which the source directory exist.
        /// </param>
        /// <param name="sourceFolderPath">
        /// The source directory path.
        /// </param>
        /// <param name="destinationFolderPath">
        /// The destination directory path.
        /// </param>
        /// <param name="cancellationToken">
        /// A <see cref="CancellationToken"/> which can be used to cancel the asynchronous operation.
        /// </param>
        /// <returns>
        /// A <see cref="Task"/> which represents the asynchronous operation.
        /// </returns>
        Task<int> CopyDirectoryFromPodAsync(string name, string @namespace, string container, string sourceFolderPath, string destinationFolderPath, CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// Copy a directory from the local machine into a pod's container.
        /// </summary>
        /// <param name="pod">
        /// The pod object which contains the container in which the directory will be copy.
        /// </param>
        /// <param name="container">
        /// The container in which the directory will be copy.
        /// </param>
        /// <param name="sourceFolderPath">
        /// The source directory path.
        /// </param>
        /// <param name="destinationFolderPath">
        /// The destination directory path.
        /// </param>
        /// <param name="cancellationToken">
        /// A <see cref="CancellationToken"/> which can be used to cancel the asynchronous operation.
        /// </param>
        /// <returns>
        /// A <see cref="Task"/> which represents the asynchronous operation.
        /// </returns>
        Task<int> CopyDirectoryToPodAsync(V1Pod pod, string container, string sourceFolderPath, string destinationFolderPath, CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// Copy a directory from the local machine into a pod's container.
        /// </summary>
        /// <param name="name">
        /// The name of the pod which contains the container in which the directory will be copy.
        /// </param>
        /// <param name="namespace">
        /// The namespace of the container.
        /// </param>
        /// <param name="container">
        /// The container in which the directory will be copy.
        /// </param>
        /// <param name="sourceFolderPath">
        /// The source directory path.
        /// </param>
        /// <param name="destinationFolderPath">
        /// The destination directory path.
        /// </param>
        /// <param name="cancellationToken">
        /// A <see cref="CancellationToken"/> which can be used to cancel the asynchronous operation.
        /// </param>
        /// <returns>
        /// A <see cref="Task"/> which represents the asynchronous operation.
        /// </returns>
        Task<int> CopyDirectoryToPodAsync(string name, string @namespace, string container, string sourceFolderPath, string destinationFolderPath, CancellationToken cancellationToken = default(CancellationToken));
    }
}
