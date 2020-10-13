using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace k8s.Models
{
    /// <summary>
    /// Describes the resource usage metrics of a pod pull from metrics server API.
    /// </summary>
    public class PodMetrics : IKubernetesObject, IMetadata<V1ObjectMeta>
    {
        /// <summary>
        /// Defines the versioned schema of this representation of an object.
        /// </summary>
        [JsonProperty( PropertyName = "apiVersion" )]
        public string ApiVersion { get; set; }

        /// <summary>
        /// Defines the REST resource this object represents.
        /// </summary>
        [JsonProperty( PropertyName = "kind" )]
        public string Kind { get; set; }

        /// <summary>
        /// The kubernetes standard object's metadata.
        /// </summary>
        [JsonProperty(PropertyName = "metadata")]
        public V1ObjectMeta Metadata { get; set; }

        /// <summary>
        /// The timestamp when metrics were collected.
        /// </summary>
        [JsonProperty(PropertyName = "timestamp")]
        public DateTime? Timestamp { get; set; }

        /// <summary>
        /// The interval from which metrics were collected.
        /// </summary>
        [JsonProperty(PropertyName = "window")]
        public string Window { get; set; }

        /// <summary>
        /// The list of containers metrics.
        /// </summary>
        [JsonProperty(PropertyName = "containers")]
        public List<ContainerMetrics> Containers { get; set; }
    }
}
