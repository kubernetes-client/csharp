using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using k8s.Models;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.Rest;
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



        private static IKubernetes CreateClient()
        {
            return new Kubernetes(KubernetesClientConfiguration.BuildDefaultConfig());
        }
    }
}
