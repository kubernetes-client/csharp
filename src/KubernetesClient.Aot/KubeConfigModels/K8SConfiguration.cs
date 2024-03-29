using YamlDotNet.Serialization;

namespace k8s.KubeConfigModels
{
    /// <summary>
    /// kubeconfig configuration model. Holds the information needed to build connect to remote
    /// Kubernetes clusters as a given user.
    /// </summary>
    /// <remarks>
    /// Should be kept in sync with https://github.com/kubernetes/kubernetes/blob/master/staging/src/k8s.io/client-go/tools/clientcmd/api/v1/types.go
    /// Should update MergeKubeConfig in KubernetesClientConfiguration.ConfigFile.cs if updated.
    /// </remarks>
    [YamlSerializable]
    public class K8SConfiguration
    {
        // /// <summary>
        // /// Gets or sets general information to be use for CLI interactions
        // /// </summary>
        // [YamlMember(Alias = "preferences")]
        // public IDictionary<string, object> Preferences { get; set; }

        [YamlMember(Alias = "apiVersion")]
        public string ApiVersion { get; set; }

        [YamlMember(Alias = "kind")]
        public string Kind { get; set; }

        /// <summary>
        /// Gets or sets the name of the context that you would like to use by default.
        /// </summary>
        [YamlMember(Alias = "current-context", ApplyNamingConventions = false)]
        public string CurrentContext { get; set; }

        /// <summary>
        /// Gets or sets a map of referencable names to context configs.
        /// </summary>
        [YamlMember(Alias = "contexts")]
        public List<Context> Contexts { get; set; } = new List<Context>();

        /// <summary>
        /// Gets or sets a map of referencable names to cluster configs.
        /// </summary>
        [YamlMember(Alias = "clusters")]
        public List<Cluster> Clusters { get; set; } = new List<Cluster>();

        /// <summary>
        /// Gets or sets a map of referencable names to user configs
        /// </summary>
        [YamlMember(Alias = "users")]
        public List<User> Users { get; set; } = new List<User>();

        // /// <summary>
        // /// Gets or sets additional information. This is useful for extenders so that reads and writes don't clobber unknown fields.
        // /// </summary>
        // [YamlMember(Alias = "extensions")]
        // public List<NamedExtension> Extensions { get; set; }

        /// <summary>
        /// Gets or sets the name of the Kubernetes configuration file. This property is set only when the configuration
        /// was loaded from disk, and can be used to resolve relative paths.
        /// </summary>
        [YamlIgnore]
        public string FileName { get; set; }
    }
}
