using k8s.Models;
using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace k8s
{
    public partial class Kubernetes
    {
        public async Task CopyFileFromPod(V1Pod pod, string container, string sourcePath, string destinationPath, CancellationToken cancellationToken = default(CancellationToken))
        {
            await CopyFileFromPod(pod.Metadata.Name, pod.Metadata.NamespaceProperty, container, sourcePath, destinationPath, cancellationToken).ConfigureAwait(false);
        }

        public async Task CopyFileFromPod(string name, string @namespace, string container, string sourceFilePath, string destinationFilePath, CancellationToken cancellationToken = default(CancellationToken))
        {
            var handler = GetCopyFileFromPodHandler(destinationFilePath);

            // All other parameters are being validated by MuxedStreamNamespacedPodExecAsync called by NamespacedPodExecAsync
            if (string.IsNullOrWhiteSpace(sourceFilePath))
            {
                throw new ArgumentException($"{nameof(sourceFilePath)} cannot be null or whitespace");
            }
            if (string.IsNullOrWhiteSpace(destinationFilePath))
            {
                throw new ArgumentException($"{nameof(destinationFilePath)} cannot be null or whitespace");
            }

            await NamespacedPodExecAsync(name, @namespace, container, new string[] { "sh", "-c", $"cat {sourceFilePath} | base64" }, false, handler, cancellationToken).ConfigureAwait(false);
        }

        private ExecAsyncCallback GetCopyFileFromPodHandler(string destinationFilePath)
        {
            return new ExecAsyncCallback((stdIn, stdOut, stdError) =>
            {
                StreamReader errorReader = new StreamReader(stdError);

                if (errorReader.Peek() != -1)
                {
                    var error = errorReader.ReadToEnd();
                    throw new IOException($"Copy command failed: {error}");
                }

                StreamReader outReader = new StreamReader(stdOut);
                var encodedData = outReader.ReadToEnd();

                using (FileStream output = new FileStream(destinationFilePath, FileMode.Create))
                {
                    var data = Convert.FromBase64String(encodedData);
                    output.Write(data, 0, data.Length);
                }

#if NET452
                return Task.FromResult(0);
#else
                return Task.CompletedTask;
#endif
            });
        }
    }
}
