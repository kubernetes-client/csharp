namespace k8s.Util.Common
{
    public class Config
    {
        public const string ServiceAccountRoot = "/var/run/secrets/kubernetes.io/serviceaccount";
        public const string ServiceAccountCaPath = ServiceAccountRoot + "/ca.crt";
        public const string ServiceAccountTokenPath = ServiceAccountRoot + "/token";
        public const string ServiceAccountNamespacePath = ServiceAccountRoot + "/namespace";
        public const string EnvKubeconfig = "KUBECONFIG";
        public const string EnvServiceHost = "KUBERNETES_SERVICE_HOST";
        public const string EnvServicePort = "KUBERNETES_SERVICE_PORT";

        // The last resort host to try
        public const string DefaultFallbackHost = "http://localhost:8080";
    }
}
