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
        public IDictionary<string, object> preferences{ get; set; }

        [YamlMember(Alias = "apiVersion")]
        public string ApiVersion { get; set; }

        [YamlMember(Alias = "kind")]
        public string Kind { get; set; }

        [YamlMember(Alias = "current-context")]
        public string CurrentContext { get; set; }

        [YamlMember(Alias = "contexts")]
        public IEnumerable<Context> Contexts { get; set; }

        [YamlMember(Alias = "clusters")]
        public IEnumerable<Cluster> Clusters { get; set; }

        [YamlMember(Alias = "users")]
        public IEnumerable<User> Users { get; set; }
    }
}
