using System.Net;
using System.Net.Http;
using System.Net.Security;
using System.Net.Sockets;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Security.Cryptography.X509Certificates;
using System.Threading;
using k8s.Exceptions;
using k8s.Autorest;

namespace k8s
{
    public partial class Kubernetes
    {
        /// <summary>
        ///     Timeout of REST calls to Kubernetes server
        ///     Does not apply to watch related api
        /// </summary>
        /// <value>timeout</value>
        public TimeSpan HttpClientTimeout { get; set; } = TimeSpan.FromSeconds(100);

        /// <summary>
        ///     Initializes a new instance of the <see cref="Kubernetes" /> class.
        /// </summary>
        /// <param name='config'>
        ///     The kube config to use.
        /// </param>
        /// <param name="handlers">
        ///     Optional. The delegating handlers to add to the http client pipeline.
        /// </param>
        public Kubernetes(KubernetesClientConfiguration config, params DelegatingHandler[] handlers)
        {
            Initialize();
            ValidateConfig(config);
            CaCerts = config.SslCaCerts;
            SkipTlsVerify = config.SkipTlsVerify;
            CreateHttpClient(handlers, config);
            InitializeFromConfig(config);
            HttpClientTimeout = config.HttpClientTimeout;
        }

        private void ValidateConfig(KubernetesClientConfiguration config)
        {
            if (config == null)
            {
                throw new KubeConfigException("KubeConfig must be provided");
            }

            if (string.IsNullOrWhiteSpace(config.Host))
            {
                throw new KubeConfigException("Host url must be set");
            }

            try
            {
                BaseUri = new Uri(config.Host);
            }
            catch (UriFormatException e)
            {
                throw new KubeConfigException("Bad host url", e);
            }
        }

        private void InitializeFromConfig(KubernetesClientConfiguration config)
        {
            if (BaseUri.Scheme == "https")
            {
                if (config.SkipTlsVerify)
                {
#if NET5_0_OR_GREATER
                    HttpClientHandler.SslOptions.RemoteCertificateValidationCallback =
#else
                    HttpClientHandler.ServerCertificateCustomValidationCallback =
#endif
                        (sender, certificate, chain, sslPolicyErrors) => true;
                }
                else
                {
                    if (CaCerts == null)
                    {
                        throw new KubeConfigException("A CA must be set when SkipTlsVerify === false");
                    }

#if NET5_0_OR_GREATER
                    HttpClientHandler.SslOptions.RemoteCertificateValidationCallback =
#else
                    HttpClientHandler.ServerCertificateCustomValidationCallback =
#endif
                        (sender, certificate, chain, sslPolicyErrors) =>
                        {
                            return CertificateValidationCallBack(sender, CaCerts, certificate, chain,
                                sslPolicyErrors);
                        };
                }
            }

            // set credentails for the kubernetes client
            SetCredentials(config);

            var clientCert = CertUtils.GetClientCert(config);
            if (clientCert != null)
            {
#if NET5_0_OR_GREATER
                HttpClientHandler.SslOptions.ClientCertificates.Add(clientCert);
#else
                HttpClientHandler.ClientCertificates.Add(clientCert);
#endif
            }
        }

        private X509Certificate2Collection CaCerts { get; }

        private X509Certificate2 ClientCert { get; }

        private bool SkipTlsVerify { get; }

