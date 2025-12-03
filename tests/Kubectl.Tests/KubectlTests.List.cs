using k8s.Autorest;
using k8s.E2E;
using k8s.kubectl.beta;
using k8s.Models;
using Xunit;

namespace k8s.kubectl.Tests;

public partial class KubectlTests
{
    [MinikubeFact]
    public void ListNamespaces()
    {
        using var kubernetes = MinikubeTests.CreateClient();
        var client = new Kubectl(kubernetes);

        // List all namespaces (cluster-scoped resources)
        var namespaces = client.List<V1NamespaceList>();

        Assert.NotNull(namespaces);
        Assert.NotNull(namespaces.Items);
        Assert.Contains(namespaces.Items, ns => ns.Metadata.Name == "default");
        Assert.Contains(namespaces.Items, ns => ns.Metadata.Name == "kube-system");
    }

    [MinikubeFact]
    public void ListPods()
    {
        using var kubernetes = MinikubeTests.CreateClient();
        var client = new Kubectl(kubernetes);
        var namespaceParameter = "default";
        var podName = "k8scsharp-e2e-list-pod";

        // Create a test pod with a label
        var pod = new V1Pod
        {
            Metadata = new V1ObjectMeta
            {
                Name = podName,
                NamespaceProperty = namespaceParameter,
                Labels = new Dictionary<string, string>
                {
                    { "app", "k8scsharp-e2e-test" },
                    { "test-type", "list" },
                },
            },
            Spec = new V1PodSpec
            {
                Containers = new[]
                {
                    new V1Container
                    {
                        Name = "test",
                        Image = "nginx:latest",
                    },
                },
            },
        };

        try
        {
            kubernetes.CoreV1.CreateNamespacedPod(pod, namespaceParameter);

            // List pods in the namespace
            var pods = client.List<V1PodList>(namespaceParameter);

            Assert.NotNull(pods);
            Assert.NotNull(pods.Items);
            Assert.Contains(pods.Items, p => p.Metadata.Name == podName);
        }
        finally
        {
            // Cleanup
            try
            {
                kubernetes.CoreV1.DeleteNamespacedPod(podName, namespaceParameter);
            }
            catch (HttpOperationException)
            {
                // Ignore cleanup errors if pod was already deleted or doesn't exist
            }
        }
    }

    [MinikubeFact]
    public void ListPodsWithLabelSelector()
    {
        using var kubernetes = MinikubeTests.CreateClient();
        var client = new Kubectl(kubernetes);
        var namespaceParameter = "default";
        var podNameA = "k8scsharp-e2e-list-selector-a";
        var podNameB = "k8scsharp-e2e-list-selector-b";

        // Create two test pods with different labels
        var podA = new V1Pod
        {
            Metadata = new V1ObjectMeta
            {
                Name = podNameA,
                NamespaceProperty = namespaceParameter,
                Labels = new Dictionary<string, string>
                {
                    { "app", "k8scsharp-e2e-selector-test" },
                    { "tier", "frontend" },
                },
            },
            Spec = new V1PodSpec
            {
                Containers = new[]
                {
                    new V1Container
                    {
                        Name = "test",
                        Image = "nginx:latest",
                    },
                },
            },
        };

        var podB = new V1Pod
        {
            Metadata = new V1ObjectMeta
            {
                Name = podNameB,
                NamespaceProperty = namespaceParameter,
                Labels = new Dictionary<string, string>
                {
                    { "app", "k8scsharp-e2e-selector-test" },
                    { "tier", "backend" },
                },
            },
            Spec = new V1PodSpec
            {
                Containers = new[]
                {
                    new V1Container
                    {
                        Name = "test",
                        Image = "nginx:latest",
                    },
                },
            },
        };

        try
        {
            kubernetes.CoreV1.CreateNamespacedPod(podA, namespaceParameter);
            kubernetes.CoreV1.CreateNamespacedPod(podB, namespaceParameter);

            // List pods with label selector for frontend tier only
            var frontendPods = client.List<V1PodList>(
                namespaceParameter,
                labelSelector: "tier=frontend,app=k8scsharp-e2e-selector-test");

            Assert.NotNull(frontendPods);
            Assert.NotNull(frontendPods.Items);
            Assert.Single(frontendPods.Items);
            Assert.Equal(podNameA, frontendPods.Items[0].Metadata.Name);

            // List pods with label selector for app only (should return both)
            var allTestPods = client.List<V1PodList>(
                namespaceParameter,
                labelSelector: "app=k8scsharp-e2e-selector-test");

            Assert.NotNull(allTestPods);
            Assert.NotNull(allTestPods.Items);
            Assert.Equal(2, allTestPods.Items.Count);
        }
        finally
        {
            // Cleanup
            try
            {
                kubernetes.CoreV1.DeleteNamespacedPod(podNameA, namespaceParameter);
            }
            catch (HttpOperationException)
            {
                // Ignore cleanup errors
            }

            try
            {
                kubernetes.CoreV1.DeleteNamespacedPod(podNameB, namespaceParameter);
            }
            catch (HttpOperationException)
            {
                // Ignore cleanup errors
            }
        }
    }

