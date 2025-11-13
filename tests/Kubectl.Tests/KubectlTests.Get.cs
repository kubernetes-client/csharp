using k8s.E2E;
using k8s.kubectl.beta;
using k8s.Models;
using Xunit;

namespace k8s.kubectl.Tests;

public partial class KubectlTests
{
    [MinikubeFact]
    public void GetNamespace()
    {
        using var kubernetes = MinikubeTests.CreateClient();
        var client = new Kubectl(kubernetes);

        // Get the default namespace (cluster-scoped resource, no namespace parameter)
        var ns = client.Get<V1Namespace>("default");

        Assert.NotNull(ns);
        Assert.Equal("default", ns.Metadata.Name);
        Assert.Equal("v1", ns.ApiVersion);
        Assert.Equal("Namespace", ns.Kind);
    }

    [MinikubeFact]
    public void GetPod()
    {
        using var kubernetes = MinikubeTests.CreateClient();
        var client = new Kubectl(kubernetes);
        var namespaceParameter = "default";
        var podName = "k8scsharp-e2e-get-pod";

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

            // Get the pod using kubectl generic get
            var retrievedPod = client.Get<V1Pod>(podName, namespaceParameter);

            Assert.NotNull(retrievedPod);
            Assert.Equal(podName, retrievedPod.Metadata.Name);
            Assert.Equal(namespaceParameter, retrievedPod.Metadata.NamespaceProperty);
            Assert.Equal("Pod", retrievedPod.Kind);
            Assert.Equal("v1", retrievedPod.ApiVersion);
        }
        finally
        {
            // Cleanup
            try
            {
                kubernetes.CoreV1.DeleteNamespacedPod(podName, namespaceParameter);
            }
            catch
            {
                // Ignore cleanup errors
            }
        }
    }

    [MinikubeFact]
    public void GetService()
    {
        using var kubernetes = MinikubeTests.CreateClient();
        var client = new Kubectl(kubernetes);
        var namespaceParameter = "default";
        var serviceName = "k8scsharp-e2e-get-service";

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

            // Get the service using kubectl generic get
            var retrievedService = client.Get<V1Service>(serviceName, namespaceParameter);

            Assert.NotNull(retrievedService);
            Assert.Equal(serviceName, retrievedService.Metadata.Name);
            Assert.Equal(namespaceParameter, retrievedService.Metadata.NamespaceProperty);
            Assert.Equal("Service", retrievedService.Kind);
            Assert.Equal("v1", retrievedService.ApiVersion);
        }
        finally
        {
            // Cleanup
            try
            {
                kubernetes.CoreV1.DeleteNamespacedService(serviceName, namespaceParameter);
            }
            catch
            {
                // Ignore cleanup errors
            }
        }
    }

    [MinikubeFact]
    public void GetDeployment()
    {
        using var kubernetes = MinikubeTests.CreateClient();
        var client = new Kubectl(kubernetes);
        var namespaceParameter = "default";
        var deploymentName = "k8scsharp-e2e-get-deployment";

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

            // Get the deployment using kubectl generic get
            var retrievedDeployment = client.Get<V1Deployment>(deploymentName, namespaceParameter);

            Assert.NotNull(retrievedDeployment);
            Assert.Equal(deploymentName, retrievedDeployment.Metadata.Name);
            Assert.Equal(namespaceParameter, retrievedDeployment.Metadata.NamespaceProperty);
            Assert.Equal("Deployment", retrievedDeployment.Kind);
        }
        finally
        {
            // Cleanup
            try
            {
                kubernetes.AppsV1.DeleteNamespacedDeployment(deploymentName, namespaceParameter);
            }
            catch
            {
                // Ignore cleanup errors
            }
        }
    }
}
