using k8s;
using k8s.kubectl.beta;
using System;
using System.Linq;
using System.Threading.Tasks;

async Task NodesMetrics(IKubernetes client)
{
    var nodesMetrics = await client.GetKubernetesNodesMetricsAsync().ConfigureAwait(false);

    foreach (var item in nodesMetrics.Items)
    {
        Console.WriteLine(item.Metadata.Name);

        foreach (var metric in item.Usage)
        {
            Console.WriteLine($"{metric.Key}: {metric.Value}");
        }
    }
}

async Task PodsMetrics(IKubernetes client)
{
    var podsMetrics = await client.GetKubernetesPodsMetricsAsync().ConfigureAwait(false);

    if (!podsMetrics.Items.Any())
    {
        Console.WriteLine("Empty");
    }

    foreach (var item in podsMetrics.Items)
    {
        foreach (var container in item.Containers)
        {
            Console.WriteLine(container.Name);

            foreach (var metric in container.Usage)
            {
                Console.WriteLine($"{metric.Key}: {metric.Value}");
            }
        }

        Console.Write(Environment.NewLine);
    }
}

async Task TopNodes(Kubectl kubectl)
{
    Console.WriteLine("=== Top Nodes by CPU ===");
    var topNodesCpu = kubectl.TopNodes("cpu");

    // Show top 5
    var topNodesCpuList = topNodesCpu.Take(5).ToList();
    foreach (var item in topNodesCpuList)
    {
        var cpuUsage = item.Metrics.Usage["cpu"];
        var cpuCapacity = item.Resource.Status.Capacity["cpu"];
        var cpuPercent = (cpuUsage.ToDouble() / cpuCapacity.ToDouble()) * 100;

        Console.WriteLine($"{item.Resource.Metadata.Name}: CPU {cpuUsage} / {cpuCapacity} ({cpuPercent:F2}%)");
    }

    Console.WriteLine(Environment.NewLine);
    Console.WriteLine("=== Top Nodes by Memory ===");
    var topNodesMemory = kubectl.TopNodes("memory");

    // Show top 5
    var topNodesMemoryList = topNodesMemory.Take(5).ToList();
    foreach (var item in topNodesMemoryList)
    {
        var memUsage = item.Metrics.Usage["memory"];
        var memCapacity = item.Resource.Status.Capacity["memory"];
        var memPercent = (memUsage.ToDouble() / memCapacity.ToDouble()) * 100;

        Console.WriteLine($"{item.Resource.Metadata.Name}: Memory {memUsage} / {memCapacity} ({memPercent:F2}%)");
    }
}

async Task TopPods(Kubectl kubectl)
{
    Console.WriteLine("=== Top Pods by CPU (kube-system namespace) ===");
    var topPodsCpu = kubectl.TopPods("kube-system", "cpu");

    // Show top 5
    var topPodsCpuList = topPodsCpu.Take(5).ToList();
    foreach (var item in topPodsCpuList)
    {
        var cpuSum = item.Metrics.Containers.Sum(c =>
            c.Usage.ContainsKey("cpu") ? c.Usage["cpu"].ToDouble() : 0);

        Console.WriteLine($"{item.Resource.Metadata.Name}: CPU {cpuSum:F3} cores");
    }

    Console.WriteLine(Environment.NewLine);
    Console.WriteLine("=== Top Pods by Memory (kube-system namespace) ===");
    var topPodsMemory = kubectl.TopPods("kube-system", "memory");

    // Show top 5
    var topPodsMemoryList = topPodsMemory.Take(5).ToList();
    foreach (var item in topPodsMemoryList)
    {
        var memSum = item.Metrics.Containers.Sum(c =>
            c.Usage.ContainsKey("memory") ? c.Usage["memory"].ToDouble() : 0);

        Console.WriteLine($"{item.Resource.Metadata.Name}: Memory {memSum / (1024 * 1024):F2} MiB");
    }
}

var config = KubernetesClientConfiguration.BuildConfigFromConfigFile();
var client = new Kubernetes(config);
var kubectl = new Kubectl(client);

Console.WriteLine("=== Raw Metrics API ===");
await NodesMetrics(client).ConfigureAwait(false);
Console.WriteLine(Environment.NewLine);
await PodsMetrics(client).ConfigureAwait(false);

Console.WriteLine(Environment.NewLine);
Console.WriteLine("=== Kubectl Top API ===");
await TopNodes(kubectl).ConfigureAwait(false);
Console.WriteLine(Environment.NewLine);
await TopPods(kubectl).ConfigureAwait(false);
