namespace k8s.KubeConfigModels
{
    using YamlDotNet.RepresentationModel;
    using YamlDotNet.Serialization;

    public class UserCredentials
    {
        [YamlMember(Alias = "client-certificate-data")]
        public string ClientCertificateData { get; set; }

        [YamlMember(Alias = "client-key-data")]
        public string ClientKeyData { get; set; }

        [YamlMember(Alias = "token")]
        public string Token { get; set; }

        [YamlMember(Alias = "username")]
        public string UserName { get; set; }

        [YamlMember(Alias = "password")]
        public string Password { get; set; }
    }
}
