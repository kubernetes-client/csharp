namespace k8s.Util.Common
{
    public static class Config
    {
        public static string ServiceAccountCaPath => KubernetesClientConfiguration.ServiceAccountPath + "/ca.crt";
        public static string ServiceAccountTokenPath => KubernetesClientConfiguration.ServiceAccountPath + "/token";
        public static string ServiceAccountNamespacePath => KubernetesClientConfiguration.ServiceAccountPath + "/namespace";
        public static string EnvKubeconfig => "KUBECONFIG";
        public static string EnvServiceHost => "KUBERNETES_SERVICE_HOST";
        public static string EnvServicePort => "KUBERNETES_SERVICE_PORT";

        // The last resort host to try
        public static string DefaultFallbackHost => "http://localhost:8080";
    }
}
