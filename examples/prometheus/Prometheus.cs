using k8s;
using Prometheus;
using System;
using System.Threading;

namespace prom
{
    internal class Prometheus
    {
        private static void Main(string[] args)
        {
            var config = KubernetesClientConfiguration.BuildDefaultConfig();
            var handler = new PrometheusHandler();
            IKubernetes client = new Kubernetes(config, configure: null, handler);

            var server = new MetricServer(hostname: "localhost", port: 1234);
            server.Start();

            Console.WriteLine("Making requests!");
            while (true)
            {
                client.CoreV1.ListNamespacedPod("default");
                client.CoreV1.ListNode();
                client.AppsV1.ListNamespacedDeployment("default");
                Thread.Sleep(1000);
            }
        }
    }
}
