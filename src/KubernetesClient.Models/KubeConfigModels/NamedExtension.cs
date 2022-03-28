using YamlDotNet.Serialization;

namespace k8s.KubeConfigModels
{
    /// <summary>
    /// <see cref="NamedExtension"/> relates nicknames to extension information
    /// </summary>
    public class NamedExtension
    {
        /// <summary>
        /// Gets or sets the nickname for this extension.
        /// </summary>
        [YamlMember(Alias = "name")]
        public string Name { get; set; }

        /// <summary>
        /// Get or sets the extension information.
        /// </summary>
        [YamlMember(Alias = "extension")]
        public dynamic Extension { get; set; }
    }
}
