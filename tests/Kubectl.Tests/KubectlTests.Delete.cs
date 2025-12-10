using k8s.Autorest;
using k8s.E2E;
using k8s.kubectl.beta;
using k8s.Models;
using Xunit;

namespace k8s.kubectl.Tests;

public partial class KubectlTests
{
    [MinikubeFact]
    public void DeletePod()
    {
        using var kubernetes = MinikubeTests.CreateClient();
        var client = new Kubectl(kubernetes);
        var namespaceParameter = "default";
        var podName = "k8scsharp-e2e-delete-pod";

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

        kubernetes.CoreV1.CreateNamespacedPod(pod, namespaceParameter);

        // Delete the pod using kubectl
        var deletedPod = client.Delete<V1Pod>(podName, namespaceParameter);

        Assert.NotNull(deletedPod);
        Assert.Equal(podName, deletedPod.Metadata.Name);
        Assert.Equal(namespaceParameter, deletedPod.Metadata.NamespaceProperty);

        // Verify the pod is deleted by checking it doesn't exist
        Assert.Throws<HttpOperationException>(() =>
        {
            kubernetes.CoreV1.ReadNamespacedPod(podName, namespaceParameter);
        });
    }

    [MinikubeFact]
    public void DeleteService()
    {
        using var kubernetes = MinikubeTests.CreateClient();
        var client = new Kubectl(kubernetes);
        var namespaceParameter = "default";
        var serviceName = "k8scsharp-e2e-delete-service";

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

        kubernetes.CoreV1.CreateNamespacedService(service, namespaceParameter);

        // Delete the service using kubectl
        var deletedService = client.Delete<V1Service>(serviceName, namespaceParameter);

        Assert.NotNull(deletedService);
        Assert.Equal(serviceName, deletedService.Metadata.Name);
        Assert.Equal(namespaceParameter, deletedService.Metadata.NamespaceProperty);

        // Verify the service is deleted
        Assert.Throws<HttpOperationException>(() =>
        {
            kubernetes.CoreV1.ReadNamespacedService(serviceName, namespaceParameter);
        });
    }

    [MinikubeFact]
    public void DeleteDeployment()
    {
        using var kubernetes = MinikubeTests.CreateClient();
        var client = new Kubectl(kubernetes);
        var namespaceParameter = "default";
        var deploymentName = "k8scsharp-e2e-delete-deployment";

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

        kubernetes.AppsV1.CreateNamespacedDeployment(deployment, namespaceParameter);

        // Delete the deployment using kubectl
        var deletedDeployment = client.Delete<V1Deployment>(deploymentName, namespaceParameter);

        Assert.NotNull(deletedDeployment);
        Assert.Equal(deploymentName, deletedDeployment.Metadata.Name);
        Assert.Equal(namespaceParameter, deletedDeployment.Metadata.NamespaceProperty);

        // Verify the deployment is deleted
        Assert.Throws<HttpOperationException>(() =>
        {
            kubernetes.AppsV1.ReadNamespacedDeployment(deploymentName, namespaceParameter);
        });
    }

    [MinikubeFact]
    public void DeleteNamespace()
    {
        using var kubernetes = MinikubeTests.CreateClient();
        var client = new Kubectl(kubernetes);
        var namespaceName = "k8scsharp-e2e-delete-ns";

        try
        {
            // Create a test namespace
            var ns = new V1Namespace
            {
                Metadata = new V1ObjectMeta
                {
                    Name = namespaceName,
                },
            };

            kubernetes.CoreV1.CreateNamespace(ns);

            // Delete the namespace using kubectl (cluster-scoped resource, no namespace parameter)
            var deletedNs = client.Delete<V1Namespace>(namespaceName);

            Assert.NotNull(deletedNs);
            Assert.Equal(namespaceName, deletedNs.Metadata.Name);

            // Verify the namespace is being deleted or already deleted
            // Note: Namespace deletion is async, so it might still exist in Terminating state
            try
            {
                var readNs = kubernetes.CoreV1.ReadNamespace(namespaceName);
                // If we can still read it, it should be in Terminating status
                Assert.Equal("Terminating", readNs.Status?.Phase);
            }
            catch (HttpOperationException)
            {
                // If we can't read it, that's also fine - it's been deleted
            }
        }
        finally
        {
            try
            {
                var ns = kubernetes.CoreV1.ReadNamespace(namespaceName);
                if (ns != null && ns.Status?.Phase != "Terminating")
                {
                    kubernetes.CoreV1.DeleteNamespace(namespaceName, new V1DeleteOptions());
                }
            }
            catch (HttpOperationException)
            {
                // Ignore - already deleted or not found
            }
        }
    }
}
