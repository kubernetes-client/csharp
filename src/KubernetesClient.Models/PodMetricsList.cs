namespace k8s.Models
{
    public class PodMetricsList
    {
        /// <summary>
        /// Defines the versioned schema of this representation of an object.
        /// </summary>
        [JsonPropertyName("apiVersion")]
        public string ApiVersion { get; set; }

        /// <summary>
        /// Defines the REST resource this object represents.
        /// </summary>
        [JsonPropertyName("kind")]
        public string Kind { get; set; }

        /// <summary>
        /// The kubernetes standard object's metadata.
        /// </summary>
        [JsonPropertyName("metadata")]
        public V1ObjectMeta Metadata { get; set; }

        /// <summary>
        /// The list of pod metrics.
        /// </summary>
        [JsonPropertyName("items")]
        public IEnumerable<PodMetrics> Items { get; set; }
    }
}
