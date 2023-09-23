using k8s.Authentication;
using k8s.Exceptions;

namespace k8s
{
    public partial class KubernetesClientConfiguration
    {
#pragma warning disable SA1401
        // internal for testing
        internal static string ServiceAccountPath =
            Path.Combine(new string[]
            {
                $"{Path.DirectorySeparatorChar}var", "run", "secrets", "kubernetes.io", "serviceaccount",
            });
#pragma warning restore SA1401

        internal const string ServiceAccountTokenKeyFileName = "token";
        internal const string ServiceAccountRootCAKeyFileName = "ca.crt";
        internal const string ServiceAccountNamespaceFileName = "namespace";

        public static bool IsInCluster()
        {
            var host = Environment.GetEnvironmentVariable("KUBERNETES_SERVICE_HOST");
            var port = Environment.GetEnvironmentVariable("KUBERNETES_SERVICE_PORT");

            if (string.IsNullOrEmpty(host) || string.IsNullOrEmpty(port))
            {
                return false;
            }

            var tokenPath = Path.Combine(ServiceAccountPath, ServiceAccountTokenKeyFileName);
            if (!FileSystem.Current.Exists(tokenPath))
            {
                return false;
            }

            var certPath = Path.Combine(ServiceAccountPath, ServiceAccountRootCAKeyFileName);
            return FileSystem.Current.Exists(certPath);
        }

        public static KubernetesClientConfiguration InClusterConfig()
        {
            if (!IsInCluster())
            {
                throw new KubeConfigException(
                    "Unable to load in-cluster configuration. Missing environment variables KUBERNETES_SERVICE_HOST and KUBERNETES_SERVICE_PORT or service account token. Hint: consider using option \"automountServiceAccountToken: true\" in deployment declaration.");
            }

            var rootCAFile = Path.Combine(ServiceAccountPath, ServiceAccountRootCAKeyFileName);
            var host = Environment.GetEnvironmentVariable("KUBERNETES_SERVICE_HOST");
            var port = Environment.GetEnvironmentVariable("KUBERNETES_SERVICE_PORT");
            if (string.IsNullOrEmpty(host))
            {
                host = "kubernetes.default.svc";
            }

            if (string.IsNullOrEmpty(port))
            {
                port = "443";
            }

            var result = new KubernetesClientConfiguration
            {
                Host = new UriBuilder("https", host, Convert.ToInt32(port)).ToString(),
                TokenProvider = new TokenFileAuth(Path.Combine(ServiceAccountPath, ServiceAccountTokenKeyFileName)),
                SslCaCerts = CertUtils.LoadPemFileCert(rootCAFile),
            };

            var namespaceFile = Path.Combine(ServiceAccountPath, ServiceAccountNamespaceFileName);
            if (FileSystem.Current.Exists(namespaceFile))
            {
                result.Namespace = FileSystem.Current.ReadAllText(namespaceFile);
            }

            return result;
        }
    }
}
