using System;
using System.IO;
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

            var list = client.ListNamespacedPod("default");
            if (list.Items.Count == 0)
            {
                Console.WriteLine("No pods!");
                return;
            }

            var pod = list.Items[0];

            var response = await client.ReadNamespacedPodLogWithHttpMessagesAsync(pod.Metadata.Name,
                pod.Metadata.NamespaceProperty, follow: true);
            var stream = response.Body;
            stream.CopyTo(Console.OpenStandardOutput());
        }
    }
}
