using System;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using k8s.Exceptions;
using k8s.KubeConfigModels;

namespace k8s
{
    public partial class KubernetesClientConfiguration
    {
        /// <summary>
        ///     kubeconfig Default Location
        /// </summary>
        private static readonly string KubeConfigDefaultLocation =
            RuntimeInformation.IsOSPlatform(OSPlatform.Windows)
                ? Path.Combine(Environment.GetEnvironmentVariable("USERPROFILE"), @".kube\config")
                : Path.Combine(Environment.GetEnvironmentVariable("HOME"), ".kube/config");

        /// <summary>
        ///     Gets CurrentContext
        /// </summary>
        public string CurrentContext { get; private set; }

        /// <summary>
        ///     Initializes a new instance of the <see cref="KubernetesClientConfiguration" /> from config file
        /// </summary>
        /// <param name="masterUrl">kube api server endpoint</param>
        /// <param name="kubeconfigPath">Explicit file path to kubeconfig. Set to null to use the default file path</param>
        public static KubernetesClientConfiguration BuildConfigFromConfigFile(string kubeconfigPath = null,
            string currentContext = null, string masterUrl = null)
        {
            return BuildConfigFromConfigFile(new FileInfo(kubeconfigPath ?? KubeConfigDefaultLocation), null,
                masterUrl);
        }

        /// <summary>
        /// </summary>
        /// <param name="kubeconfig">Fileinfo of the kubeconfig, cannot be null</param>
        /// <param name="currentContext">override the context in config file, set null if do not want to override</param>
        /// <param name="masterUrl">overrider kube api server endpoint, set null if do not want to override</param>
        public static KubernetesClientConfiguration BuildConfigFromConfigFile(FileInfo kubeconfig,
            string currentContext = null, string masterUrl = null)
        {
            if (kubeconfig == null)
            {
                throw new NullReferenceException(nameof(kubeconfig));
            }

            var k8SConfig = LoadKubeConfig(kubeconfig);
            var k8SConfiguration = GetKubernetesClientConfiguration(currentContext, masterUrl, k8SConfig);

            return k8SConfiguration;
        }

        /// <summary>
        /// </summary>
        /// <param name="kubeconfig">Fileinfo of the kubeconfig, cannot be null, whitespaced or empty</param>
        /// <param name="currentContext">override the context in config file, set null if do not want to override</param>
        /// <param name="masterUrl">overrider kube api server endpoint, set null if do not want to override</param>
        public static KubernetesClientConfiguration BuildConfigFromConfigFile(Stream kubeconfig,
            string currentContext = null, string masterUrl = null)
        {
            if (kubeconfig == null)
            {
                throw new NullReferenceException(nameof(kubeconfig));
            }

            if (!kubeconfig.CanSeek)
            {
                throw new Exception("Stream don't support seeking!");
            }

            kubeconfig.Position = 0;

            var k8SConfig = Yaml.LoadFromStreamAsync<K8SConfiguration>(kubeconfig).GetAwaiter().GetResult();
            var k8SConfiguration = GetKubernetesClientConfiguration(currentContext, masterUrl, k8SConfig);

            return k8SConfiguration;
        }

        private static KubernetesClientConfiguration GetKubernetesClientConfiguration(string currentContext, string masterUrl, K8SConfiguration k8SConfig)
        {
            var k8SConfiguration = new KubernetesClientConfiguration();

            currentContext = currentContext ?? k8SConfig.CurrentContext;
            // only init context if context if set
            if (currentContext != null)
            {
                k8SConfiguration.InitializeContext(k8SConfig, currentContext);
            }
            if (!string.IsNullOrWhiteSpace(masterUrl))
            {
                k8SConfiguration.Host = masterUrl;
            }

            if (string.IsNullOrWhiteSpace(k8SConfiguration.Host))
            {
                throw new KubeConfigException("Cannot infer server host url either from context or masterUrl");
            }

            return k8SConfiguration;
        }

        /// <summary>
        ///     Validates and Intializes Client Configuration
        /// </summary>
        /// <param name="k8SConfig">Kubernetes Configuration</param>
        /// <param name="currentContext">Current Context</param>
        private void InitializeContext(K8SConfiguration k8SConfig, string currentContext)
        {
            // current context
            var activeContext =
                k8SConfig.Contexts.FirstOrDefault(
                    c => c.Name.Equals(currentContext, StringComparison.OrdinalIgnoreCase));
            if (activeContext == null)
            {
                throw new KubeConfigException($"CurrentContext: {currentContext} not found in contexts in kubeconfig");
            }

            CurrentContext = activeContext.Name;

            // cluster
            SetClusterDetails(k8SConfig, activeContext);

            // user
            SetUserDetails(k8SConfig, activeContext);

            // namespace
            Namespace = activeContext.Namespace;
        }

