namespace k8s.Models
{
    /// <summary>
    /// Describes the resource usage metrics of a container pull from metrics server API.
    /// </summary>
    public class ContainerMetrics
    {
        /// <summary>
        /// Defines container name corresponding to the one from pod.spec.containers.
        /// </summary>
        [JsonPropertyName("name")]
        public string Name { get; set; }

        /// <summary>
        /// The resource usage.
        /// </summary>
        [JsonPropertyName("usage")]
        public IDictionary<string, ResourceQuantity> Usage { get; set; }
    }
}
