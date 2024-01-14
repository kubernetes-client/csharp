using ICSharpCode.SharpZipLib.Tar;
using k8s.Autorest;
using k8s.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace k8s.E2E
{
    [Collection(nameof(Onebyone))]
    public class MinikubeTests
    {
        [MinikubeFact]
        public void SimpleTest()
        {
            var namespaceParameter = "default";
            var podName = "k8scsharp-e2e-pod";

            using var client = CreateClient();

            void Cleanup()
            {
                var pods = client.CoreV1.ListNamespacedPod(namespaceParameter);
                while (pods.Items.Any(p => p.Metadata.Name == podName))
                {
                    try
                    {
                        client.CoreV1.DeleteNamespacedPod(podName, namespaceParameter);
                    }
                    catch (HttpOperationException e)
                    {
                        if (e.Response.StatusCode == System.Net.HttpStatusCode.NotFound)
                        {
                            return;
                        }
                    }
                }
            }

            try
            {
                Cleanup();

                client.CoreV1.CreateNamespacedPod(
                    new V1Pod()
                    {
                        Metadata = new V1ObjectMeta { Name = podName, },
                        Spec = new V1PodSpec
                        {
                            Containers = new[] { new V1Container() { Name = "k8scsharp-e2e", Image = "nginx", }, },
                        },
                    },
                    namespaceParameter);

                var pods = client.CoreV1.ListNamespacedPod(namespaceParameter);
                Assert.Contains(pods.Items, p => p.Metadata.Name == podName);
            }
            finally
            {
                Cleanup();
            }
        }

        [MinikubeFact]
        public async Task LogStreamTestAsync()
        {
            var namespaceParameter = "default";
            var podName = "k8scsharp-e2e-logstream-pod";

            using var client = CreateClient();

            void Cleanup()
            {
                var pods = client.CoreV1.ListNamespacedPod(namespaceParameter);
                while (pods.Items.Any(p => p.Metadata.Name == podName))
                {
                    try
                    {
                        client.CoreV1.DeleteNamespacedPod(podName, namespaceParameter);
                    }
                    catch (HttpOperationException e)
                    {
                        if (e.Response.StatusCode == System.Net.HttpStatusCode.NotFound)
                        {
                            return;
                        }
                    }
                }
            }

            try
            {
                Cleanup();

                client.CoreV1.CreateNamespacedPod(
                    new V1Pod()
                    {
                        Metadata = new V1ObjectMeta { Name = podName, },
                        Spec = new V1PodSpec
                        {
                            Containers = new[]
                            {
                                new V1Container()
                                {
                                    Name = "k8scsharp-e2e-logstream",
                                    Image = "busybox",
                                    Command = new[] { "ping" },
                                    Args = new[] { "-i", "10",  "127.0.0.1" },
                                },
                            },
                        },
                    },
                    namespaceParameter);

                var lines = new List<string>();
                var started = new ManualResetEvent(false);

                async Task<V1Pod> Pod()
                {
                    var pods = client.CoreV1.ListNamespacedPod(namespaceParameter);
                    var pod = pods.Items.First(p => p.Metadata.Name == podName);
                    while (pod.Status.Phase != "Running")
                    {
                        await Task.Delay(TimeSpan.FromSeconds(1)).ConfigureAwait(false);
                        return await Pod().ConfigureAwait(false);
                    }

                    return pod;
                }

                var pod = await Pod().ConfigureAwait(false);
                var stream = client.CoreV1.ReadNamespacedPodLog(pod.Metadata.Name, pod.Metadata.NamespaceProperty, follow: true);
                using var reader = new StreamReader(stream);

                var copytask = Task.Run(() =>
                {
                    for (; ; )
                    {
                        try
                        {
                            lines.Add(reader.ReadLine());
                        }
                        finally
                        {
                            started.Set();
                        }
                    }
                });

                Assert.True(started.WaitOne(TimeSpan.FromMinutes(2)));
                await Task.Delay(TimeSpan.FromSeconds(1)).ConfigureAwait(false);
                Assert.Null(copytask.Exception);
                Assert.Equal(2, lines.Count);
                await Task.Delay(TimeSpan.FromSeconds(11)).ConfigureAwait(false);
                Assert.Equal(3, lines.Count);
            }
            finally
            {
                Cleanup();
            }
        }

        [MinikubeFact]
        public async Task DatetimeFieldTest()
        {
            using var kubernetes = CreateClient();

            await kubernetes.CoreV1.CreateNamespacedEventAsync(
                new Corev1Event(
                    new V1ObjectReference(
                        "v1alpha1",
                        kind: "Test",
                        name: "test",
                        namespaceProperty: "default",
                        resourceVersion: "1",
                        uid: "1"),
                    new V1ObjectMeta()
                    {
                        GenerateName = "started-",
                    },
                    action: "STARTED",
                    type: "Normal",
                    reason: "STARTED",
                    message: "Started",
                    eventTime: DateTime.Now,
                    firstTimestamp: DateTime.Now,
                    lastTimestamp: DateTime.Now,
                    reportingComponent: "37",
                    reportingInstance: "38"), "default").ConfigureAwait(false);
        }

        [MinikubeFact]
        public async Task CopyToPodTestAsync()
        {
            var namespaceParameter = "default";
            var podName = "k8scsharp-e2e-cp-pod";

            using var client = CreateClient();

            async Task<int> CopyFileToPodAsync(string name, string @namespace, string container, Stream inputFileStream, string destinationFilePath, CancellationToken cancellationToken = default(CancellationToken))
            {
                // The callback which processes the standard input, standard output and standard error of exec method
                var handler = new ExecAsyncCallback(async (stdIn, stdOut, stdError) =>
                {
                    var fileInfo = new FileInfo(destinationFilePath);
                    try
                    {
                        using (var memoryStream = new MemoryStream())
                        {
                            using (var tarOutputStream = new TarOutputStream(memoryStream, Encoding.Default))
                            {
                                tarOutputStream.IsStreamOwner = false;

                                var fileSize = inputFileStream.Length;
                                var entry = TarEntry.CreateTarEntry(fileInfo.Name);

                                entry.Size = fileSize;

                                tarOutputStream.PutNextEntry(entry);
                                await inputFileStream.CopyToAsync(tarOutputStream).ConfigureAwait(false);
                                tarOutputStream.CloseEntry();
                            }

                            memoryStream.Position = 0;

                            await memoryStream.CopyToAsync(stdIn).ConfigureAwait(false);
                            await memoryStream.FlushAsync().ConfigureAwait(false);
                            stdIn.Close();
                        }
                    }
                    catch (Exception ex)
                    {
                        throw new IOException($"Copy command failed: {ex.Message}");
                    }

                    using StreamReader streamReader = new StreamReader(stdError);
                    while (streamReader.EndOfStream == false)
                    {
                        string error = await streamReader.ReadToEndAsync().ConfigureAwait(false);
                        throw new IOException($"Copy command failed: {error}");
                    }
                });

                string destinationFolder = Path.GetDirectoryName(destinationFilePath).Replace("\\", "/");

                return await client.NamespacedPodExecAsync(
                    name,
                    @namespace,
                    container,
                    new string[] { "tar", "-xmf", "-", "-C", destinationFolder },
                    false,
                    handler,
                    cancellationToken).ConfigureAwait(false);
            }


            void Cleanup()
            {
                var pods = client.CoreV1.ListNamespacedPod(namespaceParameter);
                while (pods.Items.Any(p => p.Metadata.Name == podName))
                {
                    try
                    {
                        client.CoreV1.DeleteNamespacedPod(podName, namespaceParameter);
                    }
                    catch (HttpOperationException e)
                    {
                        if (e.Response.StatusCode == System.Net.HttpStatusCode.NotFound)
                        {
                            return;
                        }
                    }
                }
            }

            try
            {
                Cleanup();

                client.CoreV1.CreateNamespacedPod(
                    new V1Pod()
                    {
                        Metadata = new V1ObjectMeta { Name = podName, },
                        Spec = new V1PodSpec
                        {
                            Containers = new[]
                            {
                                new V1Container()
                                {
                                    Name = "container",
                                    Image = "ubuntu",
                                    // Image = "busybox", // TODO not work with busybox
                                    Command = new[] { "sleep" },
                                    Args = new[] { "infinity" },
                                },
                            },
                        },
                    },
                    namespaceParameter);

                var lines = new List<string>();
                var started = new ManualResetEvent(false);

                async Task<V1Pod> Pod()
                {
                    var pods = client.CoreV1.ListNamespacedPod(namespaceParameter);
                    var pod = pods.Items.First(p => p.Metadata.Name == podName);
                    while (pod.Status.Phase != "Running")
                    {
                        await Task.Delay(TimeSpan.FromSeconds(1)).ConfigureAwait(false);
                        return await Pod().ConfigureAwait(false);
                    }

                    return pod;
                }

                var pod = await Pod().ConfigureAwait(false);


                async Task AssertMd5sumAsync(string file, byte[] orig)
                {
                    var ws = await client.WebSocketNamespacedPodExecAsync(
                       pod.Metadata.Name,
                       pod.Metadata.NamespaceProperty,
                       new string[] { "md5sum", file },
                       "container").ConfigureAwait(false);

                    var demux = new StreamDemuxer(ws);
                    demux.Start();

                    var buff = new byte[4096];
                    var stream = demux.GetStream(1, 1);
                    var read = stream.Read(buff, 0, 4096);
                    var remotemd5 = Encoding.Default.GetString(buff);
                    remotemd5 = remotemd5.Substring(0, 32);

                    var md5 = MD5.Create().ComputeHash(orig);
                    var localmd5 = BitConverter.ToString(md5).Replace("-", string.Empty).ToLower();

                    Assert.Equal(localmd5, remotemd5);
                }


                //
                {
                    // small
                    var content = new byte[1 * 1024 * 1024];
                    new Random().NextBytes(content);
                    await CopyFileToPodAsync(pod.Metadata.Name, pod.Metadata.NamespaceProperty, "container", new MemoryStream(content), "/tmp/test").ConfigureAwait(false);
                    await AssertMd5sumAsync("/tmp/test", content).ConfigureAwait(false);
                }

                {
                    // big
                    var content = new byte[40 * 1024 * 1024];
                    new Random().NextBytes(content);
                    await CopyFileToPodAsync(pod.Metadata.Name, pod.Metadata.NamespaceProperty, "container", new MemoryStream(content), "/tmp/test").ConfigureAwait(false);
                    await AssertMd5sumAsync("/tmp/test", content).ConfigureAwait(false);
                }
            }
            finally
            {
                Cleanup();
            }
        }

        public static IKubernetes CreateClient()
        {
            return new Kubernetes(KubernetesClientConfiguration.BuildDefaultConfig());
        }
    }
}
