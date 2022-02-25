using System;
using System.Threading.Tasks;
using k8s;
using k8s.Models;
using k8s.Autorest;

namespace attach
{
    internal class Attach
    {
        private static async Task Main(string[] args)
        {
            var config = KubernetesClientConfiguration.BuildConfigFromConfigFile();
            IKubernetes client = new Kubernetes(config);
            Console.WriteLine("Starting Request!");

            var list = client.ListNamespacedPod("default");
            var pod = list.Items[0];
            await AttachToPod(client, pod).ConfigureAwait(false);
        }

        private async static Task AttachToPod(IKubernetes client, V1Pod pod)
        {
            var webSocket =
                await client.WebSocketNamespacedPodAttachAsync(pod.Metadata.Name, "default",
                    pod.Spec.Containers[0].Name).ConfigureAwait(false);

            var demux = new StreamDemuxer(webSocket);
            demux.Start();

            var buff = new byte[4096];
            var stream = demux.GetStream(1, 1);
            while (true)
            {
                var read = stream.Read(buff, 0, 4096);
                var str = System.Text.Encoding.Default.GetString(buff);
                Console.WriteLine(str);
            }
        }
    }
}
