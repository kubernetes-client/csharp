using System;
using System.IO;
using k8s.Authentication;
using k8s.Exceptions;

namespace k8s
{
    public partial class KubernetesClientConfiguration
    {
        private static string ServiceAccountPath =
            Path.Combine(new string[]
            {
                $"{Path.DirectorySeparatorChar}var", "run", "secrets", "kubernetes.io", "serviceaccount",
            });

        private const string ServiceAccountTokenKeyFileName = "token";
        private const string ServiceAccountRootCAKeyFileName = "ca.crt";

        public static Boolean IsInCluster()
        {
            var host = Environment.GetEnvironmentVariable("KUBERNETES_SERVICE_HOST");
            var port = Environment.GetEnvironmentVariable("KUBERNETES_SERVICE_PORT");
            if (string.IsNullOrWhiteSpace(host) || string.IsNullOrWhiteSpace(port))
            {
                return false;
            }

            var tokenPath = Path.Combine(ServiceAccountPath, ServiceAccountTokenKeyFileName);
            if (!File.Exists(tokenPath))
            {
                return false;
            }

            var certPath = Path.Combine(ServiceAccountPath, ServiceAccountRootCAKeyFileName);
            return File.Exists(certPath);
        }

        public static KubernetesClientConfiguration InClusterConfig()
        {
            if (!IsInCluster())
            {
                throw new KubeConfigException(
                    "unable to load in-cluster configuration, KUBERNETES_SERVICE_HOST and KUBERNETES_SERVICE_PORT must be defined");
            }

            var rootCAFile = Path.Combine(ServiceAccountPath, ServiceAccountRootCAKeyFileName);
            var host = Environment.GetEnvironmentVariable("KUBERNETES_SERVICE_HOST");
            var port = Environment.GetEnvironmentVariable("KUBERNETES_SERVICE_PORT");

            return new KubernetesClientConfiguration
            {
                Host = new UriBuilder("https", host, Convert.ToInt32(port)).ToString(),
                TokenProvider = new TokenFileAuth(Path.Combine(ServiceAccountPath, ServiceAccountTokenKeyFileName)),
                SslCaCerts = CertUtils.LoadPemFileCert(rootCAFile),
            };
        }
    }
}
