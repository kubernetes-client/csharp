using Microsoft.Rest;

namespace k8s.Models
{
    public class KubernetesList<T> : IMetadata<V1ListMeta>, IItems<T>
        where T : IKubernetesObject
    {
        public KubernetesList(IList<T> items, string apiVersion = default, string kind = default,
            V1ListMeta metadata = default)
        {
            ApiVersion = apiVersion;
            Items = items;
            Kind = kind;
            Metadata = metadata;
        }

        /// <summary>
        /// Gets or sets aPIVersion defines the versioned schema of this
        /// representation of an object. Servers should convert recognized
        /// schemas to the latest internal value, and may reject unrecognized
        /// values. More info:
        /// https://git.k8s.io/community/contributors/devel/sig-architecture/api-conventions.md#resources
        /// </summary>
        [JsonPropertyName("apiVersion")]
        public string ApiVersion { get; set; }

        [JsonPropertyName("items")]
        public IList<T> Items { get; set; }

        /// <summary>
        /// Gets or sets kind is a string value representing the REST resource
        /// this object represents. Servers may infer this from the endpoint
        /// the client submits requests to. Cannot be updated. In CamelCase.
        /// More info:
        /// https://git.k8s.io/community/contributors/devel/sig-architecture/api-conventions.md#types-kinds
        /// </summary>
        [JsonPropertyName("kind")]
        public string Kind { get; set; }

        /// <summary>
        /// Gets or sets standard object's metadata.
        /// </summary>
        [JsonPropertyName("metadata")]
        public V1ListMeta Metadata { get; set; }

        /// <summary>
        /// Validate the object.
        /// </summary>
        /// <exception cref="ValidationException">
        /// Thrown if validation fails
        /// </exception>
        public void Validate()
        {
            if (Items == null)
            {
                throw new ArgumentNullException("Items");
            }

            if (Items != null)
            {
                foreach (var element in Items.OfType<IValidate>())
                {
                    element.Validate();
                }
            }
        }
    }
}
