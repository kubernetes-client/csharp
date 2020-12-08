using k8s.Models;
using Newtonsoft.Json.Linq;
using System.Threading.Tasks;

namespace k8s
{
    /// <summary>
    /// Extension methods for Kubernetes metrics.
    /// </summary>
    public static class KubernetesMetricsExtensions
    {
        /// <summary>
        /// Get nodes metrics pull from metrics server API.
        /// </summary>
        /// <param name="kubernetes">kubernetes client object</param>
        /// <returns>the metrics <see cref="PodMetricsList"/></returns>
        public static async Task<NodeMetricsList> GetKubernetesNodesMetricsAsync(this IKubernetes kubernetes)
        {
            var customObject = (JObject)await kubernetes.GetClusterCustomObjectAsync("metrics.k8s.io", "v1beta1", "nodes", string.Empty).ConfigureAwait(false);
            return customObject.ToObject<NodeMetricsList>();
        }

        /// <summary>
        /// Get pods metrics pull from metrics server API.
        /// </summary>
        /// <param name="kubernetes">kubernetes client object</param>
        /// <returns>the metrics <see cref="PodMetricsList"/></returns>
        public static async Task<PodMetricsList> GetKubernetesPodsMetricsAsync(this IKubernetes kubernetes)
        {
            var customObject = (JObject)await kubernetes.GetClusterCustomObjectAsync("metrics.k8s.io", "v1beta1", "pods", string.Empty).ConfigureAwait(false);
            return customObject.ToObject<PodMetricsList>();
        }

        /// <summary>
        /// Get pods metrics by namespace pull from metrics server API.
        /// </summary>
        /// <param name="kubernetes">kubernetes client object</param>
        /// <param name="namespaceParameter">the querying namespace</param>
        /// <returns>the metrics <see cref="PodMetricsList"/></returns>
        public static async Task<PodMetricsList> GetKubernetesPodsMetricsByNamespaceAsync(this IKubernetes kubernetes, string namespaceParameter)
        {
            var customObject = (JObject)await kubernetes.GetNamespacedCustomObjectAsync("metrics.k8s.io", "v1beta1", namespaceParameter, "pods", string.Empty).ConfigureAwait(false);
            return customObject.ToObject<PodMetricsList>();
        }
    }
}