        private void SetClusterDetails(K8SConfiguration k8SConfig, Context activeContext)
        {
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
            Host = clusterDetails.ClusterEndpoint.Server;

            SkipTlsVerify = clusterDetails.ClusterEndpoint.SkipTlsVerify;

            try
            {
                var uri = new Uri(Host);
                if (uri.Scheme == "https")
                {
                    // check certificate for https
                    if (!clusterDetails.ClusterEndpoint.SkipTlsVerify &&
                        string.IsNullOrWhiteSpace(clusterDetails.ClusterEndpoint.CertificateAuthorityData) &&
                        string.IsNullOrWhiteSpace(clusterDetails.ClusterEndpoint.CertificateAuthority))
                    {
                        throw new KubeConfigException(
                            $"neither certificate-authority-data nor certificate-authority not found for current-context :{activeContext} in kubeconfig");
                    }

                    if (!string.IsNullOrEmpty(clusterDetails.ClusterEndpoint.CertificateAuthorityData))
                    {
                        var data = clusterDetails.ClusterEndpoint.CertificateAuthorityData;
                        SslCaCert = new X509Certificate2(Convert.FromBase64String(data));
                    }
                    else if (!string.IsNullOrEmpty(clusterDetails.ClusterEndpoint.CertificateAuthority))
                    {
                        SslCaCert = new X509Certificate2(clusterDetails.ClusterEndpoint.CertificateAuthority);
                    }
                }
            }
            catch (UriFormatException e)
            {
                throw new KubeConfigException("Bad Server host url", e);
            }
        }

        private void SetUserDetails(K8SConfiguration k8SConfig, Context activeContext)
        {
            if (string.IsNullOrWhiteSpace(activeContext.ContextDetails.User))
            {
                return;
            }

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
                AccessToken = userDetails.UserCredentials.Token;
                userCredentialsFound = true;
            }
            else if (!string.IsNullOrWhiteSpace(userDetails.UserCredentials.UserName) &&
                     !string.IsNullOrWhiteSpace(userDetails.UserCredentials.Password))
            {
                Username = userDetails.UserCredentials.UserName;
                Password = userDetails.UserCredentials.Password;
                userCredentialsFound = true;
            }

            // Token and cert based auth can co-exist
            if (!string.IsNullOrWhiteSpace(userDetails.UserCredentials.ClientCertificateData) &&
                !string.IsNullOrWhiteSpace(userDetails.UserCredentials.ClientKeyData))
            {
                ClientCertificateData = userDetails.UserCredentials.ClientCertificateData;
                ClientCertificateKeyData = userDetails.UserCredentials.ClientKeyData;
                userCredentialsFound = true;
            }

            if (!string.IsNullOrWhiteSpace(userDetails.UserCredentials.ClientCertificate) &&
                !string.IsNullOrWhiteSpace(userDetails.UserCredentials.ClientKey))
            {
                ClientCertificateFilePath = userDetails.UserCredentials.ClientCertificate;
                ClientKeyFilePath = userDetails.UserCredentials.ClientKey;
                userCredentialsFound = true;
            }

            if (!userCredentialsFound)
            {
                throw new KubeConfigException(
                    $"User: {userDetails.Name} does not have appropriate auth credentials in kubeconfig");
            }
        }

        /// <summary>
        ///     Loads entire Kube Config from default or explicit file path
        /// </summary>
        /// <param name="kubeconfigPath">Explicit file path to kubeconfig. Set to null to use the default file path</param>
        /// <returns></returns>
        public static async Task<K8SConfiguration> LoadKubeConfigAsync(string kubeconfigPath = null)
        {
            var fileInfo = new FileInfo(kubeconfigPath ?? KubeConfigDefaultLocation);

            return await LoadKubeConfigAsync(fileInfo);
        }

        /// <summary>
        ///     Loads entire Kube Config from default or explicit file path
        /// </summary>
        /// <param name="kubeconfigPath">Explicit file path to kubeconfig. Set to null to use the default file path</param>
        /// <returns></returns>
        public static K8SConfiguration LoadKubeConfig(string kubeconfigPath = null)
        {
            return LoadKubeConfigAsync(kubeconfigPath).GetAwaiter().GetResult();
        }
        
        // <summary>
        ///     Loads Kube Config
        /// </summary>
        /// <param name="kubeconfig">Kube config file contents</param>
        /// <returns>Instance of the <see cref="K8SConfiguration"/> class</returns>
        public static async Task<K8SConfiguration> LoadKubeConfigAsync(FileInfo kubeconfig)
        {
            if (!kubeconfig.Exists)
            {
                throw new KubeConfigException($"kubeconfig file not found at {kubeconfig.FullName}");
            }

            using (var stream = kubeconfig.OpenRead())
            {
                return await Yaml.LoadFromStreamAsync<K8SConfiguration>(stream);
            }
        }

        /// <summary>
        ///     Loads Kube Config
        /// </summary>
        /// <param name="kubeconfig">Kube config file contents</param>
        /// <returns>Instance of the <see cref="K8SConfiguration"/> class</returns>
        public static K8SConfiguration LoadKubeConfig(FileInfo kubeconfig)
        {
            return LoadKubeConfigAsync(kubeconfig).GetAwaiter().GetResult();
        }

        // <summary>
        ///     Loads Kube Config
        /// </summary>
        /// <param name="kubeconfigStream">Kube config file contents stream</param>
        /// <returns>Instance of the <see cref="K8SConfiguration"/> class</returns>
        public static async Task<K8SConfiguration> LoadKubeConfigAsync(Stream kubeconfigStream)
        {
            return await Yaml.LoadFromStreamAsync<K8SConfiguration>(kubeconfigStream);
        }

        /// <summary>
        ///     Loads Kube Config
        /// </summary>
        /// <param name="kubeconfig">Kube config file contents stream</param>
        /// <returns>Instance of the <see cref="K8SConfiguration"/> class</returns>
        public static K8SConfiguration LoadKubeConfig(Stream kubeconfigStream)
        {
            return LoadKubeConfigAsync(kubeconfigStream).GetAwaiter().GetResult();
        }
    }
}
