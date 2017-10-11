using System;
using System.IO;
using System.Security.Cryptography.X509Certificates;
using k8s.Exceptions;

namespace k8s
{
    public partial class KubernetesClientConfiguration
    {
        private const string ServiceAccountTokenKey = "token";
        private const string ServiceAccountRootCAKey = "ca.crt";

        public static KubernetesClientConfiguration InClusterConfig()
        {
            var host = Environment.GetEnvironmentVariable("KUBERNETES_SERVICE_HOST");
            var port = Environment.GetEnvironmentVariable("KUBERNETES_SERVICE_PORT");

            if (string.IsNullOrWhiteSpace(host) || string.IsNullOrWhiteSpace(port))
            {
                throw new KubeConfigException(
                    "unable to load in-cluster configuration, KUBERNETES_SERVICE_HOST and KUBERNETES_SERVICE_PORT must be defined");
            }

            var token = File.ReadAllText("/var/run/secrets/kubernetes.io/serviceaccount/" + ServiceAccountTokenKey);
            var rootCAFile = "/var/run/secrets/kubernetes.io/serviceaccount/" + ServiceAccountRootCAKey;

            return new KubernetesClientConfiguration
            {
                Host = new UriBuilder("https", host, Convert.ToInt32(port)).ToString(),
                AccessToken = token,
                SslCaCert = Utils.LoadPemFileCert(rootCAFile)
            };
        }
    }
}