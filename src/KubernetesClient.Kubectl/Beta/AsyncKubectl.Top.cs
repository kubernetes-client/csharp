using k8s.Models;

namespace k8s.kubectl.beta;

public partial class AsyncKubectl
{
    /// <summary>
    /// Describes a pair of Kubernetes resource and its metrics.
    /// </summary>
    /// <typeparam name="TResource">The type of Kubernetes resource.</typeparam>
    /// <typeparam name="TMetrics">The type of metrics.</typeparam>
    public class ResourceMetrics<TResource, TMetrics>
    {
        /// <summary>
        /// Gets or sets the Kubernetes resource.
        /// </summary>
        public required TResource Resource { get; set; }

        /// <summary>
        /// Gets or sets the metrics for the resource.
        /// </summary>
        public required TMetrics Metrics { get; set; }
    }

    /// <summary>
    /// Get top nodes sorted by CPU or memory usage.
    /// </summary>
    /// <param name="metric">The metric to sort by ("cpu" or "memory"). Defaults to "cpu".</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A list of nodes with their metrics, sorted by the specified metric in descending order.</returns>
    public async Task<List<ResourceMetrics<V1Node, NodeMetrics>>> TopNodesAsync(string metric = "cpu", CancellationToken cancellationToken = default)
    {
        if (metric != "cpu" && metric != "memory")
        {
            throw new ArgumentException("Metric must be either 'cpu' or 'memory'", nameof(metric));
        }

        // Get all nodes
        var nodes = await client.CoreV1.ListNodeAsync(cancellationToken: cancellationToken).ConfigureAwait(false);

        // Get node metrics
        var nodeMetrics = await client.GetKubernetesNodesMetricsAsync().ConfigureAwait(false);

        // Create a dictionary for quick lookup of metrics by node name
        var metricsDict = nodeMetrics.Items.ToDictionary(m => m.Metadata.Name, m => m);

        // Combine nodes with their metrics and calculate percentage
        var result = new List<ResourceMetrics<V1Node, NodeMetrics>>();
        foreach (var node in nodes.Items)
        {
            if (metricsDict.TryGetValue(node.Metadata.Name, out var metrics))
            {
                result.Add(new ResourceMetrics<V1Node, NodeMetrics>
                {
                    Resource = node,
                    Metrics = metrics,
                });
            }
        }

        // Sort by metric value (percentage of capacity) in descending order
        result.Sort((a, b) =>
        {
            var percentageA = CalculateNodePercentage(a.Resource, a.Metrics, metric);
            var percentageB = CalculateNodePercentage(b.Resource, b.Metrics, metric);
            return percentageB.CompareTo(percentageA); // Descending order
        });

        return result;
    }

    /// <summary>
    /// Get top pods in a namespace sorted by CPU or memory usage.
    /// </summary>
    /// <param name="namespace">The namespace to get pod metrics from.</param>
    /// <param name="metric">The metric to sort by ("cpu" or "memory"). Defaults to "cpu".</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A list of pods with their metrics, sorted by the specified metric in descending order.</returns>
    public async Task<List<ResourceMetrics<V1Pod, PodMetrics>>> TopPodsAsync(string @namespace, string metric = "cpu", CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrEmpty(@namespace))
        {
            throw new ArgumentException("Namespace cannot be null or empty", nameof(@namespace));
        }

        if (metric != "cpu" && metric != "memory")
        {
            throw new ArgumentException("Metric must be either 'cpu' or 'memory'", nameof(metric));
        }

        // Get all pods in the namespace
        var pods = await client.CoreV1.ListNamespacedPodAsync(@namespace, cancellationToken: cancellationToken).ConfigureAwait(false);

        // Get pod metrics for the namespace
        var podMetrics = await client.GetKubernetesPodsMetricsByNamespaceAsync(@namespace).ConfigureAwait(false);

        // Create a dictionary for quick lookup of metrics by pod name
        var metricsDict = podMetrics.Items.ToDictionary(m => m.Metadata.Name, m => m);

        // Combine pods with their metrics
        var result = new List<ResourceMetrics<V1Pod, PodMetrics>>();
        foreach (var pod in pods.Items)
        {
            if (metricsDict.TryGetValue(pod.Metadata.Name, out var metrics))
            {
                result.Add(new ResourceMetrics<V1Pod, PodMetrics>
                {
                    Resource = pod,
                    Metrics = metrics,
                });
            }
        }

        // Sort by metric value (sum across all containers) in descending order
        result.Sort((a, b) =>
        {
            var sumA = CalculatePodMetricSum(a.Metrics, metric);
            var sumB = CalculatePodMetricSum(b.Metrics, metric);
            return sumB.CompareTo(sumA); // Descending order
        });

        return result;
    }

    /// <summary>
    /// Calculate the percentage of node capacity used by the specified metric.
    /// </summary>
    private static double CalculateNodePercentage(V1Node node, NodeMetrics metrics, string metric)
    {
        if (metrics?.Usage == null || !metrics.Usage.TryGetValue(metric, out var usage))
        {
            return 0;
        }

        if (node?.Status?.Capacity == null || !node.Status.Capacity.TryGetValue(metric, out var capacity))
        {
            return double.PositiveInfinity;
        }

        var usageValue = usage.ToDouble();
        var capacityValue = capacity.ToDouble();

        if (capacityValue == 0)
        {
            return double.PositiveInfinity;
        }

        return usageValue / capacityValue;
    }

    /// <summary>
    /// Calculate the sum of a metric across all containers in a pod.
    /// </summary>
    private static double CalculatePodMetricSum(PodMetrics podMetrics, string metric)
    {
        if (podMetrics?.Containers == null)
        {
            return 0;
        }

        double sum = 0;
        foreach (var container in podMetrics.Containers)
        {
            if (container?.Usage != null && container.Usage.TryGetValue(metric, out var value))
            {
                sum += value.ToDouble();
            }
        }

        return sum;
    }
}
