using System;
using System.Threading.Tasks;
using k8s;
using k8s.Models;

namespace exec
{
    internal class Generic
    {
        private static async Task Main(string[] args)
        {
            var config = KubernetesClientConfiguration.BuildConfigFromConfigFile();
            var generic = new GenericClient(config, "", "v1", "nodes");
            var node = await generic.ReadAsync<V1Node>("kube0").ConfigureAwait(false);
            Console.WriteLine(node.Metadata.Name);

            var genericPods = new GenericClient(config, "", "v1", "pods");
            var pods = await genericPods.ListNamespacedAsync<V1PodList>("default").ConfigureAwait(false);
            foreach (var pod in pods.Items)
            {
                Console.WriteLine(pod.Metadata.Name);
            }
        }
    }
}
