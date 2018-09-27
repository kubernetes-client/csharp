using System;
using System.Threading;
using k8s;
using k8s.Models;

namespace watch
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            var config = KubernetesClientConfiguration.BuildConfigFromConfigFile();

            IKubernetes client = new Kubernetes(config);

            var podlistResp = client.ListNamespacedPodWithHttpMessagesAsync("default", watch: true).Result;
            using (podlistResp.Watch<V1Pod>((type, item) =>
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
