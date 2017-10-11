namespace k8s
{
    using System;
    using System.IO;
    using System.Linq;
    using System.Security.Cryptography.X509Certificates;
    using k8s.Exceptions;
    using k8s.KubeConfigModels;
    using YamlDotNet.Serialization;
    using System.Runtime.InteropServices;

    /// <summary>
    /// Represents a set of kubernetes client configuration settings
    /// </summary>
    public partial class KubernetesClientConfiguration
    {
        private KubernetesClientConfiguration()
        {
            
        }

        /// <summary>
        /// Gets Host
        /// </summary>
        public string Host { get; private set; }

        /// <summary>
        /// Gets SslCaCert
        /// </summary>
        public X509Certificate2 SslCaCert { get; private set; }

        /// <summary>
        /// Gets ClientCertificateData
        /// </summary>
        public string ClientCertificateData { get; private set; }

        /// <summary>
        /// Gets ClientCertificate Key
        /// </summary>
        public string ClientCertificateKey { get; private set; }

        /// <summary>
        /// Gets ClientCertificate filename
        /// </summary>
        public string ClientCertificate { get; private set; }

        /// <summary>
        /// Gets ClientCertificate Key filename
        /// </summary>
        public string ClientKey { get; private set; }

        /// <summary>
        /// Gets a value indicating whether to skip ssl server cert validation
        /// </summary>
        public bool SkipTlsVerify { get; private set; }

        /// <summary>
        /// Gets or sets the HTTP user agent.
        /// </summary>
        /// <value>Http user agent.</value>
        public string UserAgent { get; set; }

        /// <summary>
        /// Gets or sets the username (HTTP basic authentication).
        /// </summary>
        /// <value>The username.</value>
        public string Username { get; set; }

        /// <summary>
        /// Gets or sets the password (HTTP basic authentication).
        /// </summary>
        /// <value>The password.</value>
        public string Password { get; set; }

        /// <summary>
        /// Gets or sets the access token for OAuth2 authentication.
        /// </summary>
        /// <value>The access token.</value>
        public string AccessToken { get; set; }
    }
}
