using System;
using System.Collections.Generic;
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
        public static KubernetesClientConfiguration BuildDefaultConfig() {
            var kubeconfig = Environment.GetEnvironmentVariable("KUBECONFIG");
            if (kubeconfig != null) {
                return BuildConfigFromConfigFile(kubeconfigPath: kubeconfig);
            }
            if (File.Exists(KubeConfigDefaultLocation)) {
                return BuildConfigFromConfigFile(kubeconfigPath: KubeConfigDefaultLocation);
            }
            if (IsInCluster()) {
                return InClusterConfig();
            }
            var config = new KubernetesClientConfiguration();
            config.Host = "http://localhost:8080";
            return config;
        }


        /// <summary>
        ///     Initializes a new instance of the <see cref="KubernetesClientConfiguration" /> from config file
        /// </summary>
        /// <param name="kubeconfigPath">Explicit file path to kubeconfig. Set to null to use the default file path</param>
        /// <param name="currentContext">override the context in config file, set null if do not want to override</param>
        /// <param name="masterUrl">kube api server endpoint</param>
        /// <param name="useRelativePaths">When <see langword="true"/>, the paths in the kubeconfig file will be considered to be relative to the directory in which the kubeconfig
        /// file is located. When <see langword="false"/>, the paths will be considered to be relative to the current working directory.</param>
        public static KubernetesClientConfiguration BuildConfigFromConfigFile(string kubeconfigPath = null,
            string currentContext = null, string masterUrl = null, bool useRelativePaths = true)
        {
            return BuildConfigFromConfigFile(new FileInfo(kubeconfigPath ?? KubeConfigDefaultLocation), currentContext,
                masterUrl, useRelativePaths);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="KubernetesClientConfiguration" /> from config file
        /// </summary>
        /// <param name="kubeconfig">Fileinfo of the kubeconfig, cannot be null</param>
        /// <param name="currentContext">override the context in config file, set null if do not want to override</param>
        /// <param name="masterUrl">override the kube api server endpoint, set null if do not want to override</param>
        /// <param name="useRelativePaths">When <see langword="true"/>, the paths in the kubeconfig file will be considered to be relative to the directory in which the kubeconfig
        /// file is located. When <see langword="false"/>, the paths will be considered to be relative to the current working directory.</param>
        public static KubernetesClientConfiguration BuildConfigFromConfigFile(FileInfo kubeconfig,
            string currentContext = null, string masterUrl = null, bool useRelativePaths = true)
        {
            return BuildConfigFromConfigFileAsync(kubeconfig, currentContext, masterUrl, useRelativePaths).GetAwaiter().GetResult();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="KubernetesClientConfiguration" /> from config file
        /// </summary>
        /// <param name="kubeconfig">Fileinfo of the kubeconfig, cannot be null</param>
        /// <param name="currentContext">override the context in config file, set null if do not want to override</param>
        /// <param name="masterUrl">override the kube api server endpoint, set null if do not want to override</param>
        /// <param name="useRelativePaths">When <see langword="true"/>, the paths in the kubeconfig file will be considered to be relative to the directory in which the kubeconfig
        /// file is located. When <see langword="false"/>, the paths will be considered to be relative to the current working directory.</param>

        public static async Task<KubernetesClientConfiguration> BuildConfigFromConfigFileAsync(FileInfo kubeconfig,
            string currentContext = null, string masterUrl = null, bool useRelativePaths = true)
        {
            if (kubeconfig == null)
            {
                throw new NullReferenceException(nameof(kubeconfig));
            }

            var k8SConfig = await LoadKubeConfigAsync(kubeconfig, useRelativePaths);
            var k8SConfiguration = GetKubernetesClientConfiguration(currentContext, masterUrl, k8SConfig);

            return k8SConfiguration;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="KubernetesClientConfiguration" /> from config file
        /// </summary>
        /// <param name="kubeconfig">Stream of the kubeconfig, cannot be null</param>
        /// <param name="currentContext">Override the current context in config, set null if do not want to override</param>
        /// <param name="masterUrl">Override the Kubernetes API server endpoint, set null if do not want to override</param>
        public static KubernetesClientConfiguration BuildConfigFromConfigFile(Stream kubeconfig,
            string currentContext = null, string masterUrl = null)
        {
            return BuildConfigFromConfigFileAsync(kubeconfig, currentContext, masterUrl).GetAwaiter().GetResult();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="KubernetesClientConfiguration" /> from config file
        /// </summary>
        /// <param name="kubeconfig">Stream of the kubeconfig, cannot be null</param>
        /// <param name="currentContext">Override the current context in config, set null if do not want to override</param>
        /// <param name="masterUrl">Override the Kubernetes API server endpoint, set null if do not want to override</param>
        public static async Task<KubernetesClientConfiguration> BuildConfigFromConfigFileAsync(Stream kubeconfig,
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

            var k8SConfig = await Yaml.LoadFromStreamAsync<K8SConfiguration>(kubeconfig);
            var k8SConfiguration = GetKubernetesClientConfiguration(currentContext, masterUrl, k8SConfig);

            return k8SConfiguration;
        }

        /// <summary>
        /// Initializes a new instance of <see cref="KubernetesClientConfiguration"/> from pre-loaded config object.
        /// </summary>
        /// <param name="k8sConfig">A <see cref="K8SConfiguration"/>, for example loaded from <see cref="LoadKubeConfigAsync(string, bool)" /></param>
        /// <param name="currentContext">Override the current context in config, set null if do not want to override</param>
        /// <param name="masterUrl">Override the Kubernetes API server endpoint, set null if do not want to override</param>
        public static KubernetesClientConfiguration BuildConfigFromConfigObject(K8SConfiguration k8SConfig, string currentContext = null, string masterUrl = null)
            => GetKubernetesClientConfiguration(currentContext, masterUrl, k8SConfig);

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
                throw new KubeConfigException($"Cluster not found for context `{activeContext}` in kubeconfig");
            }
            if (string.IsNullOrWhiteSpace(clusterDetails.ClusterEndpoint.Server))
            {
                throw new KubeConfigException($"Server not found for current-context `{activeContext}` in kubeconfig");
            }

            Host = clusterDetails.ClusterEndpoint.Server;
            SkipTlsVerify = clusterDetails.ClusterEndpoint.SkipTlsVerify;

            if(!Uri.TryCreate(Host, UriKind.Absolute, out Uri uri))
            {
                throw new KubeConfigException($"Bad server host URL `{Host}` (cannot be parsed)");
            }

            if (uri.Scheme == "https")
            {
                if (!string.IsNullOrEmpty(clusterDetails.ClusterEndpoint.CertificateAuthorityData))
                {
                    var data = clusterDetails.ClusterEndpoint.CertificateAuthorityData;
                    SslCaCerts = new X509Certificate2Collection(new X509Certificate2(Convert.FromBase64String(data)));
                }
                else if (!string.IsNullOrEmpty(clusterDetails.ClusterEndpoint.CertificateAuthority))
                {
                    SslCaCerts = new X509Certificate2Collection(new X509Certificate2(GetFullPath(k8SConfig, clusterDetails.ClusterEndpoint.CertificateAuthority)));
                }
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
                ClientCertificateFilePath = GetFullPath(k8SConfig, userDetails.UserCredentials.ClientCertificate);
                ClientKeyFilePath = GetFullPath(k8SConfig, userDetails.UserCredentials.ClientKey);
                userCredentialsFound = true;
            }

            if (userDetails.UserCredentials.AuthProvider != null)
            {
                if (userDetails.UserCredentials.AuthProvider.Config != null
                 && userDetails.UserCredentials.AuthProvider.Config.ContainsKey("access-token"))
                {
                    switch (userDetails.UserCredentials.AuthProvider.Name)
                    {
                        case "azure":
                        {
                            var config = userDetails.UserCredentials.AuthProvider.Config;
                            if (config.ContainsKey("expires-on"))
                            {
                                var expiresOn = Int32.Parse(config["expires-on"]);
                                DateTimeOffset expires;
                                #if NET452
                                var epoch = new DateTimeOffset(1970, 1, 1, 0, 0, 0, TimeSpan.Zero);
                                expires = epoch.AddSeconds(expiresOn);
                                #else
                                expires = DateTimeOffset.FromUnixTimeSeconds(expiresOn);
                                #endif

                                if (DateTimeOffset.Compare(expires
                                                         , DateTimeOffset.Now)
                                 <= 0)
                                {
                                    var tenantId = config["tenant-id"];
                                    var clientId = config["client-id"];
                                    var apiServerId = config["apiserver-id"];
                                    var refresh = config["refresh-token"];
                                    var newToken = RenewAzureToken(tenantId
                                                                 , clientId
                                                                 , apiServerId
                                                                 , refresh);
                                    config["access-token"] = newToken;
                                }
                            }

                            AccessToken = config["access-token"];
                            userCredentialsFound = true;
                            break;
                        }
                        case "gcp":
                        {
                            var config = userDetails.UserCredentials.AuthProvider.Config;
                            const string keyExpire = "expiry";
                            if (config.ContainsKey(keyExpire))
                            {
                                if (DateTimeOffset.TryParse(config[keyExpire]
                                                          , out DateTimeOffset expires))
                                {
                                    if (DateTimeOffset.Compare(expires
                                                             , DateTimeOffset.Now)
                                     <= 0)
                                    {
                                        throw new KubeConfigException("Refresh not supported.");
                                    }
                                }
                            }

                            AccessToken = config["access-token"];
                            userCredentialsFound = true;
                            break;
                        }
                    }
                }
            }

            if (!userCredentialsFound)
            {
                throw new KubeConfigException(
                    $"User: {userDetails.Name} does not have appropriate auth credentials in kubeconfig");
            }
        }

        public static string RenewAzureToken(string tenantId, string clientId, string apiServerId, string refresh)
        {
            throw new KubeConfigException("Refresh not supported.");
        }

        /// <summary>
        ///     Loads entire Kube Config from default or explicit file path
        /// </summary>
        /// <param name="kubeconfigPath">Explicit file path to kubeconfig. Set to null to use the default file path</param>
        /// <param name="useRelativePaths">When <see langword="true"/>, the paths in the kubeconfig file will be considered to be relative to the directory in which the kubeconfig
        /// file is located. When <see langword="false"/>, the paths will be considered to be relative to the current working directory.</param>
        /// <returns>Instance of the <see cref="K8SConfiguration"/> class</returns>
        public static async Task<K8SConfiguration> LoadKubeConfigAsync(string kubeconfigPath = null, bool useRelativePaths = true)
        {
            var fileInfo = new FileInfo(kubeconfigPath ?? KubeConfigDefaultLocation);

            return await LoadKubeConfigAsync(fileInfo, useRelativePaths);
        }

        /// <summary>
        ///     Loads entire Kube Config from default or explicit file path
        /// </summary>
        /// <param name="kubeconfigPath">Explicit file path to kubeconfig. Set to null to use the default file path</param>
        /// <param name="useRelativePaths">When <see langword="true"/>, the paths in the kubeconfig file will be considered to be relative to the directory in which the kubeconfig
        /// file is located. When <see langword="false"/>, the paths will be considered to be relative to the current working directory.</param>
        /// <returns>Instance of the <see cref="K8SConfiguration"/> class</returns>
        public static K8SConfiguration LoadKubeConfig(string kubeconfigPath = null, bool useRelativePaths = true)
        {
            return LoadKubeConfigAsync(kubeconfigPath, useRelativePaths).GetAwaiter().GetResult();
        }

        /// <summary>
        ///     Loads Kube Config
        /// </summary>
        /// <param name="kubeconfig">Kube config file contents</param>
        /// <param name="useRelativePaths">When <see langword="true"/>, the paths in the kubeconfig file will be considered to be relative to the directory in which the kubeconfig
        /// file is located. When <see langword="false"/>, the paths will be considered to be relative to the current working directory.</param>
        /// <returns>Instance of the <see cref="K8SConfiguration"/> class</returns>
        public static async Task<K8SConfiguration> LoadKubeConfigAsync(FileInfo kubeconfig, bool useRelativePaths = true)
        {
            if (!kubeconfig.Exists)
            {
                throw new KubeConfigException($"kubeconfig file not found at {kubeconfig.FullName}");
            }

            using (var stream = kubeconfig.OpenRead())
            {
                var config = await Yaml.LoadFromStreamAsync<K8SConfiguration>(stream);

                if (useRelativePaths)
                {
                    config.FileName = kubeconfig.FullName;
                }

                return config;
            }
        }

        /// <summary>
        ///     Loads Kube Config
        /// </summary>
        /// <param name="kubeconfig">Kube config file contents</param>
        /// <param name="useRelativePaths">When <see langword="true"/>, the paths in the kubeconfig file will be considered to be relative to the directory in which the kubeconfig
        /// file is located. When <see langword="false"/>, the paths will be considered to be relative to the current working directory.</param>
        /// <returns>Instance of the <see cref="K8SConfiguration"/> class</returns>
        public static K8SConfiguration LoadKubeConfig(FileInfo kubeconfig, bool useRelativePaths = true)
        {
            return LoadKubeConfigAsync(kubeconfig, useRelativePaths).GetAwaiter().GetResult();
        }

        /// <summary>
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

        /// <summary>
        /// Tries to get the full path to a file referenced from the Kubernetes configuration.
        /// </summary>
        /// <param name="configuration">
        /// The Kubernetes configuration.
        /// </param>
        /// <param name="path">
        /// The path to resolve.
        /// </param>
        /// <returns>
        /// When possible a fully qualified path to the file.
        /// </returns>
        /// <remarks>
        /// For example, if the configuration file is at "C:\Users\me\kube.config" and path is "ca.crt",
        /// this will return "C:\Users\me\ca.crt". Similarly, if path is "D:\ca.cart", this will return
        /// "D:\ca.crt".
        /// </remarks>
        private static string GetFullPath(K8SConfiguration configuration, string path)
        {
            // If we don't have a file name,
            if (string.IsNullOrWhiteSpace(configuration.FileName) || Path.IsPathRooted(path))
            {
                return path;
            }
            else
            {
                return Path.Combine(Path.GetDirectoryName(configuration.FileName), path);
            }
        }
    }
}
