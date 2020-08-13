using System;
using System.IO;
using System.Security.Cryptography;
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

            return await NamespacedPodExecAsync(name, @namespace, container, new string[] { "sh", "-c", $"tar cz {sourceFilePath} | base64" }, false, handler, cancellationToken).ConfigureAwait(false);
        }
    }
}
