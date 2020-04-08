using Newtonsoft.Json;

namespace k8s
{
    /// <summary>
    /// Represents a generic Kubernetes object.
    /// </summary>
    /// <remarks>
    /// You can use the <see cref="KubernetesObject"/> if you receive JSON from a Kubernetes API server but
    /// are unsure which object the API server is about to return. You can parse the JSON as a <see cref="KubernetesObject"/>
    /// and use the <see cref="ApiVersion"/> and <see cref="Kind"/> properties to get basic metadata about any Kubernetes object.
    /// You can then
    /// </remarks>
    public interface IKubernetesObject
    {
        /// <summary>
        /// Gets or sets aPIVersion defines the versioned schema of this
        /// representation of an object. Servers should convert recognized
        /// schemas to the latest internal value, and may reject unrecognized
        /// values. More info:
        /// https://git.k8s.io/community/contributors/devel/api-conventions.md#resources
        /// </summary>
        [JsonProperty(PropertyName = "apiVersion")]
        string ApiVersion { get; set; }

        /// <summary>
        /// Gets or sets kind is a string value representing the REST resource
        /// this object represents. Servers may infer this from the endpoint
        /// the client submits requests to. Cannot be updated. In CamelCase.
        /// More info:
        /// https://git.k8s.io/community/contributors/devel/api-conventions.md#types-kinds
        /// </summary>
        [JsonProperty(PropertyName = "kind")]
        string Kind { get; set; }
    }

    /// <summary>Represents a generic Kubernetes object that has an API version, a kind, and metadata.</summary>
    /// <typeparam name="TMetadata"></typeparam>
    public interface IKubernetesObject<TMetadata> : IKubernetesObject, IMetadata<TMetadata>
    {
    }
}
