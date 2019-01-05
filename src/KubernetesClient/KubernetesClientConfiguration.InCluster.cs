using System;
using System.IO;
using k8s.Exceptions;

namespace k8s
{
    public partial class KubernetesClientConfiguration
    {
        private const string ServiceaccountPath = "/var/run/secrets/kubernetes.io/serviceaccount/";
        private const string ServiceAccountTokenKeyFileName = "token";
        private const string ServiceAccountRootCAKeyFileName = "ca.crt";

        public static KubernetesClientConfiguration InClusterConfig()
        {
            var host = Environment.GetEnvironmentVariable("KUBERNETES_SERVICE_HOST");
            var port = Environment.GetEnvironmentVariable("KUBERNETES_SERVICE_PORT");

            if (string.IsNullOrWhiteSpace(host) || string.IsNullOrWhiteSpace(port))
            {
                throw new KubeConfigException(
                    "unable to load in-cluster configuration, KUBERNETES_SERVICE_HOST and KUBERNETES_SERVICE_PORT must be defined");
            }

            var token = File.ReadAllText(Path.Combine(ServiceaccountPath, ServiceAccountTokenKeyFileName));
            var rootCAFile = Path.Combine(ServiceaccountPath, ServiceAccountRootCAKeyFileName);

            return new KubernetesClientConfiguration
            {
                Host = new UriBuilder("https", host, Convert.ToInt32(port)).ToString(),
                AccessToken = token,
                SslCaCert = CertUtils.LoadPemFileCert(rootCAFile)
            };
        }
    }
}
