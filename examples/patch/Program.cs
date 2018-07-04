using System;
using System.Collections.Generic;
using System.Linq;
using k8s;
using k8s.Models;
using Microsoft.AspNetCore.JsonPatch;

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

            var newlables = new Dictionary<string, string>(pod.Metadata.Labels)
            {
                ["test"] = "test"
            };
            var patch = new JsonPatchDocument<V1Pod>();
            patch.Replace(e => e.Metadata.Labels, newlables);
            client.PatchNamespacedPod(new V1Patch(patch), name, "default");

            PrintLabels(client.ReadNamespacedPod(name, "default"));
        }

        private static void PrintLabels(V1Pod pod)
        {
            Console.WriteLine($"Lables: for {pod.Metadata.Name}");
            foreach (var (k, v) in pod.Metadata.Labels)
            {
                Console.WriteLine($"{k} : {v}");
            }
            Console.WriteLine("=-=-=-=-=-=-=-=-=-=-=");
        }
    }
}
