using k8s.Tests.Mock;
using System.Linq;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace k8s.Tests
{
    public class KubernetesMetricsTests
    {
        private readonly ITestOutputHelper testOutput;

        // Copy / Paste from metrics server on minikube
        public const string NodeMetricsResponse = "{\n  \"kind\": \"NodeMetricsList\",\n  \"apiVersion\": \"metrics.k8s.io/v1beta1\",\n  \"metadata\": {\n    \"selfLink\": \"/apis/metrics.k8s.io/v1beta1/nodes/\"\n  },\n  \"items\": [\n    {\n      \"metadata\": {\n        \"name\": \"minikube\",\n        \"selfLink\": \"/apis/metrics.k8s.io/v1beta1/nodes/minikube\",\n        \"creationTimestamp\": \"2020-07-28T20:01:05Z\"\n      },\n      \"timestamp\": \"2020-07-28T20:01:00Z\",\n      \"window\": \"1m0s\",\n      \"usage\": {\n        \"cpu\": \"394m\",\n        \"memory\": \"1948140Ki\"\n      }\n    }\n  ]\n}";
        // Copy / Paste from metrics server minikube
        public const string PodMetricsResponse = "{\n  \"kind\": \"PodMetricsList\",\n  \"apiVersion\": \"metrics.k8s.io/v1beta1\",\n  \"metadata\": {\n    \"selfLink\": \"/apis/metrics.k8s.io/v1beta1/namespaces/default/pods/\"\n  },\n  \"items\": [\n    {\n      \"metadata\": {\n        \"name\": \"dotnet-test-d4894bfbd-2q2dw\",\n        \"namespace\": \"default\",\n        \"selfLink\": \"/apis/metrics.k8s.io/v1beta1/namespaces/default/pods/dotnet-test-d4894bfbd-2q2dw\",\n        \"creationTimestamp\": \"2020-08-01T07:40:05Z\"\n      },\n      \"timestamp\": \"2020-08-01T07:40:00Z\",\n      \"window\": \"1m0s\",\n      \"containers\": [\n        {\n          \"name\": \"dotnet-test\",\n          \"usage\": {\n            \"cpu\": \"0\",\n            \"memory\": \"14512Ki\"\n          }\n        }\n      ]\n    }\n  ]\n}";
        // Copy / Paste from metrics server minikube
        public const string EmptyPodMetricsResponse = "{\n  \"kind\": \"PodMetricsList\",\n  \"apiVersion\": \"metrics.k8s.io/v1beta1\",\n  \"metadata\": {\n    \"selfLink\": \"/apis/metrics.k8s.io/v1beta1/namespaces/empty/pods/\"\n  },\n  \"items\": []\n}";
        // Copy / Paste from metrics server minikube
        public const string NonExistingNamespaceResponse = "{\n  \"kind\": \"PodMetricsList\",\n  \"apiVersion\": \"metrics.k8s.io/v1beta1\",\n  \"metadata\": {\n    \"selfLink\": \"/apis/metrics.k8s.io/v1beta1/namespaces/nonexisting/pods/\"\n  },\n  \"items\": []\n}";

        public const string DefaultNodeName = "minikube";
        public const string DefaultPodName = "dotnet-test";
        public const string DefaultCpuKey = "cpu";
        public const string DefaultMemoryKey = "memory";

        public KubernetesMetricsTests(ITestOutputHelper testOutput)
        {
            this.testOutput = testOutput;
        }

        [Fact(DisplayName = "Node metrics")]
        public async Task NodesMetrics()
        {
            using (var server = new MockKubeApiServer(testOutput, resp: NodeMetricsResponse))
            {
                var client = new Kubernetes(new KubernetesClientConfiguration { Host = server.Uri.ToString() });

                var nodesMetricsList = await client.GetKubernetesNodesMetricsAsync().ConfigureAwait(false);

                Assert.Single(nodesMetricsList.Items);

                var nodeMetrics = nodesMetricsList.Items.First();
                Assert.Equal(DefaultNodeName, nodeMetrics.Metadata.Name);

                Assert.Equal(2, nodeMetrics.Usage.Count);
                Assert.True(nodeMetrics.Usage.ContainsKey(DefaultCpuKey));
                Assert.True(nodeMetrics.Usage.ContainsKey(DefaultMemoryKey));
            }
        }

        [Fact(DisplayName = "Pod metrics")]
        public async Task PodsMetrics()
        {
            using (var server = new MockKubeApiServer(testOutput, resp: PodMetricsResponse))
            {
                var client = new Kubernetes(new KubernetesClientConfiguration { Host = server.Uri.ToString() });

                var podsMetricsList = await client.GetKubernetesPodsMetricsAsync().ConfigureAwait(false);

                Assert.Single(podsMetricsList.Items);

                var podMetrics = podsMetricsList.Items.First();

                Assert.Single(podMetrics.Containers);

                var containerMetrics = podMetrics.Containers.First();
                Assert.Equal(DefaultPodName, containerMetrics.Name);

                Assert.Equal(2, containerMetrics.Usage.Count);
                Assert.True(containerMetrics.Usage.ContainsKey(DefaultCpuKey));
                Assert.True(containerMetrics.Usage.ContainsKey(DefaultMemoryKey));
            }
        }

        [Fact(DisplayName = "Pod metrics empty response")]
        public async Task PodsMetricsEmptyResponse()
        {
            using (var server = new MockKubeApiServer(testOutput, resp: EmptyPodMetricsResponse))
            {
                var client = new Kubernetes(new KubernetesClientConfiguration { Host = server.Uri.ToString() });

                var podsMetricsList = await client.GetKubernetesPodsMetricsByNamespaceAsync("empty").ConfigureAwait(false);

                Assert.Empty(podsMetricsList.Items);
            }
        }

        [Fact(DisplayName = "Pod metrics by namespace")]
        public async Task PodsMetricsByNamespace()
        {
            var namespaceName = "default";

            using (var server = new MockKubeApiServer(testOutput, resp: PodMetricsResponse))
            {
                var client = new Kubernetes(new KubernetesClientConfiguration { Host = server.Uri.ToString() });

                var podsMetricsList = await client.GetKubernetesPodsMetricsByNamespaceAsync(namespaceName).ConfigureAwait(false);

                Assert.Single(podsMetricsList.Items);

                var podMetrics = podsMetricsList.Items.First();
                Assert.Equal(namespaceName, podMetrics.Metadata.NamespaceProperty);

                Assert.Single(podMetrics.Containers);

                var containerMetrics = podMetrics.Containers.First();
                Assert.Equal(DefaultPodName, containerMetrics.Name);

                Assert.Equal(2, containerMetrics.Usage.Count);
                Assert.True(containerMetrics.Usage.ContainsKey(DefaultCpuKey));
                Assert.True(containerMetrics.Usage.ContainsKey(DefaultMemoryKey));
            }
        }

        [Fact(DisplayName = "Pod metrics non existing namespace response")]
        public async Task PodsMetricsNonExistingNamespaceResponse()
        {
            using (var server = new MockKubeApiServer(testOutput, resp: NonExistingNamespaceResponse))
            {
                var client = new Kubernetes(new KubernetesClientConfiguration { Host = server.Uri.ToString() });

                var podsMetricsList = await client.GetKubernetesPodsMetricsByNamespaceAsync("nonexisting").ConfigureAwait(false);

                Assert.Empty(podsMetricsList.Items);
            }
        }
    }
}
