namespace k8s.Models
{
    /// <summary>
    /// Describes the resource usage metrics of a pod pull from metrics server API.
    /// </summary>
    public class PodMetrics
    {
        /// <summary>
        /// The kubernetes standard object's metadata.
        /// </summary>
        [JsonPropertyName("metadata")]
        public V1ObjectMeta Metadata { get; set; }

        /// <summary>
        /// The timestamp when metrics were collected.
        /// </summary>
        [JsonPropertyName("timestamp")]
        public DateTime? Timestamp { get; set; }

        /// <summary>
        /// The interval from which metrics were collected.
        /// </summary>
        [JsonPropertyName("window")]
        public string Window { get; set; }

        /// <summary>
        /// The list of containers metrics.
        /// </summary>
        [JsonPropertyName("containers")]
        public List<ContainerMetrics> Containers { get; set; }
    }
}
