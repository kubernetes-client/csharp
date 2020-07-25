using Newtonsoft.Json;
using System.Collections.Generic;

namespace k8s.Models
{
    public class NodeMetricsList
    {
        /// <summary>
        /// Defines the versioned schema of this representation of an object.
        /// </summary>
        [JsonProperty(PropertyName = "apiVersion")]
        public string ApiVersion { get; set; }

        /// <summary>
        /// Defines the REST resource this object represents.
        /// </summary>
        [JsonProperty(PropertyName = "kind")]
        public string Kind { get; set; }

        /// <summary>
        /// The kubernetes standard object's metadata.
        /// </summary>
        [JsonProperty(PropertyName = "metadata")]
        public V1ObjectMeta Metadata { get; set; }

        /// <summary>
        /// The list of node metrics.
        /// </summary>
        [JsonProperty(PropertyName = "items")]
        public IEnumerable<NodeMetrics> Items { get; set; }
    }
}
