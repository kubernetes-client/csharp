using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using k8s.LeaderElection;
using k8s.LeaderElection.ResourceLock;
using k8s.Models;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.Rest;
using Nito.AsyncEx;
using Xunit;

namespace k8s.E2E
{
    public class MnikubeTests
    {
        [MinikubeFact]
        public void SimpleTest()
        {
            var namespaceParameter = "default";
            var podName = "k8scsharp-e2e-pod";

            var client = CreateClient();

            void Cleanup()
            {
                var pods = client.ListNamespacedPod(namespaceParameter);
                if (pods.Items.Any(p => p.Metadata.Name == podName))
                {
                    client.DeleteNamespacedPod(podName, namespaceParameter);
                }
            }

            try
            {
                Cleanup();

                client.CreateNamespacedPod(
                    new V1Pod()
                    {
                        Metadata = new V1ObjectMeta { Name = podName, },
                        Spec = new V1PodSpec
                        {
                            Containers = new[] { new V1Container() { Name = "k8scsharp-e2e", Image = "nginx", }, },
                        },
                    },
                    namespaceParameter);

                var pods = client.ListNamespacedPod(namespaceParameter);
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
                var pods = client.ListNamespacedPod(namespaceParameter);
                while (pods.Items.Any(p => p.Metadata.Name == podName))
                {
                    try
                    {
                        client.DeleteNamespacedPod(podName, namespaceParameter);
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

                    client.CreateNamespacedPod(
                        new V1Pod()
                        {
                            Metadata = new V1ObjectMeta { Name = podName, Labels = new Dictionary<string, string> { { "place", "holder" }, }, },
                            Spec = new V1PodSpec
                            {
                                Containers = new[] { new V1Container() { Name = "k8scsharp-patch", Image = "nginx", }, },
                            },
                        },
                        namespaceParameter);


                    var pod = client.ListNamespacedPod(namespaceParameter).Items.First(p => p.Metadata.Name == podName);

                    var newlabels = new Dictionary<string, string>(pod.Metadata.Labels) { ["test"] = "test-jsonpatch" };
                    var patch = new JsonPatchDocument<V1Pod>();
                    patch.Replace(e => e.Metadata.Labels, newlabels);
                    client.PatchNamespacedPod(new V1Patch(patch, V1Patch.PatchType.JsonPatch), pod.Metadata.Name, "default");

                    Assert.False(pod.Labels().ContainsKey("test"));

                    // refresh
                    pod = client.ListNamespacedPod(namespaceParameter).Items.First(p => p.Metadata.Name == podName);

                    Assert.Equal("test-jsonpatch", pod.Labels()["test"]);
                }

                {
                    Cleanup();
                }

                {
                    client.CreateNamespacedPod(
                        new V1Pod()
                        {
                            Metadata = new V1ObjectMeta { Name = podName, Labels = new Dictionary<string, string> { { "place", "holder" }, }, },
                            Spec = new V1PodSpec
                            {
                                Containers = new[] { new V1Container() { Name = "k8scsharp-patch", Image = "nginx", }, },
                            },
                        },
                        namespaceParameter);


                    var pod = client.ListNamespacedPod(namespaceParameter).Items.First(p => p.Metadata.Name == podName);

                    var patchStr = @"
{
    ""metadata"": {
        ""labels"": {
            ""test"": ""test-mergepatch""
        }
    }
}";

                    client.PatchNamespacedPod(new V1Patch(patchStr, V1Patch.PatchType.MergePatch), pod.Metadata.Name, "default");

                    Assert.False(pod.Labels().ContainsKey("test"));

                    // refresh
                    pod = client.ListNamespacedPod(namespaceParameter).Items.First(p => p.Metadata.Name == podName);

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

            var job = await kubernetes.CreateNamespacedJobAsync(
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

            var watcher = await kubernetes.WatchNamespacedJobAsync(
                job.Metadata.Name,
                job.Metadata.NamespaceProperty,
                resourceVersion: job.Metadata.ResourceVersion,
                timeoutSeconds: 30,
                onEvent:
                (type, source) =>
                {
                    Debug.WriteLine($"Watcher 1: {type}, {source}");
                    events.Add(new Tuple<WatchEventType, V1Job>(type, source));
                    job = source;
                    started.Set();
                },
                onClosed: connectionClosed.Set).ConfigureAwait(false);

            await started.WaitAsync().ConfigureAwait(false);

            await Task.WhenAny(connectionClosed.WaitAsync(), Task.Delay(TimeSpan.FromMinutes(3))).ConfigureAwait(false);
            Assert.True(connectionClosed.IsSet);

            await kubernetes.DeleteNamespacedJobAsync(
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
                var endpoints = client.ListNamespacedEndpoints(namespaceParameter);

                void DeleteEndpoints(string name)
                {
                    while (endpoints.Items.Any(p => p.Metadata.Name == name))
                    {
                        try
                        {
                            client.DeleteNamespacedEndpoints(name, namespaceParameter);
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
        public async Task DatetimeFieldTest()
        {
            var kubernetes = CreateClient();

            await kubernetes.CreateNamespacedEventAsync(
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

        private static IKubernetes CreateClient()
        {
            return new Kubernetes(KubernetesClientConfiguration.BuildDefaultConfig());
        }
    }
}
