using System;
using System.Linq;
using k8s.Models;
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
            var podName = "k8s-e2e-pod";

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
                            Containers = new[] { new V1Container() { Name = "k8s-e2e", Image = "nginx", }, },
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

        private static IKubernetes CreateClient()
        {
            return new Kubernetes(KubernetesClientConfiguration.BuildDefaultConfig());
        }
    }
}
