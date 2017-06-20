namespace k8s.KubeConfigModels
{
    using YamlDotNet.RepresentationModel;
    using YamlDotNet.Serialization;

    public class ContextDetails
    {
        [YamlMember(Alias = "cluster")]
        public string Cluster { get; set; }

        [YamlMember(Alias = "user")]
        public string User { get; set; }

        [YamlMember(Alias = "namespace")]
        public string Namespace { get; set; }
    }
}
