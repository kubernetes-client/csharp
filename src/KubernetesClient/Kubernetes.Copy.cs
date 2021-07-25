using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ICSharpCode.SharpZipLib.GZip;
using ICSharpCode.SharpZipLib.Tar;
using k8s.Models;

namespace k8s
{
    public partial class Kubernetes
    {
        /*******************************************************************
        ** /!\ Requires that the 'tar' binary is present in your container
        ** image. If 'tar' is not present, the copy will fail. /!\
        *******************************************************************/
        public async Task<int> CopyFileFromPodAsync(V1Pod pod, string container, string sourceFilePath, string destinationFilePath, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (pod == null)
            {
                throw new ArgumentNullException(nameof(pod));
            }

            return await CopyFileFromPodAsync(pod.Metadata.Name, pod.Metadata.NamespaceProperty, container, sourceFilePath, destinationFilePath, cancellationToken).ConfigureAwait(false);
        }

        public async Task<int> CopyFileFromPodAsync(string name, string @namespace, string container, string sourceFilePath, string destinationFilePath, CancellationToken cancellationToken = default(CancellationToken))
        {
            // All other parameters are being validated by MuxedStreamNamespacedPodExecAsync called by NamespacedPodExecAsync
            ValidatePathParameters(sourceFilePath, destinationFilePath);

            // The callback which processes the standard input, standard output and standard error of exec method
            var handler = new ExecAsyncCallback(async (stdIn, stdOut, stdError) =>
            {
                using (var errorReader = new StreamReader(stdError))
                {
                    if (errorReader.Peek() != -1)
                    {
                        var error = await errorReader.ReadToEndAsync().ConfigureAwait(false);
                        throw new IOException($"Copy command failed: {error}");
                    }
                }

                try
                {
                    using (var stream = new CryptoStream(stdOut, new FromBase64Transform(), CryptoStreamMode.Read))
                    using (var gzipStream = new GZipInputStream(stream))
                    using (var tarInputStream = new TarInputStream(gzipStream))
                    {
                        var tarEntry = tarInputStream.GetNextEntry();
                        var directoryName = Path.GetDirectoryName(destinationFilePath);

                        if (!string.IsNullOrEmpty(directoryName))
                        {
                            Directory.CreateDirectory(directoryName);
                        }

                        using (var outputFile = new FileStream(destinationFilePath, FileMode.Create))
                        {
                            tarInputStream.CopyEntryContents(outputFile);
                        }
                    }
                }
                catch (Exception ex)
                {
                    throw new IOException($"Copy command failed: {ex.Message}");
                }
            });

            var sourceFileInfo = new FileInfo(sourceFilePath);
            var sourceFolder = GetFolderName(sourceFilePath);

            return await NamespacedPodExecAsync(
                name,
                @namespace,
                container,
                new string[] { "sh", "-c", $"tar czf - -C {sourceFolder} {sourceFileInfo.Name} | base64" },
                false,
                handler,
                cancellationToken).ConfigureAwait(false);
        }

        public async Task<int> CopyFileToPodAsync(V1Pod pod, string container, string sourceFilePath, string destinationFilePath, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (pod == null)
            {
                throw new ArgumentNullException(nameof(pod));
            }

            return await CopyFileToPodAsync(pod.Metadata.Name, pod.Metadata.NamespaceProperty, container, sourceFilePath, destinationFilePath, cancellationToken).ConfigureAwait(false);
        }

        public async Task<int> CopyFileToPodAsync(string name, string @namespace, string container, string sourceFilePath, string destinationFilePath, CancellationToken cancellationToken = default(CancellationToken))
        {
            // All other parameters are being validated by MuxedStreamNamespacedPodExecAsync called by NamespacedPodExecAsync
            ValidatePathParameters(sourceFilePath, destinationFilePath);

            string destinationFolder = GetFolderName(destinationFilePath);

            // The callback which processes the standard input, standard output and standard error of exec method
            var handler = new ExecAsyncCallback(async (stdIn, stdOut, stdError) =>
            {
                var fileInfo = new FileInfo(destinationFilePath);

                try
                {
                    await CompressTo(sourceFilePath, stdIn);
                    stdIn.Close();
                }
                catch (Exception ex)
                {
                    throw new IOException($"Copy command failed: {ex.Message}");
                }
            });

            return await NamespacedPodExecAsync(
                name,
                @namespace,
                container,
                new string[] { "tar", "xzmf", "-", "-C", destinationFolder },
                true,
                handler,
                cancellationToken).ConfigureAwait(false);
        }

        public async Task<int> CopyDirectoryFromPodAsync(V1Pod pod, string container, string sourceFolderPath, string destinationFolderPath, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (pod == null)
            {
                throw new ArgumentNullException(nameof(pod));
            }

            return await CopyDirectoryFromPodAsync(pod.Metadata.Name, pod.Metadata.NamespaceProperty, container, sourceFolderPath, destinationFolderPath, cancellationToken).ConfigureAwait(false);

        }

