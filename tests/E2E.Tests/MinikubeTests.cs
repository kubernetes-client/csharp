using ICSharpCode.SharpZipLib.Tar;
using Json.Patch;
using k8s.Autorest;
using k8s.LeaderElection;
using k8s.LeaderElection.ResourceLock;
using k8s.Models;
using Nito.AsyncEx;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace k8s.E2E
{
    public class MinikubeTests
    {
        [MinikubeFact]
        public void SimpleTest()
        {
            var namespaceParameter = "default";
            var podName = "k8scsharp-e2e-pod";

            var client = CreateClient();

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
        public void PatchTest()
        {
            var namespaceParameter = "default";
            var podName = "k8scsharp-e2e-patch-pod";

            var client = CreateClient();

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
                {
                    Cleanup();

                    client.CoreV1.CreateNamespacedPod(
                        new V1Pod()
                        {
                            Metadata = new V1ObjectMeta { Name = podName, Labels = new Dictionary<string, string> { { "place", "holder" }, }, },
                            Spec = new V1PodSpec
                            {
                                Containers = new[] { new V1Container() { Name = "k8scsharp-patch", Image = "nginx", }, },
                            },
                        },
                        namespaceParameter);

                    // patch
                    {
                        var pod = client.CoreV1.ListNamespacedPod(namespaceParameter).Items.First(p => p.Metadata.Name == podName);
                        var old = JsonSerializer.SerializeToDocument(pod);

                        var newlabels = new Dictionary<string, string>(pod.Metadata.Labels) { ["test"] = "test-jsonpatch" };
                        pod.Metadata.Labels = newlabels;

                        var expected = JsonSerializer.SerializeToDocument(pod);
                        var patch = old.CreatePatch(expected);
                        client.CoreV1.PatchNamespacedPod(new V1Patch(patch, V1Patch.PatchType.JsonPatch), pod.Metadata.Name, "default");
                    }

                    // refresh
                    {
                        var pod = client.CoreV1.ListNamespacedPod(namespaceParameter).Items.First(p => p.Metadata.Name == podName);
                        Assert.Equal("test-jsonpatch", pod.Labels()["test"]);
                    }
                }

                {
                    Cleanup();
                }

                {
                    client.CoreV1.CreateNamespacedPod(
                        new V1Pod()
                        {
                            Metadata = new V1ObjectMeta { Name = podName, Labels = new Dictionary<string, string> { { "place", "holder" }, }, },
                            Spec = new V1PodSpec
                            {
                                Containers = new[] { new V1Container() { Name = "k8scsharp-patch", Image = "nginx", }, },
                            },
                        },
                        namespaceParameter);


                    var pod = client.CoreV1.ListNamespacedPod(namespaceParameter).Items.First(p => p.Metadata.Name == podName);

                    var patchStr = @"
{
    ""metadata"": {
        ""labels"": {
            ""test"": ""test-mergepatch""
        }
    }
}";

                    client.CoreV1.PatchNamespacedPod(new V1Patch(patchStr, V1Patch.PatchType.MergePatch), pod.Metadata.Name, "default");

                    Assert.False(pod.Labels().ContainsKey("test"));

                    // refresh
                    pod = client.CoreV1.ListNamespacedPod(namespaceParameter).Items.First(p => p.Metadata.Name == podName);

                    Assert.Equal("test-mergepatch", pod.Labels()["test"]);
                }
            }
            finally
            {
                Cleanup();
            }
        }

        [MinikubeFact]
        public async Task WatcherIntegrationTest()
        {
            var kubernetes = CreateClient();

            var job = await kubernetes.BatchV1.CreateNamespacedJobAsync(
                new V1Job()
                {
                    ApiVersion = "batch/v1",
                    Kind = V1Job.KubeKind,
                    Metadata = new V1ObjectMeta() { Name = nameof(WatcherIntegrationTest).ToLowerInvariant() },
                    Spec = new V1JobSpec()
                    {
                        Template = new V1PodTemplateSpec()
                        {
                            Spec = new V1PodSpec()
                            {
                                Containers = new List<V1Container>()
                                {
                                    new V1Container()
                                    {
                                        Image = "ubuntu",
                                        Name = "runner",
                                        Command = new List<string>() { "/bin/bash", "-c", "--" },
                                        Args = new List<string>()
                                        {
                                            "trap : TERM INT; sleep infinity & wait",
                                        },
                                    },
                                },
                                RestartPolicy = "Never",
                            },
                        },
                    },
                },
                "default").ConfigureAwait(false);

            var events = new Collection<Tuple<WatchEventType, V1Job>>();

            var started = new AsyncManualResetEvent();
            var connectionClosed = new AsyncManualResetEvent();

            var watcher = kubernetes.BatchV1.ListNamespacedJobWithHttpMessagesAsync(
                job.Metadata.NamespaceProperty,
                fieldSelector: $"metadata.name={job.Metadata.Name}",
                resourceVersion: job.Metadata.ResourceVersion,
                timeoutSeconds: 30,
                watch: true).Watch<V1Job, V1JobList>(
                (type, source) =>
                {
                    Debug.WriteLine($"Watcher 1: {type}, {source}");
                    events.Add(new Tuple<WatchEventType, V1Job>(type, source));
                    job = source;
                    started.Set();
                },
                onClosed: connectionClosed.Set);

            await started.WaitAsync().ConfigureAwait(false);

            await Task.WhenAny(connectionClosed.WaitAsync(), Task.Delay(TimeSpan.FromMinutes(3))).ConfigureAwait(false);
            Assert.True(connectionClosed.IsSet);

            var st = await kubernetes.BatchV1.DeleteNamespacedJobAsync(
                job.Metadata.Name,
                job.Metadata.NamespaceProperty,
                new V1DeleteOptions() { PropagationPolicy = "Foreground" }).ConfigureAwait(false);
        }

        [MinikubeFact]
        public void LeaderIntegrationTest()
        {
            var client = CreateClient();
            var namespaceParameter = "default";

            void Cleanup()
            {
                var endpoints = client.CoreV1.ListNamespacedEndpoints(namespaceParameter);

                void DeleteEndpoints(string name)
                {
                    while (endpoints.Items.Any(p => p.Metadata.Name == name))
                    {
                        try
                        {
                            client.CoreV1.DeleteNamespacedEndpoints(name, namespaceParameter);
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

                DeleteEndpoints("leaderendpoint");
            }

            var cts = new CancellationTokenSource();

            var leader1acq = new ManualResetEvent(false);
            var leader1lose = new ManualResetEvent(false);

            try
            {
                Cleanup();

                var tasks = new List<Task>();

                // 1
                {
                    var l = new EndpointsLock(client, namespaceParameter, "leaderendpoint", "leader1");
                    var le = new LeaderElector(new LeaderElectionConfig(l)
                    {
                        LeaseDuration = TimeSpan.FromSeconds(1),
                        RetryPeriod = TimeSpan.FromMilliseconds(400),
                    });

                    le.OnStartedLeading += () => leader1acq.Set();
                    le.OnStoppedLeading += () => leader1lose.Set();

                    tasks.Add(le.RunAsync(cts.Token));
                }

                // wait 1 become leader
                Assert.True(leader1acq.WaitOne(TimeSpan.FromSeconds(30)));

                // 2
                {
                    var l = new EndpointsLock(client, namespaceParameter, "leaderendpoint", "leader2");
                    var le = new LeaderElector(new LeaderElectionConfig(l)
                    {
                        LeaseDuration = TimeSpan.FromSeconds(1),
                        RetryPeriod = TimeSpan.FromMilliseconds(400),
                    });

                    var leader2init = new ManualResetEvent(false);

                    le.OnNewLeader += _ =>
                    {
                        leader2init.Set();
                    };

                    tasks.Add(le.RunAsync());
                    Assert.True(leader2init.WaitOne(TimeSpan.FromSeconds(30)));

                    Assert.Equal("leader1", le.GetLeader());
                    cts.Cancel();

                    Assert.True(leader1lose.WaitOne(TimeSpan.FromSeconds(30)));
                    Task.Delay(TimeSpan.FromSeconds(3)).Wait();

                    Assert.True(le.IsLeader());
                }

                Task.WaitAll(tasks.ToArray(), TimeSpan.FromSeconds(30));
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

            var client = CreateClient();

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
            var kubernetes = CreateClient();

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
        public async void GenericTest()
        {
            var namespaceParameter = "default";
            var podName = "k8scsharp-e2e-generic-pod";

            var client = CreateClient();
            var genericPods = new GenericClient(client, "", "v1", "pods");

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

                await genericPods.CreateNamespacedAsync(
                    new V1Pod()
                    {
                        Metadata = new V1ObjectMeta { Name = podName, },
                        Spec = new V1PodSpec
                        {
                            Containers = new[] { new V1Container() { Name = "k8scsharp-e2e", Image = "nginx", }, },
                        },
                    },
                    namespaceParameter).ConfigureAwait(false);

                var pods = await genericPods.ListNamespacedAsync<V1PodList>(namespaceParameter).ConfigureAwait(false);
                Assert.Contains(pods.Items, p => p.Metadata.Name == podName);

                int retry = 5;
                while (retry-- > 0)
                {
                    try
                    {
                        await genericPods.DeleteNamespacedAsync<V1Pod>(namespaceParameter, podName).ConfigureAwait(false);
                    }
                    catch (HttpOperationException e)
                    {
                        if (e.Response.StatusCode == System.Net.HttpStatusCode.NotFound)
                        {
                            return;
                        }
                    }

                    pods = await genericPods.ListNamespacedAsync<V1PodList>(namespaceParameter).ConfigureAwait(false);
                    if (!pods.Items.Any(p => p.Metadata.Name == podName))
                    {
                        break;
                    }

                    await Task.Delay(TimeSpan.FromSeconds(2.5)).ConfigureAwait(false);
                }

                Assert.DoesNotContain(pods.Items, p => p.Metadata.Name == podName);
            }
            finally
            {
                Cleanup();
            }
        }


        [MinikubeFact]
        public async Task CopyToPodTestAsync()
        {
            var namespaceParameter = "default";
            var podName = "k8scsharp-e2e-cp-pod";

            var client = CreateClient();

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
