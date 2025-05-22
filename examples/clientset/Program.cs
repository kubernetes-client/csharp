// See https://aka.ms/new-console-template for more information
using k8s;
using k8s.ClientSets;
using System.Threading.Tasks;

namespace clientset
{
    internal class Program
    {
        private static async Task Main(string[] args)
        {
            var config = KubernetesClientConfiguration.BuildConfigFromConfigFile();
            var client = new Kubernetes(config);

            ClientSet clientSet = new ClientSet(client);
            var list = await clientSet.CoreV1.Pod.ListAsync("default").ConfigureAwait(false);
            foreach (var item in list)
            {
                System.Console.WriteLine(item.Metadata.Name);
            }

            var pod = await clientSet.CoreV1.Pod.GetAsync("test","default").ConfigureAwait(false);
            System.Console.WriteLine(pod?.Metadata?.Name);
        }
    }
}
