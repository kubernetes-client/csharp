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
        public static async Task<NodeMetricsList> GetKubernetesNodesMetricsAsync(this Kubernetes operations)
        {
            JObject customObject = (JObject)await operations.GetClusterCustomObjectAsync("metrics.k8s.io", "v1beta1", "nodes", string.Empty).ConfigureAwait(false);
            return customObject.ToObject<NodeMetricsList>();
        }
    }
}