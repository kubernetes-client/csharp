using YamlDotNet.Serialization;

namespace k8s.KubeConfigModels
{
    /// <summary>
    /// Relates nicknames to context information.
    /// </summary>
    [YamlSerializable]
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
    }
}
