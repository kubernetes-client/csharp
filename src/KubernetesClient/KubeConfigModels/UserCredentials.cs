using System.Collections.Generic;
using YamlDotNet.Serialization;

namespace k8s.KubeConfigModels
{
    /// <summary>
    /// Contains information that describes identity information.  This is use to tell the kubernetes cluster who you are.
    /// </summary>
    public class UserCredentials
    {
        /// <summary>
        /// Gets or sets PEM-encoded data from a client cert file for TLS. Overrides <see cref="ClientCertificate"/>.
        /// </summary>
        [YamlMember(Alias = "client-certificate-data", ApplyNamingConventions = false)]
        public string ClientCertificateData { get; set; }

        /// <summary>
        /// Gets or sets the path to a client cert file for TLS.
        /// </summary>
        [YamlMember(Alias = "client-certificate", ApplyNamingConventions = false)]
        public string ClientCertificate { get; set; }

        /// <summary>
        /// Gets or sets PEM-encoded data from a client key file for TLS. Overrides <see cref="ClientKey"/>.
        /// </summary>
        [YamlMember(Alias = "client-key-data", ApplyNamingConventions = false)]
        public string ClientKeyData { get; set; }

        /// <summary>
        /// Gets or sets the path to a client key file for TLS.
        /// </summary>
        [YamlMember(Alias = "client-key", ApplyNamingConventions = false)]
        public string ClientKey { get; set; }

        /// <summary>
        /// Gets or sets the bearer token for authentication to the kubernetes cluster.
        /// </summary>
        [YamlMember(Alias = "token")]
        public string Token { get; set; }

        /// <summary>
        /// Gets or sets the username to impersonate. The name matches the flag.
        /// </summary>
        [YamlMember(Alias = "as")]
        public string Impersonate { get; set; }

        /// <summary>
        /// Gets or sets the groups to impersonate.
        /// </summary>
        [YamlMember(Alias = "as-groups", ApplyNamingConventions = false)]
        public IEnumerable<string> ImpersonateGroups { get; set; } = new string[0];

        /// <summary>
        /// Gets or sets additional information for impersonated user.
        /// </summary>
        [YamlMember(Alias = "as-user-extra", ApplyNamingConventions = false)]
        public Dictionary<string, string> ImpersonateUserExtra { get; set; } = new Dictionary<string, string>();

        /// <summary>
        /// Gets or sets the username for basic authentication to the kubernetes cluster.
        /// </summary>
        [YamlMember(Alias = "username")]
        public string UserName { get; set; }

        /// <summary>
        /// Gets or sets the password for basic authentication to the kubernetes cluster.
        /// </summary>
        [YamlMember(Alias = "password")]
        public string Password { get; set; }

        /// <summary>
        /// Gets or sets custom authentication plugin for the kubernetes cluster.
        /// </summary>
        [YamlMember(Alias = "auth-provider", ApplyNamingConventions = false)]
        public AuthProvider AuthProvider { get; set; }

        /// <summary>
        /// Gets or sets additional information. This is useful for extenders so that reads and writes don't clobber unknown fields.
        /// </summary>
        [YamlMember(Alias = "extensions")]
        public IEnumerable<NamedExtension> Extensions { get; set; }

        /// <summary>
        /// Gets or sets external command and its arguments to receive user credentials
        /// </summary>
        [YamlMember(Alias = "exec")]
        public ExternalExecution ExternalExecution { get; set; }
    }
}
