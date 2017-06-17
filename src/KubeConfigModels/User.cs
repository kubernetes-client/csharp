namespace k8s.KubeConfigModels
{
    using YamlDotNet.RepresentationModel;
    using YamlDotNet.Serialization;

    public class User
    {
        [YamlMember(Alias = "user")]
        public UserCrednetials UserCredentials { get; set; }

        [YamlMember(Alias = "name")]
        public string Name { get; set; }
    }
}
