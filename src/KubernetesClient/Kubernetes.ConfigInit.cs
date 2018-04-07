using System;
using System.Diagnostics.CodeAnalysis;
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
        ///     Optional. The delegating handlers to add to the http client pipeline.
        /// </param>
        /// <param name="handlers">
        ///     Optional. The delegating handlers to add to the http client pipeline.
        /// </param>
        public Kubernetes(KubernetesClientConfiguration config, params DelegatingHandler[] handlers) : this(handlers)
        {
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

            CaCert = config.SslCaCert;
            SkipTlsVerify = config.SkipTlsVerify;

            if (BaseUri.Scheme == "https")
            {
                if (config.SkipTlsVerify)
                {
#if NET452
                    ((WebRequestHandler) HttpClientHandler).ServerCertificateValidationCallback =
                        (sender, certificate, chain, sslPolicyErrors) => true;
#else
                    HttpClientHandler.ServerCertificateCustomValidationCallback =
                        (sender, certificate, chain, sslPolicyErrors) => true;
#endif
                }
                else
                {
                    if (CaCert == null)
                    {
                        throw new KubeConfigException("a CA must be set when SkipTlsVerify === false");
                    }

#if NET452
                    ((WebRequestHandler) HttpClientHandler).ServerCertificateValidationCallback = (sender, certificate, chain, sslPolicyErrors) =>
                    {
                        return Kubernetes.CertificateValidationCallBack(sender, CaCert, certificate, chain, sslPolicyErrors);
                    };
#else
                    HttpClientHandler.ServerCertificateCustomValidationCallback = (sender, certificate, chain, sslPolicyErrors) => 
                    {
                        return Kubernetes.CertificateValidationCallBack(sender, CaCert, certificate, chain, sslPolicyErrors);
                    };
#endif
                }
            }

            // set credentails for the kubernernet client
            SetCredentials(config, HttpClientHandler);
        }

        private X509Certificate2 CaCert { get; }

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
                var cert = CertUtils.GeneratePfx(config);

#if NET452
                ((WebRequestHandler) handler).ClientCertificates.Add(cert);
#else
                handler.ClientCertificates.Add(cert);
#endif
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
            X509Certificate2 caCert,
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
                chain.ChainPolicy.ExtraStore.Add(caCert);
                chain.ChainPolicy.VerificationFlags = X509VerificationFlags.AllowUnknownCertificateAuthority;
                var isValid = chain.Build((X509Certificate2) certificate);
                return isValid;
            }

            // In all other cases, return false.
            return false;
        }
    }
}
