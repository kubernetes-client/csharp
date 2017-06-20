namespace k8s.KubeConfigModels
{
    using YamlDotNet.Serialization;

    public class Cluster
    {
        [YamlMember(Alias = "cluster")]
        public ClusterEndpoint ClusterEndpoint { get; set; }

        [YamlMember(Alias = "name")]
        public string Name { get; set; }
    }
}
