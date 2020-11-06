using System;
using System.Linq;
using k8s;
using k8s.Models;

namespace patch
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            var config = KubernetesClientConfiguration.BuildConfigFromConfigFile();
            IKubernetes client = new Kubernetes(config);
            Console.WriteLine("Starting Request!");

            var pod = client.ListNamespacedPod("default").Items.First();
            var name = pod.Metadata.Name;
            PrintLabels(pod);

            var patchStr = @"
{
    ""metadata"": {
        ""labels"": {
            ""test"": ""test""
        }
    }
}";

            client.PatchNamespacedPod(new V1Patch(patchStr, V1Patch.PatchType.MergePatch), name, "default");
            PrintLabels(client.ReadNamespacedPod(name, "default"));
        }

        private static void PrintLabels(V1Pod pod)
        {
            Console.WriteLine($"Labels: for {pod.Metadata.Name}");
            foreach (var (k, v) in pod.Metadata.Labels)
            {
                Console.WriteLine($"{k} : {v}");
            }

            Console.WriteLine("=-=-=-=-=-=-=-=-=-=-=");
        }
    }
}
