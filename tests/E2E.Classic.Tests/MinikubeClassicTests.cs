using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading;
using k8s.Autorest;
using k8s.Models;
using Xunit;

namespace k8s.E2E
{
    [Collection(nameof(Onebyone))]
    public class MinikubeClassicTests
    {
        // A small, self contained smoke test that exercises the .NET Framework (net48)
        // build of the Classic client against a real cluster (kind running inside WSL on
        // the Windows CI runner). ConfigMaps are used instead of Pods so the test does not
        // depend on image pulls or scheduling, keeping it fast and reliable.
        [MinikubeFact]
        public void ConfigMapRoundTrip()
        {
            var namespaceParameter = "default";
            var name = "k8scsharp-e2e-classic-configmap";

            using var client = CreateClient();

            void Cleanup()
            {
                var maps = client.CoreV1.ListNamespacedConfigMap(namespaceParameter);
                while (maps.Items.Any(m => m.Metadata.Name == name))
                {
                    try
                    {
                        client.CoreV1.DeleteNamespacedConfigMap(name, namespaceParameter);
                    }
                    catch (HttpOperationException e) when (e.Response.StatusCode == HttpStatusCode.NotFound)
                    {
                        return;
                    }

                    Thread.Sleep(TimeSpan.FromSeconds(1));
                    maps = client.CoreV1.ListNamespacedConfigMap(namespaceParameter);
                }
            }

            try
            {
                Cleanup();

                client.CoreV1.CreateNamespacedConfigMap(
                    new V1ConfigMap
                    {
                        Metadata = new V1ObjectMeta { Name = name },
                        Data = new Dictionary<string, string> { ["key"] = "value" },
                    },
                    namespaceParameter);

                var read = client.CoreV1.ReadNamespacedConfigMap(name, namespaceParameter);
                Assert.Equal("value", read.Data["key"]);

                var list = client.CoreV1.ListNamespacedConfigMap(namespaceParameter);
                Assert.Contains(list.Items, m => m.Metadata.Name == name);
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
