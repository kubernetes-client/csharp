namespace k8s.KubeConfigModels
{
    using System.Collections.Generic;
    using YamlDotNet.RepresentationModel;
    using YamlDotNet.Serialization;

    public class UserCredentials
    {
        [YamlMember(Alias = "client-certificate-data")]
        public string ClientCertificateData { get; set; }

        [YamlMember(Alias = "client-certificate")]
        public string ClientCertificate { get; set; }

        [YamlMember(Alias = "client-key-data")]
        public string ClientKeyData { get; set; }

        [YamlMember(Alias = "client-key")]
        public string ClientKey { get; set; }

        [YamlMember(Alias = "token")]
        public string Token { get; set; }

        [YamlMember(Alias = "username")]
        public string UserName { get; set; }

        [YamlMember(Alias = "password")]
        public string Password { get; set; }

        [YamlMember(Alias = "auth-provider")]
        public Dictionary<string, dynamic> AuthProvider { get; set; }
    }
}
