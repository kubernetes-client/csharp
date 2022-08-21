using System;
using System.IO;
using System.Threading.Tasks;
using k8s;
using ICSharpCode.SharpZipLib.Tar;
using System.Threading;
using System.Linq;
using System.Text;

namespace cp
{
    internal class Cp
    {
        private static IKubernetes client;

        private static async Task Main(string[] args)
        {
            var config = KubernetesClientConfiguration.BuildConfigFromConfigFile();
            client = new Kubernetes(config);


            var pods = client.CoreV1.ListNamespacedPod("default", null, null, null, $"job-name=upload-demo");
            var pod = pods.Items.First();

            await CopyFileToPodAsync(pod.Metadata.Name, "default", "upload-demo", args[0], $"home/{args[1]}");

        }




        private static void ValidatePathParameters(string sourcePath, string destinationPath)
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

        public static async Task<int> CopyFileToPodAsync(string name, string @namespace, string container, string sourceFilePath, string destinationFilePath, CancellationToken cancellationToken = default(CancellationToken))
        {
            // All other parameters are being validated by MuxedStreamNamespacedPodExecAsync called by NamespacedPodExecAsync
            ValidatePathParameters(sourceFilePath, destinationFilePath);

            // The callback which processes the standard input, standard output and standard error of exec method
            var handler = new ExecAsyncCallback(async (stdIn, stdOut, stdError) =>
            {
                var fileInfo = new FileInfo(destinationFilePath);
                try
                {
                    using (var memoryStream = new MemoryStream())
                    {
                        using (var inputFileStream = File.OpenRead(sourceFilePath))
                        using (var tarOutputStream = new TarOutputStream(memoryStream, Encoding.Default)) 
                        {
                            tarOutputStream.IsStreamOwner = false;

                            var fileSize = inputFileStream.Length;
                            var entry = TarEntry.CreateTarEntry(fileInfo.Name);

                            entry.Size = fileSize;

                            tarOutputStream.PutNextEntry(entry);
                            await inputFileStream.CopyToAsync(tarOutputStream);
                            tarOutputStream.CloseEntry();
                        }

                        memoryStream.Position = 0;

                        await memoryStream.CopyToAsync(stdIn);
                        await stdIn.FlushAsync();
                    }

                }
                catch (Exception ex)
                {
                    throw new IOException($"Copy command failed: {ex.Message}");
                }

                using StreamReader streamReader = new StreamReader(stdError);
                while (streamReader.EndOfStream == false)
                {
                    string error = await streamReader.ReadToEndAsync();
                    throw new IOException($"Copy command failed: {error}");
                }
            });

            string destinationFolder = GetFolderName(destinationFilePath);

            return await client.NamespacedPodExecAsync(
                name,
                @namespace,
                container,
                new string[] { "sh", "-c", $"tar xmf - -C {destinationFolder}" },
                false,
                handler,
                cancellationToken);
        }


        private static string GetFolderName(string filePath)
        {
            var folderName = Path.GetDirectoryName(filePath);

            return string.IsNullOrEmpty(folderName) ? "." : folderName;
        }


    }
}
