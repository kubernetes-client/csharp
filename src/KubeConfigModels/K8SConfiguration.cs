namespace k8s.KubeConfigModels
{
    using System.Collections.Generic;
    using YamlDotNet.Serialization;

    /// <summary>
    /// kubeconfig configuration model
    /// </summary>
    public class K8SConfiguration
    {
        [YamlMember(Alias = "preferences")]
        public IDictionary<string, object> Preferences{ get; set; }

        [YamlMember(Alias = "apiVersion")]
        public string ApiVersion { get; set; }

        [YamlMember(Alias = "kind")]
        public string Kind { get; set; }

        [YamlMember(Alias = "current-context")]
        public string CurrentContext { get; set; }

        [YamlMember(Alias = "contexts")]
        public IEnumerable<Context> Contexts { get; set; } = new Context[0];

        [YamlMember(Alias = "clusters")]
        public IEnumerable<Cluster> Clusters { get; set; } = new Cluster[0];

        [YamlMember(Alias = "users")]
        public IEnumerable<User> Users { get; set; } = new User[0];
    }
}
