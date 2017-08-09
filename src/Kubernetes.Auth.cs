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

            // ssl cert validation
            Func<object, X509Certificate, X509Chain, SslPolicyErrors, bool> sslCertValidationFunc;
            if (config.SkipTlsVerify)
            {
                sslCertValidationFunc = (sender, certificate, chain, sslPolicyErrors) => true;
            }
            else
            {
                sslCertValidationFunc = this.CertificateValidationCallBack;
            }

            var handler = new HttpClientHandler
            {
                ServerCertificateCustomValidationCallback = sslCertValidationFunc
            };

            // set credentails for the kubernernet client
            this.SetCredentialsAsync(config, handler).Wait();
            this.InitializeHttpClient(handler);
        }

        private X509Certificate2 CaCert { get; set; }

        /// <summary>
        /// Set credentials for the Client
        /// </summary>
        /// <param name="config">k8s client configuration</param>
        /// <param name="handler">http client handler for the rest client</param>
        /// <returns>Task</returns>
        private async Task SetCredentialsAsync(KubernetesClientConfiguration config, HttpClientHandler handler)
        {
            // set the Credentails for token based auth
            if (!string.IsNullOrWhiteSpace(config.AccessToken))
            {
                this.Credentials = new KubernetesClientCredentials(config.AccessToken);
            }
            else if (!string.IsNullOrWhiteSpace(config.Username) && !string.IsNullOrWhiteSpace(config.Password))
            {
                this.Credentials = new KubernetesClientCredentials(config.Username, config.Password);
            }
            // othwerwise set handler for clinet cert based auth
            else if ((!string.IsNullOrWhiteSpace(config.ClientCertificateData) ||
                      !string.IsNullOrWhiteSpace(config.ClientCertificate)) &&
                     (!string.IsNullOrWhiteSpace(config.ClientCertificateKey) ||
                      !string.IsNullOrWhiteSpace(config.ClientKey)))
            {
                var pfxFilePath = await Utils.GeneratePfxAsync(config).ConfigureAwait(false);

                var cert = new X509Certificate2(pfxFilePath, string.Empty, X509KeyStorageFlags.PersistKeySet);
                handler.ClientCertificates.Add(cert);
            }
            else
            {
                throw new KubeConfigException("Configuration does not have appropriate auth credentials");
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
                X509Chain chain0 = new X509Chain();
                chain0.ChainPolicy.RevocationMode = X509RevocationMode.NoCheck;

                // add all your extra certificate chain
                chain0.ChainPolicy.ExtraStore.Add(this.CaCert);
                chain0.ChainPolicy.VerificationFlags = X509VerificationFlags.AllowUnknownCertificateAuthority;
                var isValid = chain0.Build((X509Certificate2)certificate);
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
