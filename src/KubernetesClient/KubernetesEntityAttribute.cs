using System;

namespace k8s.Models
{
    /// <summary>
    /// Describes object type in Kubernetes
    /// </summary>
    public sealed class KubernetesEntityAttribute : Attribute
    {
        /// <summary>
        /// The Kubernetes named schema this object is based on.
        /// </summary>
        public string Kind { get; set; }

        /// <summary>
        /// The Group this Kubernetes type belongs to.
        /// </summary>
        public string Group { get; set; }

        /// <summary>
        /// The API Version this Kubernetes type belongs to.
        /// </summary>
        public string ApiVersion { get; set; }

        /// <summary>
        /// The plural name of the entity.
        /// </summary>
        public string PluralName { get; set; }
    }
}
