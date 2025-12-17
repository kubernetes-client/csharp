using k8s.Models;

namespace k8s.kubectl.beta;

public partial class Kubectl
{
    /// <summary>
    /// Get top nodes sorted by CPU or memory usage.
    /// </summary>
    /// <param name="metric">The metric to sort by ("cpu" or "memory"). Defaults to "cpu".</param>
    /// <returns>A list of nodes with their metrics, sorted by the specified metric in descending order.</returns>
    public List<AsyncKubectl.ResourceMetrics<V1Node, NodeMetrics>> TopNodes(string metric = "cpu")
    {
        return client.TopNodesAsync(metric).GetAwaiter().GetResult();
    }

    /// <summary>
    /// Get top pods in a namespace sorted by CPU or memory usage.
    /// </summary>
    /// <param name="namespace">The namespace to get pod metrics from.</param>
    /// <param name="metric">The metric to sort by ("cpu" or "memory"). Defaults to "cpu".</param>
    /// <returns>A list of pods with their metrics, sorted by the specified metric in descending order.</returns>
    public List<AsyncKubectl.ResourceMetrics<V1Pod, PodMetrics>> TopPods(string @namespace, string metric = "cpu")
    {
        return client.TopPodsAsync(@namespace, metric).GetAwaiter().GetResult();
    }
}
