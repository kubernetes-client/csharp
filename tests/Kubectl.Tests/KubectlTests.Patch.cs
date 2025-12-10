using k8s.Autorest;
using k8s.E2E;
using k8s.kubectl.beta;
using k8s.Models;
using Xunit;

namespace k8s.kubectl.Tests;

public partial class KubectlTests
{
    [MinikubeFact]
    public void PatchConfigMapWithStrategicMergePatch()
    {
        using var kubernetes = MinikubeTests.CreateClient();
        var client = new Kubectl(kubernetes);
        var namespaceParameter = "default";
        var configMapName = "k8scsharp-e2e-patch-strategic";

        // Create a test ConfigMap
        var configMap = new V1ConfigMap
        {
            Metadata = new V1ObjectMeta
            {
                Name = configMapName,
                NamespaceProperty = namespaceParameter,
            },
            Data = new Dictionary<string, string>
            {
                { "key1", "value1" },
                { "key2", "value2" },
            },
        };

        try
        {
            kubernetes.CoreV1.CreateNamespacedConfigMap(configMap, namespaceParameter);

            // Patch the ConfigMap using strategic merge patch
            var patchData = new
            {
                data = new Dictionary<string, string>
                {
                    { "key3", "value3" },
                },
            };

            var patch = new V1Patch(patchData, V1Patch.PatchType.StrategicMergePatch);
            var patchedConfigMap = client.PatchNamespaced<V1ConfigMap>(patch, namespaceParameter, configMapName);

            Assert.NotNull(patchedConfigMap);
            Assert.Equal(configMapName, patchedConfigMap.Metadata.Name);
            Assert.Equal(3, patchedConfigMap.Data.Count);
            Assert.Equal("value1", patchedConfigMap.Data["key1"]);
            Assert.Equal("value2", patchedConfigMap.Data["key2"]);
            Assert.Equal("value3", patchedConfigMap.Data["key3"]);
        }
        finally
        {
            // Cleanup
            try
            {
                kubernetes.CoreV1.DeleteNamespacedConfigMap(configMapName, namespaceParameter);
            }
            catch (HttpOperationException)
            {
                // Ignore cleanup errors
            }
        }
    }

    [MinikubeFact]
    public void PatchConfigMapWithMergePatch()
    {
        using var kubernetes = MinikubeTests.CreateClient();
        var client = new Kubectl(kubernetes);
        var namespaceParameter = "default";
        var configMapName = "k8scsharp-e2e-patch-merge";

        // Create a test ConfigMap
        var configMap = new V1ConfigMap
        {
            Metadata = new V1ObjectMeta
            {
                Name = configMapName,
                NamespaceProperty = namespaceParameter,
                Labels = new Dictionary<string, string>
                {
                    { "app", "test" },
                },
            },
            Data = new Dictionary<string, string>
            {
                { "key1", "value1" },
            },
        };

        try
        {
            kubernetes.CoreV1.CreateNamespacedConfigMap(configMap, namespaceParameter);

            // Patch the ConfigMap using merge patch
            var patchData = new
            {
                metadata = new
                {
                    labels = new Dictionary<string, string>
                    {
                        { "app", "test" },
                        { "environment", "testing" },
                    },
                },
                data = new Dictionary<string, string>
                {
                    { "key1", "updatedValue1" },
                    { "key2", "value2" },
                },
            };

            var patch = new V1Patch(patchData, V1Patch.PatchType.MergePatch);
            var patchedConfigMap = client.PatchNamespaced<V1ConfigMap>(patch, namespaceParameter, configMapName);

            Assert.NotNull(patchedConfigMap);
            Assert.Equal(configMapName, patchedConfigMap.Metadata.Name);
            Assert.Equal(2, patchedConfigMap.Metadata.Labels.Count);
            Assert.Equal("test", patchedConfigMap.Metadata.Labels["app"]);
            Assert.Equal("testing", patchedConfigMap.Metadata.Labels["environment"]);
            Assert.Equal(2, patchedConfigMap.Data.Count);
            Assert.Equal("updatedValue1", patchedConfigMap.Data["key1"]);
            Assert.Equal("value2", patchedConfigMap.Data["key2"]);
        }
        finally
        {
            // Cleanup
            try
            {
                kubernetes.CoreV1.DeleteNamespacedConfigMap(configMapName, namespaceParameter);
            }
            catch (HttpOperationException)
            {
                // Ignore cleanup errors
            }
        }
    }

