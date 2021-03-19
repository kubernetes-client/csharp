using System;
using System.Net.Http;
using System.Threading;
using k8s;
using k8s.Monitoring;
using Prometheus;

namespace prom
{
    internal class Prometheus
    {
        private static void Main(string[] args)
        {
            var config = KubernetesClientConfiguration.BuildDefaultConfig();
            var handler = new PrometheusHandler();
            IKubernetes client = new Kubernetes(config, new DelegatingHandler[] { handler });

            var server = new MetricServer(hostname: "localhost", port: 1234);
            server.Start();

            Console.WriteLine("Making requests!");
            while (true)
            {
                client.ListNamespacedPod("default");
                client.ListNode();
                client.ListNamespacedDeployment("default");
                Thread.Sleep(1000);
            }
        }
    }
}
