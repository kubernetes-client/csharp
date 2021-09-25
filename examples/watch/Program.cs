using System;
using System.Threading;
using System.Threading.Tasks;
using k8s;
using k8s.Models;
using Microsoft.Rest;

namespace watch
{
    internal class Program
    {
        private async static Task Main(string[] args)
        {
            var config = KubernetesClientConfiguration.BuildConfigFromConfigFile();

            IKubernetes client = new Kubernetes(config);

            var podlistResp = client.ListNamespacedPodWithHttpMessagesAsync("default", watch: true);
            // C# 8 required https://docs.microsoft.com/en-us/archive/msdn-magazine/2019/november/csharp-iterating-with-async-enumerables-in-csharp-8
            await foreach (var (type, item) in podlistResp.WatchAsync<V1Pod, V1PodList>())
            {
                Console.WriteLine("==on watch event==");
                Console.WriteLine(type);
                Console.WriteLine(item.Metadata.Name);
                Console.WriteLine("==on watch event==");
            }

            // uncomment if you prefer callback api
            // WatchUsingCallback(client);
        }

        private static void WatchUsingCallback(IKubernetes client)
        {
            var podlistResp = client.ListNamespacedPodWithHttpMessagesAsync("default", watch: true);
            using (podlistResp.Watch<V1Pod, V1PodList>((type, item) =>
            {
                Console.WriteLine("==on watch event==");
                Console.WriteLine(type);
                Console.WriteLine(item.Metadata.Name);
                Console.WriteLine("==on watch event==");
            }))
            {
                Console.WriteLine("press ctrl + c to stop watching");

                var ctrlc = new ManualResetEventSlim(false);
                Console.CancelKeyPress += (sender, eventArgs) => ctrlc.Set();
                ctrlc.Wait();
            }
        }
    }
}
