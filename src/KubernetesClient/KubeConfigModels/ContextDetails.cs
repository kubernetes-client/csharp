using YamlDotNet.Serialization;

namespace k8s.KubeConfigModels
{
    /// <summary>
    /// Represents a tuple of references to a cluster (how do I communicate with a kubernetes cluster),
    /// a user (how do I identify myself), and a namespace (what subset of resources do I want to work with)
    /// </summary>
    public class ContextDetails
    {
        /// <summary>
        /// Gets or sets the name of the cluster for this context.
        /// </summary>
        [YamlMember(Alias = "cluster")]
        public string Cluster { get; set; }

        /// <summary>
        /// Gets or sets the name of the user for this context.
        /// </summary>
        [YamlMember(Alias = "user")]
        public string User { get; set; }

        /// <summary>
        /// /Gets or sets the default namespace to use on unspecified requests.
        /// </summary>
        [YamlMember(Alias = "namespace")]
        public string Namespace { get; set; }

        /// <summary>
        /// Gets or sets additional information. This is useful for extenders so that reads and writes don't clobber unknown fields.
        /// </summary>
        [YamlMember(Alias = "extensions")]
        public IEnumerable<NamedExtension> Extensions { get; set; }
    }
}
