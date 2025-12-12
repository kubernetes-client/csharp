using k8s.E2E;
using k8s.kubectl.beta;
using k8s.Models;
using Xunit;

namespace k8s.kubectl.Tests;

public partial class KubectlTests
{
    [MinikubeFact]
    public void TopNodes()
    {
        using var kubernetes = MinikubeTests.CreateClient();
        var client = new Kubectl(kubernetes);

        // Get top nodes sorted by CPU
        var topNodes = client.TopNodes("cpu");

        Assert.NotNull(topNodes);
        Assert.NotEmpty(topNodes);

        // Verify all nodes have both resource and metrics
        foreach (var item in topNodes)
        {
            Assert.NotNull(item.Resource);
            Assert.NotNull(item.Metrics);
            Assert.NotNull(item.Resource.Metadata);
            Assert.NotNull(item.Metrics.Usage);
            Assert.True(item.Metrics.Usage.ContainsKey("cpu"));
            Assert.True(item.Metrics.Usage.ContainsKey("memory"));
        }

        // Verify sorting - each node should have CPU <= the previous node
        for (int i = 1; i < topNodes.Count; i++)
        {
            var prevPercentage = CalculateNodeCpuPercentage(topNodes[i - 1]);
            var currPercentage = CalculateNodeCpuPercentage(topNodes[i]);
            Assert.True(
                prevPercentage >= currPercentage,
                $"Nodes should be sorted by CPU in descending order. Previous: {prevPercentage}, Current: {currPercentage}");
        }
    }

    [MinikubeFact]
    public void TopNodesMemory()
    {
        using var kubernetes = MinikubeTests.CreateClient();
        var client = new Kubectl(kubernetes);

        // Get top nodes sorted by memory
        var topNodes = client.TopNodes("memory");

        Assert.NotNull(topNodes);
        Assert.NotEmpty(topNodes);

        // Verify all nodes have both resource and metrics
        foreach (var item in topNodes)
        {
            Assert.NotNull(item.Resource);
            Assert.NotNull(item.Metrics);
            Assert.NotNull(item.Resource.Metadata);
            Assert.NotNull(item.Metrics.Usage);
            Assert.True(item.Metrics.Usage.ContainsKey("memory"));
        }

        // Verify sorting - each node should have memory <= the previous node
        for (int i = 1; i < topNodes.Count; i++)
        {
            var prevPercentage = CalculateNodeMemoryPercentage(topNodes[i - 1]);
            var currPercentage = CalculateNodeMemoryPercentage(topNodes[i]);
            Assert.True(
                prevPercentage >= currPercentage,
                $"Nodes should be sorted by memory in descending order. Previous: {prevPercentage}, Current: {currPercentage}");
        }
    }

    [MinikubeFact]
    public void TopNodesInvalidMetric()
    {
        using var kubernetes = MinikubeTests.CreateClient();
        var client = new Kubectl(kubernetes);

        // Attempting to use an invalid metric should throw
        Assert.Throws<ArgumentException>(() => client.TopNodes("invalid"));
    }

    [MinikubeFact]
    public void TopPods()
    {
        using var kubernetes = MinikubeTests.CreateClient();
        var client = new Kubectl(kubernetes);
        var namespaceParameter = "kube-system";

        // Get top pods sorted by CPU
        var topPods = client.TopPods(namespaceParameter, "cpu");

        Assert.NotNull(topPods);
        // Note: kube-system should always have some pods
        Assert.NotEmpty(topPods);

        // Verify all pods have both resource and metrics
        foreach (var item in topPods)
        {
            Assert.NotNull(item.Resource);
            Assert.NotNull(item.Metrics);
            Assert.NotNull(item.Resource.Metadata);
            Assert.NotNull(item.Metrics.Containers);
            Assert.NotEmpty(item.Metrics.Containers);

            // Verify all containers have CPU metrics
            foreach (var container in item.Metrics.Containers)
            {
                Assert.NotNull(container.Usage);
                Assert.True(container.Usage.ContainsKey("cpu"));
            }
        }

        // Verify sorting - each pod should have CPU sum <= the previous pod
        for (int i = 1; i < topPods.Count; i++)
        {
            var prevSum = CalculatePodCpuSum(topPods[i - 1].Metrics);
            var currSum = CalculatePodCpuSum(topPods[i].Metrics);
            Assert.True(
                prevSum >= currSum,
                $"Pods should be sorted by CPU in descending order. Previous: {prevSum}, Current: {currSum}");
        }
    }