        // NOTE: this method replicates the logic that the base ServiceClient uses except that it doesn't insert the RetryDelegatingHandler
        // and it does insert the WatcherDelegatingHandler. we don't want the RetryDelegatingHandler because it has a very broad definition
        // of what requests have failed. it considers everything outside 2xx to be failed, including 1xx (e.g. 101 Switching Protocols) and
        // 3xx. in particular, this prevents upgraded connections and certain generic/custom requests from working.
        private void CreateHttpClient(DelegatingHandler[] handlers, KubernetesClientConfiguration config)
        {
#if NET5_0_OR_GREATER
            FirstMessageHandler = HttpClientHandler = new SocketsHttpHandler
            {
                KeepAlivePingPolicy = HttpKeepAlivePingPolicy.WithActiveRequests,
                KeepAlivePingDelay = TimeSpan.FromMinutes(3),
                KeepAlivePingTimeout = TimeSpan.FromSeconds(30),
            };

            HttpClientHandler.SslOptions.ClientCertificates = new X509Certificate2Collection();
#else
            FirstMessageHandler = HttpClientHandler = new HttpClientHandler();
#endif

            if (handlers != null)
            {
                for (int i = handlers.Length - 1; i >= 0; i--)
                {
                    DelegatingHandler handler = handlers[i];
                    while (handler.InnerHandler is DelegatingHandler d)
                    {
                        handler = d;
                    }

                    handler.InnerHandler = FirstMessageHandler;
                    FirstMessageHandler = handlers[i];
                }
            }

            HttpClient = new HttpClient(FirstMessageHandler, false)
            {
                Timeout = Timeout.InfiniteTimeSpan,
            };
        }



        /// <summary>
        ///     Set credentials for the Client
        /// </summary>
        /// <param name="config">k8s client configuration</param>
        private void SetCredentials(KubernetesClientConfiguration config) => Credentials = CreateCredentials(config);

        /// <summary>
        ///     SSl Cert Validation Callback
        /// </summary>
        /// <param name="sender">sender</param>
        /// <param name="caCerts">client ca</param>
        /// <param name="certificate">client certificate</param>
        /// <param name="chain">chain</param>
        /// <param name="sslPolicyErrors">ssl policy errors</param>
        /// <returns>true if valid cert</returns>
        public static bool CertificateValidationCallBack(
            object sender,
            X509Certificate2Collection caCerts,
            X509Certificate certificate,
            X509Chain chain,
            SslPolicyErrors sslPolicyErrors)
        {
            if (caCerts == null)
            {
                throw new ArgumentNullException(nameof(caCerts));
            }

            if (chain == null)
            {
                throw new ArgumentNullException(nameof(chain));
            }

            // If the certificate is a valid, signed certificate, return true.
            if (sslPolicyErrors == SslPolicyErrors.None)
            {
                return true;
            }

            // If there are errors in the certificate chain, look at each error to determine the cause.
            if ((sslPolicyErrors & SslPolicyErrors.RemoteCertificateChainErrors) != 0)
            {
                chain.ChainPolicy.RevocationMode = X509RevocationMode.NoCheck;

                // Added our trusted certificates to the chain
                //
                chain.ChainPolicy.ExtraStore.AddRange(caCerts);

                chain.ChainPolicy.VerificationFlags = X509VerificationFlags.AllowUnknownCertificateAuthority;
                var isValid = chain.Build((X509Certificate2)certificate);

                var isTrusted = false;

                var rootCert = chain.ChainElements[chain.ChainElements.Count - 1].Certificate;

                // Make sure that one of our trusted certs exists in the chain provided by the server.
                //
                foreach (var cert in caCerts)
                {
                    if (rootCert.RawData.SequenceEqual(cert.RawData))
                    {
                        isTrusted = true;
                        break;
                    }
                }

                return isValid && isTrusted;
            }

            // In all other cases, return false.
            return false;
        }

        /// <summary>
        /// Creates <see cref="ServiceClientCredentials"/> based on the given config, or returns null if no such credentials are needed.
        /// </summary>
        /// <param name="config">kubenetes config object</param>
        /// <returns>instance of <see cref="ServiceClientCredentials"/></returns>
        public static ServiceClientCredentials CreateCredentials(KubernetesClientConfiguration config)
        {
            if (config == null)
            {
                throw new ArgumentNullException(nameof(config));
            }

            if (config.TokenProvider != null)
            {
                return new TokenCredentials(config.TokenProvider);
            }
            else if (!string.IsNullOrEmpty(config.AccessToken))
            {
                return new TokenCredentials(config.AccessToken);
            }
            else if (!string.IsNullOrEmpty(config.Username))
            {
                return new BasicAuthenticationCredentials() { UserName = config.Username, Password = config.Password };
            }

            return null;
        }
    }
}
