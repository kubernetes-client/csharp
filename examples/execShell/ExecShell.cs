using System;
using System.IO;
using System.Threading.Tasks;
using k8s;
using k8s.Models;

namespace execShell
{
    internal class ExecShell
    {
        private static async Task Main(string[] args)
        {
            var config = KubernetesClientConfiguration.BuildConfigFromConfigFile();
            IKubernetes client = new Kubernetes(config);
            Console.WriteLine("Starting Request!");

            var list = client.ListNamespacedPod("default");
            var pod = list.Items[0];
            await ExecInPod(client, pod).ConfigureAwait(false);
        }

        private async static Task ExecInPod(IKubernetes client, V1Pod pod)
        {
            var webSocket =
               await client.WebSocketNamespacedPodExecAsync(pod.Metadata.Name, "default", "/bin/bash",
                   pod.Spec.Containers[0].Name).ConfigureAwait(false);

            var demux = new StreamDemuxer(webSocket);
            demux.Start();

            var stream = demux.GetStream(ChannelIndex.StdOut, ChannelIndex.StdIn);

            _ = Console.OpenStandardInput().CopyToAsync(stream).ConfigureAwait(false);

            using var reader = new StreamReader(stream);
            while (!reader.EndOfStream)
            {
                Console.WriteLine(reader.ReadLine());
            }
        }
    }
}