    [MinikubeFact]
    public void ListPodsWithFieldSelector()
    {
        using var kubernetes = MinikubeTests.CreateClient();
        var client = new Kubectl(kubernetes);
        var namespaceParameter = "default";
        var podName = "k8scsharp-e2e-list-field";

        // Create a test pod
        var pod = new V1Pod
        {
            Metadata = new V1ObjectMeta
            {
                Name = podName,
                NamespaceProperty = namespaceParameter,
            },
            Spec = new V1PodSpec
            {
                Containers = new[]
                {
                    new V1Container
                    {
                        Name = "test",
                        Image = "nginx:latest",
                    },
                },
            },
        };

        try
        {
            kubernetes.CoreV1.CreateNamespacedPod(pod, namespaceParameter);

            // List pods with field selector for specific metadata.name
            var pods = client.List<V1PodList>(
                namespaceParameter,
                fieldSelector: $"metadata.name={podName}");

            Assert.NotNull(pods);
            Assert.NotNull(pods.Items);
            Assert.Single(pods.Items);
            Assert.Equal(podName, pods.Items[0].Metadata.Name);
        }
        finally
        {
            // Cleanup
            try
            {
                kubernetes.CoreV1.DeleteNamespacedPod(podName, namespaceParameter);
            }
            catch (HttpOperationException)
            {
                // Ignore cleanup errors
            }
        }
    }

    [MinikubeFact]
    public void ListServicesInNamespace()
    {
        using var kubernetes = MinikubeTests.CreateClient();
        var client = new Kubectl(kubernetes);
        var namespaceParameter = "default";
        var serviceName = "k8scsharp-e2e-list-service";

        // Create a test service
        var service = new V1Service
        {
            Metadata = new V1ObjectMeta
            {
                Name = serviceName,
                NamespaceProperty = namespaceParameter,
            },
            Spec = new V1ServiceSpec
            {
                Ports = new[]
                {
                    new V1ServicePort
                    {
                        Port = 80,
                        TargetPort = 80,
                    },
                },
                Selector = new Dictionary<string, string>
                {
                    { "app", "test" },
                },
            },
        };

        try
        {
            kubernetes.CoreV1.CreateNamespacedService(service, namespaceParameter);

            // List services in the namespace
            var services = client.List<V1ServiceList>(namespaceParameter);

            Assert.NotNull(services);
            Assert.NotNull(services.Items);
            Assert.Contains(services.Items, s => s.Metadata.Name == serviceName);
        }
        finally
        {
            // Cleanup
            try
            {
                kubernetes.CoreV1.DeleteNamespacedService(serviceName, namespaceParameter);
            }
            catch (HttpOperationException)
            {
                // Ignore cleanup errors
            }
        }
    }

    [MinikubeFact]
    public void ListDeployments()
    {
        using var kubernetes = MinikubeTests.CreateClient();
        var client = new Kubectl(kubernetes);
        var namespaceParameter = "default";
        var deploymentName = "k8scsharp-e2e-list-deployment";

        // Create a test deployment
        var deployment = new V1Deployment
        {
            Metadata = new V1ObjectMeta
            {
                Name = deploymentName,
                NamespaceProperty = namespaceParameter,
            },
            Spec = new V1DeploymentSpec
            {
                Replicas = 1,
                Selector = new V1LabelSelector
                {
                    MatchLabels = new Dictionary<string, string>
                    {
                        { "app", "test" },
                    },
                },
                Template = new V1PodTemplateSpec
                {
                    Metadata = new V1ObjectMeta
                    {
                        Labels = new Dictionary<string, string>
                        {
                            { "app", "test" },
                        },
                    },
                    Spec = new V1PodSpec
                    {
                        Containers = new[]
                        {
                            new V1Container
                            {
                                Name = "test",
                                Image = "nginx:latest",
                            },
                        },
                    },
                },
            },
        };

        try
        {
            kubernetes.AppsV1.CreateNamespacedDeployment(deployment, namespaceParameter);

            // List deployments in the namespace
            var deployments = client.List<V1DeploymentList>(namespaceParameter);

            Assert.NotNull(deployments);
            Assert.NotNull(deployments.Items);
            Assert.Contains(deployments.Items, d => d.Metadata.Name == deploymentName);
        }
        finally
        {
            // Cleanup
            try
            {
                kubernetes.AppsV1.DeleteNamespacedDeployment(deploymentName, namespaceParameter);
            }
            catch (HttpOperationException)
            {
                // Ignore cleanup errors
            }
        }
    }
}