    [MinikubeFact]
    public void TopPodsMemory()
    {
        using var kubernetes = MinikubeTests.CreateClient();
        var client = new Kubectl(kubernetes);
        var namespaceParameter = "kube-system";

        // Get top pods sorted by memory
        var topPods = client.TopPods(namespaceParameter, "memory");

        Assert.NotNull(topPods);
        Assert.NotEmpty(topPods);

        // Verify all pods have memory metrics
        foreach (var item in topPods)
        {
            Assert.NotNull(item.Metrics.Containers);
            Assert.NotEmpty(item.Metrics.Containers);

            // Verify all containers have memory metrics
            foreach (var container in item.Metrics.Containers)
            {
                Assert.NotNull(container.Usage);
                Assert.True(container.Usage.ContainsKey("memory"));
            }
        }

        // Verify sorting - each pod should have memory sum <= the previous pod
        for (int i = 1; i < topPods.Count; i++)
        {
            var prevSum = CalculatePodMemorySum(topPods[i - 1].Metrics);
            var currSum = CalculatePodMemorySum(topPods[i].Metrics);
            Assert.True(
                prevSum >= currSum,
                $"Pods should be sorted by memory in descending order. Previous: {prevSum}, Current: {currSum}");
        }
    }

    [MinikubeFact]
    public void TopPodsInvalidMetric()
    {
        using var kubernetes = MinikubeTests.CreateClient();
        var client = new Kubectl(kubernetes);

        // Attempting to use an invalid metric should throw
        Assert.Throws<ArgumentException>(() => client.TopPods("default", "invalid"));
    }

    [MinikubeFact]
    public void TopPodsEmptyNamespace()
    {
        using var kubernetes = MinikubeTests.CreateClient();
        var client = new Kubectl(kubernetes);

        // Attempting to use an empty namespace should throw
        Assert.Throws<ArgumentException>(() => client.TopPods(string.Empty, "cpu"));
#pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference type.
        Assert.Throws<ArgumentException>(() => client.TopPods(null, "cpu"));
#pragma warning restore CS8625 // Cannot convert null literal to non-nullable reference type.
    }

    private static double CalculateNodeCpuPercentage(AsyncKubectl.ResourceMetrics<V1Node, NodeMetrics> nodeMetrics)
    {
        if (!nodeMetrics.Metrics.Usage.TryGetValue("cpu", out var usage))
        {
            return 0;
        }

        if (!nodeMetrics.Resource.Status.Capacity.TryGetValue("cpu", out var capacity))
        {
            return double.PositiveInfinity;
        }

        return usage.ToDouble() / capacity.ToDouble();
    }

    private static double CalculateNodeMemoryPercentage(AsyncKubectl.ResourceMetrics<V1Node, NodeMetrics> nodeMetrics)
    {
        if (!nodeMetrics.Metrics.Usage.TryGetValue("memory", out var usage))
        {
            return 0;
        }

        if (!nodeMetrics.Resource.Status.Capacity.TryGetValue("memory", out var capacity))
        {
            return double.PositiveInfinity;
        }

        return usage.ToDouble() / capacity.ToDouble();
    }

    private static double CalculatePodCpuSum(PodMetrics podMetrics)
    {
        double sum = 0;
        foreach (var container in podMetrics.Containers)
        {
            if (container.Usage.TryGetValue("cpu", out var value))
            {
                sum += value.ToDouble();
            }
        }

        return sum;
    }

    private static double CalculatePodMemorySum(PodMetrics podMetrics)
    {
        double sum = 0;
        foreach (var container in podMetrics.Containers)
        {
            if (container.Usage.TryGetValue("memory", out var value))
            {
                sum += value.ToDouble();
            }
        }

        return sum;
    }
}
