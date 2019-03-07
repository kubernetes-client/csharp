using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;

namespace k8s
{
    /// <summary>
    ///     Represents a set of kubernetes client configuration settings
    /// </summary>
    public partial class KubernetesClientConfiguration
    {
        /// <summary>
        ///     Gets current namespace
        /// </summary>
        public string Namespace { get; set; }

        /// <summary>
        ///     Gets Host
        /// </summary>
        public string Host { get; set; }

        /// <summary>
        ///     Gets SslCaCerts
        /// </summary>
        public X509Certificate2Collection SslCaCerts { get; set; }

        /// <summary>
        ///     Gets ClientCertificateData
        /// </summary>
        public string ClientCertificateData { get; set; }

        /// <summary>
        ///     Gets ClientCertificate Key
        /// </summary>
        public string ClientCertificateKeyData { get; set; }

        /// <summary>
        ///     Gets ClientCertificate filename
        /// </summary>
        public string ClientCertificateFilePath { get; set; }

        /// <summary>
        ///     Gets or sets the ClientCertificate KeyStoreFlags to specify where and how to import the certificate private key
        /// </summary>
        public X509KeyStorageFlags? ClientCertificateKeyStoreFlags { get; set; }

        /// <summary>
        ///     Gets ClientCertificate Key filename
        /// </summary>
        public string ClientKeyFilePath { get; set; }

        /// <summary>
        ///     Gets a value indicating whether to skip ssl server cert validation
        /// </summary>
        public bool SkipTlsVerify { get; set; }

        /// <summary>
        ///     Gets or sets the HTTP user agent.
        /// </summary>
        /// <value>Http user agent.</value>
        public string UserAgent { get; set; }

        /// <summary>
        ///     Gets or sets the username (HTTP basic authentication).
        /// </summary>
        /// <value>The username.</value>
        public string Username { get; set; }

        /// <summary>
        ///     Gets or sets the password (HTTP basic authentication).
        /// </summary>
        /// <value>The password.</value>
        public string Password { get; set; }

        /// <summary>
        ///     Gets or sets the access token for OAuth2 authentication.
        /// </summary>
        /// <value>The access token.</value>
        public string AccessToken { get; set; }
    }
}
