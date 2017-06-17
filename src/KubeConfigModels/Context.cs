namespace k8s.KubeConfigModels
{
    using YamlDotNet.Serialization;

    public class Context
    {
        [YamlMember(Alias = "context")]
        public ContextDetails ContextDetails { get; set; }

        [YamlMember(Alias = "name")]
        public string Name { get; set; }
    }
}
