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
        public async Task<int> CopyFileFromPodAsync(V1Pod pod, string container, string sourcePath, string destinationPath, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (pod == null)
            {
                throw new ArgumentNullException(nameof(pod));
            }

            return await CopyFileFromPodAsync(pod.Metadata.Name, pod.Metadata.NamespaceProperty, container, sourcePath, destinationPath, cancellationToken).ConfigureAwait(false);
        }

        public async Task<int> CopyFileFromPodAsync(string name, string @namespace, string container, string sourceFilePath, string destinationFilePath, CancellationToken cancellationToken = default(CancellationToken))
        {
            // All other parameters are being validated by MuxedStreamNamespacedPodExecAsync called by NamespacedPodExecAsync
            if (string.IsNullOrWhiteSpace(sourceFilePath))
            {
                throw new ArgumentException($"{nameof(sourceFilePath)} cannot be null or whitespace");
            }

            if (string.IsNullOrWhiteSpace(destinationFilePath))
            {
                throw new ArgumentException($"{nameof(destinationFilePath)} cannot be null or whitespace");
            }

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

            return await NamespacedPodExecAsync(name, @namespace, container, new string[] { "sh", "-c", $"tar czf - -C {sourceFolder} {sourceFileInfo.Name} | base64" }, false, handler, cancellationToken).ConfigureAwait(false);
        }

        public async Task<int> CopyFileToPodAsync(V1Pod pod, string container, string sourcePath, string destinationPath, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (pod == null)
            {
                throw new ArgumentNullException(nameof(pod));
            }

            return await CopyFileToPodAsync(pod.Metadata.Name, pod.Metadata.NamespaceProperty, container, sourcePath, destinationPath, cancellationToken).ConfigureAwait(false);
        }

        public async Task<int> CopyFileToPodAsync(string name, string @namespace, string container, string sourceFilePath, string destinationFilePath, CancellationToken cancellationToken = default(CancellationToken))
        {
            // All other parameters are being validated by MuxedStreamNamespacedPodExecAsync called by NamespacedPodExecAsync
            if (string.IsNullOrWhiteSpace(sourceFilePath))
            {
                throw new ArgumentException($"{nameof(sourceFilePath)} cannot be null or whitespace");
            }

            if (string.IsNullOrWhiteSpace(destinationFilePath))
            {
                throw new ArgumentException($"{nameof(destinationFilePath)} cannot be null or whitespace");
            }

            // The callback which processes the standard input, standard output and standard error of exec method
            var handler = new ExecAsyncCallback(async (stdIn, stdOut, stdError) =>
            {
                var fileInfo = new FileInfo(destinationFilePath);

                try
                {
                    using (var outputStream = new MemoryStream())
                    {
                        using (var inputFileStream = File.OpenRead(sourceFilePath))
                        using (var gZipOutputStream = new GZipOutputStream(outputStream))
                        using (var tarOutputStream = new TarOutputStream(gZipOutputStream))
                        {
                            // To avoid gZipOutputStream to close the memoryStream
                            gZipOutputStream.IsStreamOwner = false;

                            var fileSize = inputFileStream.Length;
                            var entry = TarEntry.CreateTarEntry(fileInfo.Name);
                            entry.Size = fileSize;

                            tarOutputStream.PutNextEntry(entry);

                            // this is copied from TarArchive.WriteEntryCore
                            byte[] localBuffer = new byte[32 * 1024];
                            while (true)
                            {
                                int numRead = inputFileStream.Read(localBuffer, 0, localBuffer.Length);
                                if (numRead <= 0)
                                {
                                    break;
                                }

                                tarOutputStream.Write(localBuffer, 0, numRead);
                            }

                            tarOutputStream.CloseEntry();
                        }

                        outputStream.Position = 0;
                        using (var cryptoStream = new CryptoStream(stdIn, new ToBase64Transform(), CryptoStreamMode.Write))
                        {
                            outputStream.CopyTo(cryptoStream);
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

            var destinationFolder = GetFolderName(destinationFilePath);

            return await NamespacedPodExecAsync(
                name,
                @namespace,
                container,
                new string[] { "sh", "-c", $"base64 -d | tar xzmf - -C {destinationFolder}" },
                false,
                handler,
                cancellationToken).ConfigureAwait(false);
        }

        private string GetFolderName(string filePath)
        {
            var folderName = Path.GetDirectoryName(filePath);

            return string.IsNullOrEmpty(folderName) ? "." : folderName;
        }
    }
}
