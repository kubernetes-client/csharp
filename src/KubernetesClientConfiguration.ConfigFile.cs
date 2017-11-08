using System;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security.Cryptography.X509Certificates;
using k8s.Exceptions;
using k8s.KubeConfigModels;
using YamlDotNet.Serialization;

namespace k8s
{
    public partial class KubernetesClientConfiguration
    {
        /// <summary>
        /// Gets CurrentContext
        /// </summary>
        public string CurrentContext { get; private set; }

        /// <summary>
        /// kubeconfig Default Location
        /// </summary>
        private static readonly string KubeConfigDefaultLocation =
            RuntimeInformation.IsOSPlatform(OSPlatform.Windows)
                ? Path.Combine(Environment.GetEnvironmentVariable("USERPROFILE"), @".kube\config")
                : Path.Combine(Environment.GetEnvironmentVariable("HOME"), ".kube/config");

        /// <summary>
        /// Initializes a new instance of the <see cref="KubernetesClientConfiguration"/> class.
        /// </summary>
        /// <param name="kubeconfig">kubeconfig file info</param>
        /// <param name="currentContext">Context to use from kube config</param>
        public KubernetesClientConfiguration(FileInfo kubeconfig = null, string currentContext = null)
        {
            var k8SConfig = LoadKubeConfig(kubeconfig ?? new FileInfo(KubeConfigDefaultLocation));
            this.Initialize(k8SConfig, currentContext);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="KubernetesClientConfiguration"/> from config file
        /// </summary>
        /// <param name="masterUrl">kube api server endpoint</param>
        /// <param name="kubeconfigPath">kubeconfig filepath</param>
        public static KubernetesClientConfiguration BuildConfigFromConfigFile(string masterUrl = null, string kubeconfigPath = null)
        {
            return BuildConfigFromConfigFile(new FileInfo(kubeconfigPath ?? KubeConfigDefaultLocation), null, masterUrl);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="kubeconfig">Fileinfo of the kubeconfig, cannot be null</param>
        /// <param name="currentContext">override the context in config file, set null if do not want to override</param>
        /// <param name="masterUrl">overrider kube api server endpoint, set null if do not want to override</param>
        public static KubernetesClientConfiguration BuildConfigFromConfigFile(FileInfo kubeconfig, string currentContext = null, string masterUrl = null)
        {
            if (kubeconfig == null)
            {
                throw new NullReferenceException(nameof(kubeconfig));
            }

            var k8SConfig = LoadKubeConfig(kubeconfig);
            var k8SConfiguration = new KubernetesClientConfiguration();
            k8SConfiguration.Initialize(k8SConfig);

            if (!string.IsNullOrWhiteSpace(masterUrl))
            {
                k8SConfiguration.Host = masterUrl;
            }
            return k8SConfiguration;
        }
            

        /// <summary>
        /// Validates and Intializes Client Configuration
        /// </summary>
        /// <param name="k8SConfig">Kubernetes Configuration</param>
        /// <param name="currentContext">Current Context</param>
        private void Initialize(K8SConfiguration k8SConfig, string currentContext = null)
        {
            if (k8SConfig.Contexts == null)
            {
                throw new KubeConfigException("No contexts found in kubeconfig");
            }

            if (k8SConfig.Clusters == null)
            {
                throw new KubeConfigException($"No clusters found in kubeconfig");
            }

            if (k8SConfig.Users == null)
            {
                throw new KubeConfigException($"No users found in kubeconfig");
            }

            // current context
            currentContext = currentContext ?? k8SConfig.CurrentContext;
            Context activeContext =
                k8SConfig.Contexts.FirstOrDefault(
                    c => c.Name.Equals(currentContext, StringComparison.OrdinalIgnoreCase));
            if (activeContext == null)
            {
                throw new KubeConfigException($"CurrentContext: {currentContext} not found in contexts in kubeconfig");
            }

            this.CurrentContext = activeContext.Name;

            // cluster
            var clusterDetails =
                k8SConfig.Clusters.FirstOrDefault(c => c.Name.Equals(activeContext.ContextDetails.Cluster,
                    StringComparison.OrdinalIgnoreCase));
            if (clusterDetails?.ClusterEndpoint == null)
            {
                throw new KubeConfigException($"Cluster not found for context {activeContext} in kubeconfig");
            }

            if (string.IsNullOrWhiteSpace(clusterDetails.ClusterEndpoint.Server))
            {
                throw new KubeConfigException($"Server not found for current-context {activeContext} in kubeconfig");
            }

            if (!clusterDetails.ClusterEndpoint.SkipTlsVerify &&
                string.IsNullOrWhiteSpace(clusterDetails.ClusterEndpoint.CertificateAuthorityData) &&
                string.IsNullOrWhiteSpace(clusterDetails.ClusterEndpoint.CertificateAuthority))
            {
                throw new KubeConfigException(
                    $"neither certificate-authority-data nor certificate-authority not found for current-context :{activeContext} in kubeconfig");
            }

            this.Host = clusterDetails.ClusterEndpoint.Server;
            if (!string.IsNullOrEmpty(clusterDetails.ClusterEndpoint.CertificateAuthorityData))
            {
                string data = clusterDetails.ClusterEndpoint.CertificateAuthorityData;
                this.SslCaCert = new X509Certificate2(Convert.FromBase64String(data));
            }
            else if (!string.IsNullOrEmpty(clusterDetails.ClusterEndpoint.CertificateAuthority))
            {
                this.SslCaCert = new X509Certificate2(clusterDetails.ClusterEndpoint.CertificateAuthority);
            }
            this.SkipTlsVerify = clusterDetails.ClusterEndpoint.SkipTlsVerify;

            // user
            this.SetUserDetails(k8SConfig, activeContext);
        }

        private void SetUserDetails(K8SConfiguration k8SConfig, Context activeContext)
        {
            var userDetails = k8SConfig.Users.FirstOrDefault(c => c.Name.Equals(activeContext.ContextDetails.User,
                StringComparison.OrdinalIgnoreCase));

            if (userDetails == null)
            {
                throw new KubeConfigException("User not found for context {activeContext.Name} in kubeconfig");
            }

            if (userDetails.UserCredentials == null)
            {
                throw new KubeConfigException($"User credentials not found for user: {userDetails.Name} in kubeconfig");
            }

            var userCredentialsFound = false;

            // Basic and bearer tokens are mutually exclusive
            if (!string.IsNullOrWhiteSpace(userDetails.UserCredentials.Token))
            {
                this.AccessToken = userDetails.UserCredentials.Token;
                userCredentialsFound = true;
            }
            else if (!string.IsNullOrWhiteSpace(userDetails.UserCredentials.UserName) &&
                     !string.IsNullOrWhiteSpace(userDetails.UserCredentials.Password))
            {
                this.Username = userDetails.UserCredentials.UserName;
                this.Password = userDetails.UserCredentials.Password;
                userCredentialsFound = true;
            }

            // Token and cert based auth can co-exist
            if (!string.IsNullOrWhiteSpace(userDetails.UserCredentials.ClientCertificateData) &&
                !string.IsNullOrWhiteSpace(userDetails.UserCredentials.ClientKeyData))
            {
                this.ClientCertificateData = userDetails.UserCredentials.ClientCertificateData;
                this.ClientCertificateKeyData = userDetails.UserCredentials.ClientKeyData;
                userCredentialsFound = true;
            }

            if (!string.IsNullOrWhiteSpace(userDetails.UserCredentials.ClientCertificate) &&
                !string.IsNullOrWhiteSpace(userDetails.UserCredentials.ClientKey))
            {
                this.ClientCertificateFilePath = userDetails.UserCredentials.ClientCertificate;
                this.ClientKeyFilePath = userDetails.UserCredentials.ClientKey;
                userCredentialsFound = true;
            }

            if (!userCredentialsFound)
            {
                throw new KubeConfigException(
                    $"User: {userDetails.Name} does not have appropriate auth credentials in kubeconfig");
            }
        }

        /// <summary>
        /// Loads Kube Config
        /// </summary>
        /// <param name="config">Kube config file contents</param>
        /// <returns>Instance of the <see cref="K8SConfiguration"/> class</returns>
        private static K8SConfiguration LoadKubeConfig(FileInfo kubeconfig)
        {
            if (!kubeconfig.Exists)
            {
                throw new KubeConfigException($"kubeconfig file not found at {kubeconfig.FullName}");
            }
            var kubeconfigContent = File.ReadAllText(kubeconfig.FullName);

            var deserializeBuilder = new DeserializerBuilder();
            var deserializer = deserializeBuilder.Build();
            return deserializer.Deserialize<K8SConfiguration>(kubeconfigContent);
        }
    }
}
