namespace k8s
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.Net.Http;
    using System.Net.Security;
    using System.Security.Cryptography.X509Certificates;
    using System.Threading.Tasks;
    using k8s.Exceptions;
    using Microsoft.Rest;

    public partial class Kubernetes : ServiceClient<Kubernetes>, IKubernetes
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Kubernetes"/> class.
        /// </summary>
        /// <param name='config'>
        /// Optional. The delegating handlers to add to the http client pipeline.
        /// </param>
        public Kubernetes(KubernetesClientConfiguration config)
        {
            this.Initialize();

            this.CaCert = config.SslCaCert;
            this.BaseUri = new Uri(config.Host);

            var handler = new HttpClientHandler();

            if (BaseUri.Scheme == "https")
            {
                if (config.SkipTlsVerify)
                {
                    handler.ServerCertificateCustomValidationCallback = (sender, certificate, chain, sslPolicyErrors) => true;
                }
                else
                {
                    if (CaCert == null)
                    {
                        throw new KubeConfigException("a CA must be set when SkipTlsVerify === false");
                    }

                    handler.ServerCertificateCustomValidationCallback = CertificateValidationCallBack;
                }
            }

            // set credentails for the kubernernet client
            this.SetCredentials(config, handler);
            this.InitializeHttpClient(handler, new DelegatingHandler[]{new WatcherDelegatingHandler()});
        }

        private X509Certificate2 CaCert { get; set; }

        /// <summary>
        /// Set credentials for the Client
        /// </summary>
        /// <param name="config">k8s client configuration</param>
        /// <param name="handler">http client handler for the rest client</param>
        /// <returns>Task</returns>
        private void SetCredentials(KubernetesClientConfiguration config, HttpClientHandler handler)
        {
            // set the Credentails for token based auth
            if (!string.IsNullOrWhiteSpace(config.AccessToken))
            {
                Credentials = new TokenCredentials(config.AccessToken);
            }
            else if (!string.IsNullOrWhiteSpace(config.Username) && !string.IsNullOrWhiteSpace(config.Password))
            {
                Credentials = new BasicAuthenticationCredentials
                {
                    UserName = config.Username,
                    Password = config.Password
                };
            }
            // othwerwise set handler for clinet cert based auth
            else if ((!string.IsNullOrWhiteSpace(config.ClientCertificateData) ||
                      !string.IsNullOrWhiteSpace(config.ClientCertificateFilePath)) &&
                     (!string.IsNullOrWhiteSpace(config.ClientCertificateKeyData) ||
                      !string.IsNullOrWhiteSpace(config.ClientKeyFilePath)))
            {
                var cert = Utils.GeneratePfx(config);

                handler.ClientCertificates.Add(cert);
            }
        }

        /// <summary>
        /// SSl Cert Validation Callback
        /// </summary>
        /// <param name="sender">sender</param>
        /// <param name="certificate">client certificate</param>
        /// <param name="chain">chain</param>
        /// <param name="sslPolicyErrors">ssl policy errors</param>
        /// <returns>true if valid cert</returns>
        [SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", Justification = "Unused by design")]
        private bool CertificateValidationCallBack(
                     object sender,
                     X509Certificate certificate,
                     X509Chain chain,
                     SslPolicyErrors sslPolicyErrors)
        {
            // If the certificate is a valid, signed certificate, return true.
            if (sslPolicyErrors == SslPolicyErrors.None)
            {
                return true;
            }

            // If there are errors in the certificate chain, look at each error to determine the cause.
            if ((sslPolicyErrors & SslPolicyErrors.RemoteCertificateChainErrors) != 0)
            {
                chain.ChainPolicy.RevocationMode = X509RevocationMode.NoCheck;

                // add all your extra certificate chain
                chain.ChainPolicy.ExtraStore.Add(this.CaCert);
                chain.ChainPolicy.VerificationFlags = X509VerificationFlags.AllowUnknownCertificateAuthority;
                var isValid = chain.Build((X509Certificate2)certificate);
                return isValid;
            }
            else
            {
                // In all other cases, return false.
                return false;
            }
        }
    }
}
