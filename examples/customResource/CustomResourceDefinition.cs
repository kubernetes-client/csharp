using k8s;
using k8s.Models;
using System.Collections.Generic;
using System.Text.Json.Serialization;

[module: System.Diagnostics.CodeAnalysis.SuppressMessage("StyleCop.CSharp.MaintainabilityRules", "CA1724:TypeNamesShouldNotMatchNamespaces", Justification = "This is just an example.")]
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("StyleCop.CSharp.MaintainabilityRules", "SA1402:FileMayOnlyContainASingleClass", Justification = "This is just an example.")]

namespace customResource
{
    public class CustomResourceDefinition
    {
        public string Version { get; set; }

        public string Group { get; set; }

        public string PluralName { get; set; }

        public string Kind { get; set; }

        public string Namespace { get; set; }
    }

    public abstract class CustomResource : KubernetesObject
    {
        [JsonPropertyName("metadata")]
        public V1ObjectMeta Metadata { get; set; }
    }

    public abstract class CustomResource<TSpec, TStatus> : CustomResource
    {
        [JsonPropertyName("spec")]
        public TSpec Spec { get; set; }

        [JsonPropertyName("CStatus")]
        public TStatus CStatus { get; set; }
    }

    public class CustomResourceList<T> : KubernetesObject
    where T : CustomResource
    {
        public V1ListMeta Metadata { get; set; }
        public List<T> Items { get; set; }
    }
}
