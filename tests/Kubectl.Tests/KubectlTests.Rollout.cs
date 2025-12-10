using k8s.Autorest;
using k8s.E2E;
using k8s.kubectl.beta;
using k8s.Models;
using Xunit;

namespace k8s.kubectl.Tests;

public partial class KubectlTests
{
    [MinikubeFact]
    public void RolloutRestartDeployment()
    {
        using var kubernetes = MinikubeTests.CreateClient();
        var client = new Kubectl(kubernetes);
        var namespaceParameter = "default";
        var deploymentName = "k8scsharp-e2e-rollout-deployment";

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
                        { "app", "test-rollout" },
                    },
                },
                Template = new V1PodTemplateSpec
                {
                    Metadata = new V1ObjectMeta
                    {
                        Labels = new Dictionary<string, string>
                        {
                            { "app", "test-rollout" },
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

            // Wait a moment for the deployment to stabilize
            System.Threading.Thread.Sleep(2000);

            // Restart the deployment
            client.RolloutRestartDeployment(deploymentName, namespaceParameter);

            // Verify the restart annotation was added
            var updatedDeployment = kubernetes.AppsV1.ReadNamespacedDeployment(deploymentName, namespaceParameter);
            Assert.NotNull(updatedDeployment.Spec.Template.Metadata.Annotations);
            Assert.Contains("kubectl.kubernetes.io/restartedAt", updatedDeployment.Spec.Template.Metadata.Annotations.Keys);
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

    [MinikubeFact]
    public void RolloutStatusDeployment()
    {
        using var kubernetes = MinikubeTests.CreateClient();
        var client = new Kubectl(kubernetes);
        var namespaceParameter = "default";
        var deploymentName = "k8scsharp-e2e-rollout-status";

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
                        { "app", "test-status" },
                    },
                },
                Template = new V1PodTemplateSpec
                {
                    Metadata = new V1ObjectMeta
                    {
                        Labels = new Dictionary<string, string>
                        {
                            { "app", "test-status" },
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

            // Get rollout status
            var status = client.RolloutStatusDeployment(deploymentName, namespaceParameter);

            // Status should contain the deployment name
            Assert.Contains(deploymentName, status);
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

    [MinikubeFact]
    public void RolloutPauseAndResumeDeployment()
    {
        using var kubernetes = MinikubeTests.CreateClient();
        var client = new Kubectl(kubernetes);
        var namespaceParameter = "default";
        var deploymentName = "k8scsharp-e2e-rollout-pause";

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
                        { "app", "test-pause" },
                    },
                },
                Template = new V1PodTemplateSpec
                {
                    Metadata = new V1ObjectMeta
                    {
                        Labels = new Dictionary<string, string>
                        {
                            { "app", "test-pause" },
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

            // Pause the deployment
            client.RolloutPauseDeployment(deploymentName, namespaceParameter);

            // Verify the deployment is paused
            var pausedDeployment = kubernetes.AppsV1.ReadNamespacedDeployment(deploymentName, namespaceParameter);
            Assert.True(pausedDeployment.Spec.Paused);

            // Resume the deployment
            client.RolloutResumeDeployment(deploymentName, namespaceParameter);

            // Verify the deployment is resumed
            var resumedDeployment = kubernetes.AppsV1.ReadNamespacedDeployment(deploymentName, namespaceParameter);
            Assert.False(resumedDeployment.Spec.Paused);
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

    [MinikubeFact]
    public void RolloutHistoryDeployment()
    {
        using var kubernetes = MinikubeTests.CreateClient();
        var client = new Kubectl(kubernetes);
        var namespaceParameter = "default";
        var deploymentName = "k8scsharp-e2e-rollout-history";

        // Create a test deployment with change-cause annotation
        var deployment = new V1Deployment
        {
            Metadata = new V1ObjectMeta
            {
                Name = deploymentName,
                NamespaceProperty = namespaceParameter,
                Annotations = new Dictionary<string, string>
                {
                    { "kubernetes.io/change-cause", "Initial deployment" },
                },
            },
            Spec = new V1DeploymentSpec
            {
                Replicas = 1,
                Selector = new V1LabelSelector
                {
                    MatchLabels = new Dictionary<string, string>
                    {
                        { "app", "test-history" },
                    },
                },
                Template = new V1PodTemplateSpec
                {
                    Metadata = new V1ObjectMeta
                    {
                        Labels = new Dictionary<string, string>
                        {
                            { "app", "test-history" },
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

            // Wait for deployment to create ReplicaSets
            System.Threading.Thread.Sleep(3000);

            // Get rollout history
            var history = client.RolloutHistoryDeployment(deploymentName, namespaceParameter);

            // Should have at least one revision
            Assert.NotNull(history);
            Assert.NotEmpty(history);
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

    [MinikubeFact]
    public void RolloutRestartDaemonSet()
    {
        using var kubernetes = MinikubeTests.CreateClient();
        var client = new Kubectl(kubernetes);
        var namespaceParameter = "default";
        var daemonSetName = "k8scsharp-e2e-rollout-daemonset";

        // Create a test daemonset
        var daemonSet = new V1DaemonSet
        {
            Metadata = new V1ObjectMeta
            {
                Name = daemonSetName,
                NamespaceProperty = namespaceParameter,
            },
            Spec = new V1DaemonSetSpec
            {
                Selector = new V1LabelSelector
                {
                    MatchLabels = new Dictionary<string, string>
                    {
                        { "app", "test-daemonset" },
                    },
                },
                Template = new V1PodTemplateSpec
                {
                    Metadata = new V1ObjectMeta
                    {
                        Labels = new Dictionary<string, string>
                        {
                            { "app", "test-daemonset" },
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
                        Tolerations = new[]
                        {
                            new V1Toleration
                            {
                                OperatorProperty = "Exists",
                            },
                        },
                    },
                },
            },
        };

        try
        {
            kubernetes.AppsV1.CreateNamespacedDaemonSet(daemonSet, namespaceParameter);

            // Wait a moment for the daemonset to stabilize
            System.Threading.Thread.Sleep(2000);

            // Restart the daemonset
            client.RolloutRestartDaemonSet(daemonSetName, namespaceParameter);

            // Verify the restart annotation was added
            var updatedDaemonSet = kubernetes.AppsV1.ReadNamespacedDaemonSet(daemonSetName, namespaceParameter);
            Assert.NotNull(updatedDaemonSet.Spec.Template.Metadata.Annotations);
            Assert.Contains("kubectl.kubernetes.io/restartedAt", updatedDaemonSet.Spec.Template.Metadata.Annotations.Keys);
        }
        finally
        {
            // Cleanup
            try
            {
                kubernetes.AppsV1.DeleteNamespacedDaemonSet(daemonSetName, namespaceParameter);
            }
            catch (HttpOperationException)
            {
                // Ignore cleanup errors
            }
        }
    }

    [MinikubeFact]
    public void RolloutStatusDaemonSet()
    {
        using var kubernetes = MinikubeTests.CreateClient();
        var client = new Kubectl(kubernetes);
        var namespaceParameter = "default";
        var daemonSetName = "k8scsharp-e2e-rollout-ds-status";

        // Create a test daemonset
        var daemonSet = new V1DaemonSet
        {
            Metadata = new V1ObjectMeta
            {
                Name = daemonSetName,
                NamespaceProperty = namespaceParameter,
            },
            Spec = new V1DaemonSetSpec
            {
                Selector = new V1LabelSelector
                {
                    MatchLabels = new Dictionary<string, string>
                    {
                        { "app", "test-ds-status" },
                    },
                },
                Template = new V1PodTemplateSpec
                {
                    Metadata = new V1ObjectMeta
                    {
                        Labels = new Dictionary<string, string>
                        {
                            { "app", "test-ds-status" },
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
                        Tolerations = new[]
                        {
                            new V1Toleration
                            {
                                OperatorProperty = "Exists",
                            },
                        },
                    },
                },
            },
        };

        try
        {
            kubernetes.AppsV1.CreateNamespacedDaemonSet(daemonSet, namespaceParameter);

            // Get rollout status
            var status = client.RolloutStatusDaemonSet(daemonSetName, namespaceParameter);

            // Status should contain the daemonset name
            Assert.Contains(daemonSetName, status);
        }
        finally
        {
            // Cleanup
            try
            {
                kubernetes.AppsV1.DeleteNamespacedDaemonSet(daemonSetName, namespaceParameter);
            }
            catch (HttpOperationException)
            {
                // Ignore cleanup errors
            }
        }
    }

    [MinikubeFact]
    public void RolloutRestartStatefulSet()
    {
        using var kubernetes = MinikubeTests.CreateClient();
        var client = new Kubectl(kubernetes);
        var namespaceParameter = "default";
        var statefulSetName = "k8scsharp-e2e-rollout-statefulset";

        // Create a test statefulset
        var statefulSet = new V1StatefulSet
        {
            Metadata = new V1ObjectMeta
            {
                Name = statefulSetName,
                NamespaceProperty = namespaceParameter,
            },
            Spec = new V1StatefulSetSpec
            {
                ServiceName = "test-service",
                Replicas = 1,
                Selector = new V1LabelSelector
                {
                    MatchLabels = new Dictionary<string, string>
                    {
                        { "app", "test-statefulset" },
                    },
                },
                Template = new V1PodTemplateSpec
                {
                    Metadata = new V1ObjectMeta
                    {
                        Labels = new Dictionary<string, string>
                        {
                            { "app", "test-statefulset" },
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
            kubernetes.AppsV1.CreateNamespacedStatefulSet(statefulSet, namespaceParameter);

            // Wait a moment for the statefulset to stabilize
            System.Threading.Thread.Sleep(2000);

            // Restart the statefulset
            client.RolloutRestartStatefulSet(statefulSetName, namespaceParameter);

            // Verify the restart annotation was added
            var updatedStatefulSet = kubernetes.AppsV1.ReadNamespacedStatefulSet(statefulSetName, namespaceParameter);
            Assert.NotNull(updatedStatefulSet.Spec.Template.Metadata.Annotations);
            Assert.Contains("kubectl.kubernetes.io/restartedAt", updatedStatefulSet.Spec.Template.Metadata.Annotations.Keys);
        }
        finally
        {
            // Cleanup
            try
            {
                kubernetes.AppsV1.DeleteNamespacedStatefulSet(statefulSetName, namespaceParameter);
            }
            catch (HttpOperationException)
            {
                // Ignore cleanup errors
            }
        }
    }

    [MinikubeFact]
    public void RolloutStatusStatefulSet()
    {
        using var kubernetes = MinikubeTests.CreateClient();
        var client = new Kubectl(kubernetes);
        var namespaceParameter = "default";
        var statefulSetName = "k8scsharp-e2e-rollout-sts-status";

        // Create a test statefulset
        var statefulSet = new V1StatefulSet
        {
            Metadata = new V1ObjectMeta
            {
                Name = statefulSetName,
                NamespaceProperty = namespaceParameter,
            },
            Spec = new V1StatefulSetSpec
            {
                ServiceName = "test-service",
                Replicas = 1,
                Selector = new V1LabelSelector
                {
                    MatchLabels = new Dictionary<string, string>
                    {
                        { "app", "test-sts-status" },
                    },
                },
                Template = new V1PodTemplateSpec
                {
                    Metadata = new V1ObjectMeta
                    {
                        Labels = new Dictionary<string, string>
                        {
                            { "app", "test-sts-status" },
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
            kubernetes.AppsV1.CreateNamespacedStatefulSet(statefulSet, namespaceParameter);

            // Get rollout status
            var status = client.RolloutStatusStatefulSet(statefulSetName, namespaceParameter);

            // Status should contain the statefulset name or status information
            Assert.NotNull(status);
            Assert.NotEmpty(status);
        }
        finally
        {
            // Cleanup
            try
            {
                kubernetes.AppsV1.DeleteNamespacedStatefulSet(statefulSetName, namespaceParameter);
            }
            catch (HttpOperationException)
            {
                // Ignore cleanup errors
            }
        }
    }
}