        public async Task<int> CopyDirectoryFromPodAsync(string name, string @namespace, string container, string sourceFolderPath, string destinationFolderPath, CancellationToken cancellationToken = default(CancellationToken))
        {
            // All other parameters are being validated by MuxedStreamNamespacedPodExecAsync called by NamespacedPodExecAsync
            ValidatePathParameters(sourceFolderPath, destinationFolderPath);

            // The callback which processes the standard input, standard output and standard error of exec method
            var handler = new ExecAsyncCallback(async (stdIn, stdOut, stdError) =>
            {
                using (var errorReader = new StreamReader(stdError))
                {
                    if (errorReader.Peek() != -1)
                    {
                        var error = await errorReader.ReadToEndAsync().ConfigureAwait(false);
                        throw new IOException($"Copy command failed: {error}");
                    }
                }

                try
                {
                    using (var stream = new CryptoStream(stdOut, new FromBase64Transform(), CryptoStreamMode.Read))
                    using (var gzipStream = new GZipInputStream(stream))
                    using (var tarArchive = TarArchive.CreateInputTarArchive(gzipStream))
                    {
                        tarArchive.ExtractContents(destinationFolderPath);
                    }
                }
                catch (Exception ex)
                {
                    throw new IOException($"Copy command failed: {ex.Message}");
                }
            });

            return await NamespacedPodExecAsync(
                name,
                @namespace,
                container,
                new string[] { "sh", "-c", $"tar czf - -C {sourceFolderPath} . | base64" },
                false,
                handler,
                cancellationToken).ConfigureAwait(false);
        }

        public async Task<int> CopyDirectoryToPodAsync(V1Pod pod, string container, string sourceDirectoryPath, string destinationDirectoyPath, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (pod == null)
            {
                throw new ArgumentNullException(nameof(pod));
            }

            return await CopyFileToPodAsync(pod.Metadata.Name, pod.Metadata.NamespaceProperty, container, sourceDirectoryPath, destinationDirectoyPath, cancellationToken).ConfigureAwait(false);
        }

        public async Task<int> CopyDirectoryToPodAsync(string name, string @namespace, string container, string sourceDirectoryPath, string destinationDirectoryPath, CancellationToken cancellationToken = default(CancellationToken))
        {
            // All other parameters are being validated by MuxedStreamNamespacedPodExecAsync called by NamespacedPodExecAsync
            ValidatePathParameters(sourceDirectoryPath, destinationDirectoryPath);

            // The callback which processes the standard input, standard output and standard error of exec method
            var handler = new ExecAsyncCallback(async (stdIn, stdOut, stdError) =>
            {
                try
                {
                    using (var outputStream = new MemoryStream())
                    {
                        using (var gZipOutputStream = new GZipOutputStream(outputStream))
                        using (var tarArchive = TarArchive.CreateOutputTarArchive(gZipOutputStream))
                        {
                            // To avoid gZipOutputStream to close the memoryStream
                            gZipOutputStream.IsStreamOwner = false;

                            // RootPath must be forward slashes and must not end with a slash
                            tarArchive.RootPath = sourceDirectoryPath.Replace('\\', '/');
                            if (tarArchive.RootPath.EndsWith("/", StringComparison.InvariantCulture))
                            {
                                tarArchive.RootPath = tarArchive.RootPath.Remove(tarArchive.RootPath.Length - 1);
                            }

                            AddDirectoryFilesToTar(tarArchive, sourceDirectoryPath);
                        }

                        outputStream.Position = 0;
                        using (var cryptoStream = new CryptoStream(stdIn, new ToBase64Transform(), CryptoStreamMode.Write))
                        {
                            await outputStream.CopyToAsync(cryptoStream).ConfigureAwait(false);
                        }
                    }
                }
                catch (Exception ex)
                {
                    throw new IOException($"Copy command failed: {ex.Message}");
                }

                using (var errorReader = new StreamReader(stdError))
                {
                    if (errorReader.Peek() != -1)
                    {
                        var error = await errorReader.ReadToEndAsync().ConfigureAwait(false);
                        throw new IOException($"Copy command failed: {error}");
                    }
                }
            });

            return await NamespacedPodExecAsync(
                name,
                @namespace,
                container,
                new string[] { "sh", "-c", $"base64 -d | tar xzmf - -C {destinationDirectoryPath}" },
                false,
                handler,
                cancellationToken).ConfigureAwait(false);
        }

        /// <summary>
        /// Compress a file to the stream (use .tgz format).
        /// </summary>
        /// <param name="source">Source file to be compressed.</param>
        /// <param name="dest">Destination stream.</param>
        private static async Task CompressTo(string source, Stream dest)
        {
            using (Stream gzStream = new GZipOutputStream(dest))
            using (TarOutputStream tarStream = new TarOutputStream(gzStream, Encoding.UTF8))
            using (FileStream inStream = File.OpenRead(source))
            {
                TarEntry entry = TarEntry.CreateTarEntry(Path.GetFileName(source));
                entry.Size = inStream.Length;
                tarStream.PutNextEntry(entry);
                await inStream.CopyToAsync(tarStream);
                tarStream.CloseEntry();
            }
        }

        private void AddDirectoryFilesToTar(TarArchive tarArchive, string sourceDirectoryPath)
        {
            var tarEntry = TarEntry.CreateEntryFromFile(sourceDirectoryPath);
            tarArchive.WriteEntry(tarEntry, false);

            var filenames = Directory.GetFiles(sourceDirectoryPath);
            for (var i = 0; i < filenames.Length; i++)
            {
                tarEntry = TarEntry.CreateEntryFromFile(filenames[i]);
                tarArchive.WriteEntry(tarEntry, true);
            }

            var directories = Directory.GetDirectories(sourceDirectoryPath);
            for (var i = 0; i < directories.Length; i++)
            {
                AddDirectoryFilesToTar(tarArchive, directories[i]);
            }
        }

        private string GetFolderName(string filePath)
        {
            var folderName = Path.GetDirectoryName(filePath);

            return string.IsNullOrEmpty(folderName) ? "." : folderName;
        }

        private void ValidatePathParameters(string sourcePath, string destinationPath)
        {
            if (string.IsNullOrWhiteSpace(sourcePath))
            {
                throw new ArgumentException($"{nameof(sourcePath)} cannot be null or whitespace");
            }

            if (string.IsNullOrWhiteSpace(destinationPath))
            {
                throw new ArgumentException($"{nameof(destinationPath)} cannot be null or whitespace");
            }

        }
    }
}
