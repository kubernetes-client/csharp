namespace k8s.KubeConfigModels
{
    using YamlDotNet.Serialization;

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

        [YamlMember(Alias = "namespace")]
        public string Namespace { get; set; }
    }
}
