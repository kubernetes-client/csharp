using Newtonsoft.Json;
using System.Collections.Generic;

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
        [JsonProperty(PropertyName = "name")]
        public string Name { get; set; }

        /// <summary>
        /// The resource usage.
        /// </summary>
        [JsonProperty(PropertyName = "usage")]
        public IDictionary<string, ResourceQuantity> Usage { get; set; }
    }
}
