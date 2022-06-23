using System;
using System.Threading.Tasks;
using k8s;

namespace logs
{
    internal class Logs
    {
        private static async Task Main(string[] args)
        {
            var config = KubernetesClientConfiguration.BuildConfigFromConfigFile();
            IKubernetes client = new Kubernetes(config);
            Console.WriteLine("Starting Request!");

            var list = client.CoreV1.ListNamespacedPod("default");
            if (list.Items.Count == 0)
            {
                Console.WriteLine("No pods!");
                return;
            }

            var pod = list.Items[0];

            var response = await client.CoreV1.ReadNamespacedPodLogWithHttpMessagesAsync(
                pod.Metadata.Name,
                pod.Metadata.NamespaceProperty, container: pod.Spec.Containers[0].Name, follow: true).ConfigureAwait(false);
            var stream = response.Body;
            stream.CopyTo(Console.OpenStandardOutput());
        }
    }
}
