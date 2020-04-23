using System;
using System.Collections.Generic;
using k8s;

namespace simple
{
    internal class PodList
    {
        private static void Main(string[] args)
        {
            var config = KubernetesClientConfiguration.BuildConfigFromConfigFile();
            IKubernetes client = new Kubernetes(config);
            Console.WriteLine("Starting Request!");

            var list = client.ListNamespacedService("default");
            foreach (var item in list.Items)
            {
                Console.WriteLine("Pods for service: " + item.Metadata.Name);
                Console.WriteLine("=-=-=-=-=-=-=-=-=-=-=");
                if (item.Spec == null || item.Spec.Selector == null)
                {
                    continue;
                }

                var labels = new List<string>();
                foreach (var key in item.Spec.Selector)
                {
                    labels.Add(key.Key + "=" + key.Value);
                }

                var labelStr = string.Join(",", labels.ToArray());
                Console.WriteLine(labelStr);
                var podList = client.ListNamespacedPod("default", labelSelector: labelStr);
                foreach (var pod in podList.Items)
                {
                    Console.WriteLine(pod.Metadata.Name);
                }

                if (podList.Items.Count == 0)
                {
                    Console.WriteLine("Empty!");
                }

                Console.WriteLine();
            }
        }
    }
}
