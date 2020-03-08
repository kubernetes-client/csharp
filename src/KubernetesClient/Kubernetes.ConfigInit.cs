using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using k8s.Exceptions;
using k8s.Models;
using Microsoft.Rest;

namespace k8s
{
    public partial class Kubernetes
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="Kubernetes" /> class.
        /// </summary>
        /// <param name='config'>
        ///     The kube config to use.
        /// </param>
        /// <param name="httpClient">
        ///     The <see cref="HttpClient" /> to use for all requests.
        /// </param>
        public Kubernetes(KubernetesClientConfiguration config, HttpClient httpClient) : this(config, httpClient, false)
        {
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="Kubernetes" /> class.
        /// </summary>
        /// <param name='config'>
        ///     The kube config to use.
        /// </param>
        /// <param name="httpClient">
        ///     The <see cref="HttpClient" /> to use for all requests.
        /// </param>
        /// <param name="disposeHttpClient">
        ///     Whether or not the <see cref="Kubernetes"/> object should own the lifetime of <paramref name="httpClient"/>.
        /// </param>
        public Kubernetes(KubernetesClientConfiguration config, HttpClient httpClient, bool disposeHttpClient) : this(httpClient, disposeHttpClient)
        {
            ValidateConfig(config);
            CaCerts = config.SslCaCerts;
            SkipTlsVerify = config.SkipTlsVerify;
            SetCredentials(config); 
        }

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
            : this(handlers)
        {
            ValidateConfig(config);
            CaCerts = config.SslCaCerts;
            SkipTlsVerify = config.SkipTlsVerify;
            InitializeFromConfig(config);
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
#if NET452
                    ((WebRequestHandler) HttpClientHandler).ServerCertificateValidationCallback =
                        (sender, certificate, chain, sslPolicyErrors) => true;
#elif XAMARINIOS1_0 || MONOANDROID8_1
                    System.Net.ServicePointManager.ServerCertificateValidationCallback += (sender, certificate, chain, sslPolicyErrors) =>
                    {
                        return true;
                    };
#else
                    HttpClientHandler.ServerCertificateCustomValidationCallback =
                        (sender, certificate, chain, sslPolicyErrors) => true;
#endif
                }
                else
                {
                    if (CaCerts == null)
                    {
                        throw new KubeConfigException("A CA must be set when SkipTlsVerify === false");
                    }
#if NET452
                    ((WebRequestHandler) HttpClientHandler).ServerCertificateValidationCallback = (sender, certificate, chain, sslPolicyErrors) =>
                    {
                        return Kubernetes.CertificateValidationCallBack(sender, CaCerts, certificate, chain, sslPolicyErrors);
                    };
#elif XAMARINIOS1_0
                    System.Net.ServicePointManager.ServerCertificateValidationCallback += (sender, certificate, chain, sslPolicyErrors) =>
                    {
                        var cert = new X509Certificate2(certificate);
                        return Kubernetes.CertificateValidationCallBack(sender, CaCerts, cert, chain, sslPolicyErrors);
                    };
#elif MONOANDROID8_1
                    var certList = new System.Collections.Generic.List<Java.Security.Cert.Certificate>();

                    foreach (X509Certificate2 caCert in CaCerts)
                    {
                        using (var certStream = new System.IO.MemoryStream(caCert.RawData))
                        {
                            Java.Security.Cert.Certificate cert = Java.Security.Cert.CertificateFactory.GetInstance("X509").GenerateCertificate(certStream);

                            certList.Add(cert);
                        }
                    }

                    var handler = (Xamarin.Android.Net.AndroidClientHandler)this.HttpClientHandler;

                    handler.TrustedCerts = certList;
#else
                    HttpClientHandler.ServerCertificateCustomValidationCallback = (sender, certificate, chain, sslPolicyErrors) =>
                    {
                        return Kubernetes.CertificateValidationCallBack(sender, CaCerts, certificate, chain, sslPolicyErrors);
                    };
#endif
                }
            }

            // set credentails for the kubernetes client
            SetCredentials(config);
            config.AddCertificates(HttpClientHandler);
        }

        private X509Certificate2Collection CaCerts { get; }

        private bool SkipTlsVerify { get; }

        partial void CustomInitialize()
        {
#if NET452 
            ServicePointManager.SecurityProtocol |= SecurityProtocolType.Tls12;
#endif
            AppendDelegatingHandler<WatcherDelegatingHandler>();
            DeserializationSettings.Converters.Add(new V1Status.V1StatusObjectViewConverter());
        }

        private void AppendDelegatingHandler<T>() where T : DelegatingHandler, new()
        {
            var cur = FirstMessageHandler as DelegatingHandler;

            while (cur != null)
            {
                var next = cur.InnerHandler as DelegatingHandler;

                if (next == null)
                {
                    // last one
                    // append watcher handler between to last handler
                    cur.InnerHandler = new T
                    {
                        InnerHandler = cur.InnerHandler
                    };
                    break;
                }

                cur = next;
            }
        }

        /// <summary>
        ///     Set credentials for the Client
        /// </summary>
        /// <param name="config">k8s client configuration</param>
        private void SetCredentials(KubernetesClientConfiguration config)
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
        }

        /// <summary>
        ///     SSl Cert Validation Callback
        /// </summary>
        /// <param name="sender">sender</param>
        /// <param name="certificate">client certificate</param>
        /// <param name="chain">chain</param>
        /// <param name="sslPolicyErrors">ssl policy errors</param>
        /// <returns>true if valid cert</returns>
        [SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", Justification = "Unused by design")]
        public static bool CertificateValidationCallBack(
            object sender,
            X509Certificate2Collection caCerts,
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
    }
}
