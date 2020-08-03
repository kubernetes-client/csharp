using k8s;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace metrics
{
    class Program
    {
        static async Task NodesMetrics(IKubernetes client)
        {
            var nodesMetrics = await client.GetKubernetesNodesMetricsAsync().ConfigureAwait(false);

            foreach (var item in nodesMetrics.Items)
            {
                Console.WriteLine(item.Metadata.Name);

                foreach (var metric in item.Usage)
                {
                    Console.WriteLine($"{metric.Key}: {metric.Value}");
                }
            }
        }

        static async Task PodsMetrics(IKubernetes client)
        {
            var podsMetrics = await client.GetKubernetesPodsMetricsAsync().ConfigureAwait(false);

            if (!podsMetrics.Items.Any())
            {
                Console.WriteLine("Empty");
            }

            foreach (var item in podsMetrics.Items)
            {
                foreach (var container in item.Containers)
                {
                    Console.WriteLine(container.Name);

                    foreach (var metric in container.Usage)
                    {
                        Console.WriteLine($"{metric.Key}: {metric.Value}");
                    }
                }
                Console.Write(Environment.NewLine);
            }
        }

        static async Task Main(string[] args)
        {
            var config = KubernetesClientConfiguration.BuildConfigFromConfigFile();
            var client = new Kubernetes(config);

            await NodesMetrics(client).ConfigureAwait(false);
            Console.WriteLine(Environment.NewLine);
            await PodsMetrics(client).ConfigureAwait(false);
        }
    }
}
