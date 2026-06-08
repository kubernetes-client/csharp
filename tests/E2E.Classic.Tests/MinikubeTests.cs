using k8s.Autorest;
using k8s.Models;
using System;
using System.Collections.Concurrent;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace k8s.E2E
{
    [Collection(nameof(Onebyone))]
    public class MinikubeTests
    {
        static MinikubeTests()
        {
            // .NET Framework 4.8 defaults to TLS 1.0/1.1; K8s API server requires TLS 1.2+
            ServicePointManager.SecurityProtocol |= SecurityProtocolType.Tls12;
        }

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

                var lines = new ConcurrentQueue<string>();
                using var started = new ManualResetEvent(false);

                async Task<V1Pod> Pod()
                {
                    var deadline = DateTime.UtcNow.AddMinutes(2);
                    while (true)
                    {
                        var pods = client.CoreV1.ListNamespacedPod(namespaceParameter);
                        var pod = pods.Items.First(p => p.Metadata.Name == podName);
                        if (pod.Status.Phase == "Running")
                        {
                            return pod;
                        }

                        if (DateTime.UtcNow > deadline)
                        {
                            throw new TimeoutException($"Pod {podName} did not become Running within 2 minutes (last phase: {pod.Status.Phase})");
                        }

                        await Task.Delay(TimeSpan.FromSeconds(1)).ConfigureAwait(false);
                    }
                }

                var pod = await Pod().ConfigureAwait(false);
                var stream = client.CoreV1.ReadNamespacedPodLog(pod.Metadata.Name, pod.Metadata.NamespaceProperty, follow: true);
                using var reader = new StreamReader(stream);

                var copytask = Task.Run(() =>
                {
                    try
                    {
                        for (; ; )
                        {
                            var line = reader.ReadLine();
                            if (line == null)
                            {
                                break;
                            }

                            lines.Enqueue(line);
                            started.Set();
                        }
                    }
                    catch (ObjectDisposedException)
                    {
                        // Reader was disposed during test teardown; normal shutdown.
                    }
                    finally
                    {
                        started.Set();
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
                new Corev1Event
                {
                    InvolvedObject = new V1ObjectReference
                    {
                        ApiVersion = "v1alpha1",
                        Kind = "Test",
                        Name = "test",
                        NamespaceProperty = "default",
                        ResourceVersion = "1",
                        Uid = "1",
                    },
                    Metadata = new V1ObjectMeta
                    {
                        GenerateName = "started-",
                    },
                    Action = "STARTED",
                    Type = "Normal",
                    Reason = "STARTED",
                    Message = "Started",
                    EventTime = DateTime.Now,
                    FirstTimestamp = DateTime.Now,
                    LastTimestamp = DateTime.Now,
                    ReportingComponent = "37",
                    ReportingInstance = "38",
                },
                "default"
            ).ConfigureAwait(false);
        }

        [MinikubeFact]
        public async Task VersionTestAsync()
        {
            using var client = CreateClient();
            var version = await client.Version.GetCodeAsync().ConfigureAwait(false);
            Assert.NotNull(version);
        }

        public static IKubernetes CreateClient()
        {
            return new Kubernetes(KubernetesClientConfiguration.BuildDefaultConfig());
        }
    }
}
