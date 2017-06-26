using System;
using Xunit;
using k8s;

namespace k8s.Tests
{
    public class KubernetesClientConfigurationTests
    {
        /// <summary>
        /// Checks Host is loaded from the default configuration file
        /// </summary>
        [Fact]
        public void DefaultConfigurationLoaded()
        {
            var cfg = new KubernetesClientConfiguration();
            Assert.NotNull(cfg.Host);
        } 

        /// <summary>
        /// Checks if the are pods
        /// </summary>
        [Fact]
        public void ListDefaultNamespacedPod()
        {
            var k8sClientConfig = new KubernetesClientConfiguration();
            IKubernetes client = new Kubernetes(k8sClientConfig);
            var listTask = client.ListNamespacedPodWithHttpMessagesAsync("default").Result;
            var list = listTask.Body;
            Assert.NotEqual(0, list.Items.Count);            
        }
    }
}
