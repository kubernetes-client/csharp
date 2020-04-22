using System;
using System.Threading.Tasks;
using k8s;
using k8s.Models;

namespace exec
{
    internal class Exec
    {
        private static async Task Main(string[] args)
        {
            var config = KubernetesClientConfiguration.BuildConfigFromConfigFile();
            IKubernetes client = new Kubernetes(config);
            Console.WriteLine("Starting Request!");

            var list = client.ListNamespacedPod("default");
            var pod = list.Items[0];
            await ExecInPod(client, pod);
        }

        private async static Task ExecInPod(IKubernetes client, V1Pod pod)
        {
            var webSocket = await client.WebSocketNamespacedPodExecAsync(pod.Metadata.Name, "default", "ls", pod.Spec.Containers[0].Name);

            var demux = new StreamDemuxer(webSocket);
            demux.Start();

            var buff = new byte[4096];
            var stream = demux.GetStream(1, 1);
            var read = stream.Read(buff, 0, 4096);
            var str = System.Text.Encoding.Default.GetString(buff);
            Console.WriteLine(str);
        }
    }
}
