namespace k8s.KubeConfigModels
{
    using System.Collections.Generic;
    using YamlDotNet.RepresentationModel;
    using YamlDotNet.Serialization;

    /// <summary>
    /// Contains information that describes identity information.  This is use to tell the kubernetes cluster who you are.
    /// </summary>
    public class AuthProvider {
        /// <summary>
        /// Gets or sets the nickname for this auth provider.
        /// </summary>
        [YamlMember(Alias = "name")]
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the configuration for this auth provider
        /// </summary>
        [YamlMember(Alias = "config")]
        public Dictionary<string, string> Config { get; set; }

    }
}
