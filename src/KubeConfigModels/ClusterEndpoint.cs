namespace k8s.KubeConfigModels
{
    using YamlDotNet.Serialization;

    public class ClusterEndpoint
    {
        [YamlMember(Alias = "certificate-authority")]
        public string CertificateAuthority {get; set; }

        [YamlMember(Alias = "certificate-authority-data")]
        public string CertificateAuthorityData { get; set; }

        [YamlMember(Alias = "server")]
        public string Server { get; set; }

        [YamlMember(Alias = "insecure-skip-tls-verify")]
        public bool SkipTlsVerify { get; set; }
    }
}
