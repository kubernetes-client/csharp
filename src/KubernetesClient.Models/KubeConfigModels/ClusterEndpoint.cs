using YamlDotNet.Serialization;

namespace k8s.KubeConfigModels
{
    /// <summary>
    /// Contains information about how to communicate with a kubernetes cluster
    /// </summary>
    public class ClusterEndpoint
    {
        /// <summary>
        /// Gets or sets the path to a cert file for the certificate authority.
        /// </summary>
        [YamlMember(Alias = "certificate-authority", ApplyNamingConventions = false)]
        public string CertificateAuthority { get; set; }

        /// <summary>
        /// Gets or sets =PEM-encoded certificate authority certificates. Overrides <see cref="CertificateAuthority"/>.
        /// </summary>
        [YamlMember(Alias = "certificate-authority-data", ApplyNamingConventions = false)]
        public string CertificateAuthorityData { get; set; }

        /// <summary>
        /// Gets or sets the address of the kubernetes cluster (https://hostname:port).
        /// </summary>
        [YamlMember(Alias = "server")]
        public string Server { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to skip the validity check for the server's certificate.
        /// This will make your HTTPS connections insecure.
        /// </summary>
        [YamlMember(Alias = "insecure-skip-tls-verify", ApplyNamingConventions = false)]
        public bool SkipTlsVerify { get; set; }

        /// <summary>
        /// Gets or sets additional information. This is useful for extenders so that reads and writes don't clobber unknown fields.
        /// </summary>
        [YamlMember(Alias = "extensions")]
        public IEnumerable<NamedExtension> Extensions { get; set; }
    }
}
