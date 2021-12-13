using YamlDotNet.Serialization;

namespace k8s.KubeConfigModels
{
    /// <summary>
    /// Relates nicknames to context information.
    /// </summary>
    public class Context
    {
        /// <summary>
        /// Gets or sets the context information.
        /// </summary>
        [YamlMember(Alias = "context")]
        public ContextDetails ContextDetails { get; set; }

        /// <summary>
        /// Gets or sets the nickname for this context.
        /// </summary>
        [YamlMember(Alias = "name")]
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets additional information. This is useful for extenders so that reads and writes don't clobber unknown fields.
        /// </summary>
        [YamlMember(Alias = "extensions")]
        public IEnumerable<NamedExtension> Extensions { get; set; }


        [Obsolete("This property is not set by the YAML config. Use ContextDetails.Namespace instead.")]
        [YamlMember(Alias = "namespace")]
        public string Namespace { get; set; }
    }
}
