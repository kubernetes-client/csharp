using System;
using System.Collections.Concurrent;
using System.Text.RegularExpressions;
using k8s.Models;
using Newtonsoft.Json;

namespace k8s.CustomResources
{
    /// <summary>
    /// Base class for defining new custom resources types
    /// </summary>
    public abstract class CustomResource : KubernetesObject, IKubernetesObject<V1ObjectMeta>, IValidate
    {
        private static readonly ConcurrentDictionary<Type, KubernetesEntityAttribute> MetadataCache = new ConcurrentDictionary<Type, KubernetesEntityAttribute>();

        public CustomResource()
        {
            this.Initialize();

        }

        public CustomResource(string name) : this()
        {
            Metadata.Name = name;
        }
        [JsonProperty(PropertyName = "metadata")]
        [NJsonSchema.Annotations.JsonSchemaIgnore]
        public V1ObjectMeta Metadata { get; set; }
        public void Validate()
        {
            var metadata = this.GetKubernetesTypeMetadata();
            if (metadata == null)
            {
                throw new InvalidOperationException($"{GetType()} does not have {nameof(KubernetesEntityAttribute)} applied to it");
            }

            metadata.Validate();

            if (ApiVersion == null)
            {
                throw new InvalidOperationException($"{nameof(ApiVersion)} cannot be null");
            }

            if (ApiVersion != $"{metadata.Group}/{metadata.ApiVersion}")
            {
                throw new InvalidOperationException($"{nameof(ApiVersion)} must be equals to Group/Version as defined in {nameof(KubernetesEntityAttribute)} on the type");
            }

            if (Kind == null)
            {
                throw new InvalidOperationException($"{nameof(Kind)} cannot be null");
            }

            if (Metadata == null)
            {
                throw new InvalidOperationException($"{nameof(Metadata)} cannot be null");
            }

            if (string.IsNullOrWhiteSpace(Metadata.Name))
            {
                throw new InvalidOperationException($"{nameof(Metadata.Name)} must have a value");
            }
        }


    }
}
