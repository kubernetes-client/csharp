using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Security.Cryptography;

namespace k8s
{
    public partial class Kubernetes
    {
        public async Task<int> CopyFileFromPodAsync(string name, string @namespace, string container, string srcPath, string destPath, CancellationToken cancellationToken)
        {
            var exitCode = await NamespacedPodExecAsync
            (
                name,
                @namespace,
                container,
                new[]{ "sh", "-c", $"cat {srcPath} | base64" },
                true,
                async (_, stdout, _) =>
                {
                    // https://stackoverflow.com/questions/12901705/decoding-base64-stream-to-image
                    using var stream = new CryptoStream(stdout, new FromBase64Transform(), CryptoStreamMode.Read);
                    using var fs = new FileStream(destPath, FileMode.Create);
                    await stream.CopyToAsync(fs, cancellationToken).ConfigureAwait(false);
                    
                    await fs.FlushAsync(cancellationToken).ConfigureAwait(false);
                },
                cancellationToken
            ).ConfigureAwait(false);
            return exitCode;
        }

        public async Task<int> CopyFileToPodAsync(string name, string @namespace, string container, string srcPath, string destPath, CancellationToken cancellationToken)
        {
            var fileBytes = await File.ReadAllBytesAsync(srcPath, cancellationToken).ConfigureAwait(false);
            var base64String = Convert.ToBase64String(fileBytes);
            var exitCode = await NamespacedPodExecAsync
            (
                name,
                @namespace,
                container,
                new[]{ "sh", "-c", $"echo {base64String} | base64 --decode > {destPath}" },
                true,
                (_, _, _) => Task.CompletedTask,
                cancellationToken
            ).ConfigureAwait(false);
            return exitCode;
        }
    }
}
