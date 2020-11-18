using System;
using System.Net.Http;

namespace k8s
{
    public partial class KubernetesClientConfiguration
    {
        public HttpClientHandler CreateDefaultHttpClientHandler()
        {
            var httpClientHandler = new HttpClientHandler();

            var uriScheme = new Uri(this.Host).Scheme;

            if (uriScheme == "https")
            {
                if (this.SkipTlsVerify)
                {
                    httpClientHandler.ServerCertificateCustomValidationCallback =
                        (sender, certificate, chain, sslPolicyErrors) => true;
                }
                else
                {
                    httpClientHandler.ServerCertificateCustomValidationCallback =
                        (sender, certificate, chain, sslPolicyErrors) =>
                        {
                            return Kubernetes.CertificateValidationCallBack(sender, this.SslCaCerts, certificate, chain,
                                sslPolicyErrors);
                        };
                }
            }

            AddCertificates(httpClientHandler);

            return httpClientHandler;
        }

        public void AddCertificates(HttpClientHandler handler)
        {
            if ((!string.IsNullOrWhiteSpace(this.ClientCertificateData) ||
                 !string.IsNullOrWhiteSpace(this.ClientCertificateFilePath)) &&
                (!string.IsNullOrWhiteSpace(this.ClientCertificateKeyData) ||
                 !string.IsNullOrWhiteSpace(this.ClientKeyFilePath)))
            {
                var cert = CertUtils.GeneratePfx(this);

                handler.ClientCertificates.Add(cert);
            }
        }

        public static DelegatingHandler CreateWatchHandler() => new WatcherDelegatingHandler();
    }
}