    [MinikubeFact]
    public void PatchConfigMapWithJsonPatch()
    {
        using var kubernetes = MinikubeTests.CreateClient();
        var client = new Kubectl(kubernetes);
        var namespaceParameter = "default";
        var configMapName = "k8scsharp-e2e-patch-json";

        // Create a test ConfigMap
        var configMap = new V1ConfigMap
        {
            Metadata = new V1ObjectMeta
            {
                Name = configMapName,
                NamespaceProperty = namespaceParameter,
            },
            Data = new Dictionary<string, string>
            {
                { "key1", "value1" },
            },
        };

        try
        {
            kubernetes.CoreV1.CreateNamespacedConfigMap(configMap, namespaceParameter);

            // Patch the ConfigMap using JSON patch
            var patchData = new[]
            {
                new
                {
                    op = "replace",
                    path = "/data/key1",
                    value = "updatedValue1",
                },
                new
                {
                    op = "add",
                    path = "/data/key2",
                    value = "value2",
                },
            };

            var patch = new V1Patch(patchData, V1Patch.PatchType.JsonPatch);
            var patchedConfigMap = client.PatchNamespaced<V1ConfigMap>(patch, namespaceParameter, configMapName);

            Assert.NotNull(patchedConfigMap);
            Assert.Equal(configMapName, patchedConfigMap.Metadata.Name);
            Assert.Equal(2, patchedConfigMap.Data.Count);
            Assert.Equal("updatedValue1", patchedConfigMap.Data["key1"]);
            Assert.Equal("value2", patchedConfigMap.Data["key2"]);
        }
        finally
        {
            // Cleanup
            try
            {
                kubernetes.CoreV1.DeleteNamespacedConfigMap(configMapName, namespaceParameter);
            }
            catch (HttpOperationException)
            {
                // Ignore cleanup errors
            }
        }
    }

    [MinikubeFact]
    public void PatchNamespace()
    {
        using var kubernetes = MinikubeTests.CreateClient();
        var client = new Kubectl(kubernetes);
        var namespaceName = "k8scsharp-e2e-patch-ns";

        // Create a test namespace
        var ns = new V1Namespace
        {
            Metadata = new V1ObjectMeta
            {
                Name = namespaceName,
                Labels = new Dictionary<string, string>
                {
                    { "app", "test" },
                },
            },
        };

        try
        {
            kubernetes.CoreV1.CreateNamespace(ns);

            // Patch the namespace (cluster-scoped resource)
            var patchData = new
            {
                metadata = new
                {
                    labels = new Dictionary<string, string>
                    {
                        { "app", "test" },
                        { "patched", "true" },
                    },
                },
            };

            var patch = new V1Patch(patchData, V1Patch.PatchType.MergePatch);
            var patchedNamespace = client.Patch<V1Namespace>(patch, namespaceName);

            Assert.NotNull(patchedNamespace);
            Assert.Equal(namespaceName, patchedNamespace.Metadata.Name);
            Assert.Equal(2, patchedNamespace.Metadata.Labels.Count);
            Assert.Equal("test", patchedNamespace.Metadata.Labels["app"]);
            Assert.Equal("true", patchedNamespace.Metadata.Labels["patched"]);
        }
        finally
        {
            // Cleanup
            try
            {
                kubernetes.CoreV1.DeleteNamespace(namespaceName);
            }
            catch (HttpOperationException)
            {
                // Ignore cleanup errors
            }
        }
    }
}
