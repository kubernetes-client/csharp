using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using k8s.Authentication;
using k8s.Exceptions;
using k8s.KubeConfigModels;
using System.Net;

namespace k8s
{
    public partial class KubernetesClientConfiguration
    {
        /// <summary>
        ///     kubeconfig Default Location
        /// </summary>
        public static readonly string KubeConfigDefaultLocation =
            RuntimeInformation.IsOSPlatform(OSPlatform.Windows)
                ? Path.Combine(Environment.GetEnvironmentVariable("USERPROFILE"), @".kube\config")
                : Path.Combine(Environment.GetEnvironmentVariable("HOME"), ".kube/config");

        /// <summary>
        ///     Gets CurrentContext
        /// </summary>
        public string CurrentContext { get; private set; }

        // For testing
        internal static string KubeConfigEnvironmentVariable { get; set; } = "KUBECONFIG";

        /// <summary>
        ///     Initializes a new instance of the <see cref="KubernetesClientConfiguration" /> from default locations
        ///     If the KUBECONFIG environment variable is set, then that will be used.
        ///     Next, it looks for a config file at <see cref="KubeConfigDefaultLocation"/>.
        ///     Then, it checks whether it is executing inside a cluster and will use <see cref="InClusterConfig()" />.
        ///     Finally, if nothing else exists, it creates a default config with localhost:8080 as host.
        /// </summary>
        /// <remarks>
        ///     If multiple kubeconfig files are specified in the KUBECONFIG environment variable,
        ///     merges the files, where first occurrence wins. See https://kubernetes.io/docs/concepts/configuration/organize-cluster-access-kubeconfig/#merging-kubeconfig-files.
        /// </remarks>
        /// <returns>Instance of the<see cref="KubernetesClientConfiguration"/> class</returns>
        public static KubernetesClientConfiguration BuildDefaultConfig()
        {
            var kubeconfig = Environment.GetEnvironmentVariable(KubeConfigEnvironmentVariable);
            if (kubeconfig != null)
            {
                var configList = kubeconfig.Split(RuntimeInformation.IsOSPlatform(OSPlatform.Windows) ? ';' : ':')
                    .Select((s) => new FileInfo(s.Trim('"')));
                var k8sConfig = LoadKubeConfig(configList.ToArray());
                return BuildConfigFromConfigObject(k8sConfig);
            }

            if (File.Exists(KubeConfigDefaultLocation))
            {
                return BuildConfigFromConfigFile(KubeConfigDefaultLocation);
            }

            if (IsInCluster())
            {
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
        /// <returns>Instance of the<see cref="KubernetesClientConfiguration"/> class</returns>
        public static KubernetesClientConfiguration BuildConfigFromConfigFile(
            string kubeconfigPath = null,
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
        /// <returns>Instance of the<see cref="KubernetesClientConfiguration"/> class</returns>
        public static KubernetesClientConfiguration BuildConfigFromConfigFile(
            FileInfo kubeconfig,
            string currentContext = null, string masterUrl = null, bool useRelativePaths = true)
        {
            return BuildConfigFromConfigFileAsync(kubeconfig, currentContext, masterUrl, useRelativePaths).GetAwaiter()
                .GetResult();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="KubernetesClientConfiguration" /> from config file
        /// </summary>
        /// <param name="kubeconfig">Fileinfo of the kubeconfig, cannot be null</param>
        /// <param name="currentContext">override the context in config file, set null if do not want to override</param>
        /// <param name="masterUrl">override the kube api server endpoint, set null if do not want to override</param>
        /// <param name="useRelativePaths">When <see langword="true"/>, the paths in the kubeconfig file will be considered to be relative to the directory in which the kubeconfig
        /// file is located. When <see langword="false"/>, the paths will be considered to be relative to the current working directory.</param>
        /// <returns>Instance of the<see cref="KubernetesClientConfiguration"/> class</returns>
        public static async Task<KubernetesClientConfiguration> BuildConfigFromConfigFileAsync(
            FileInfo kubeconfig,
            string currentContext = null, string masterUrl = null, bool useRelativePaths = true)
        {
            if (kubeconfig == null)
            {
                throw new NullReferenceException(nameof(kubeconfig));
            }

            var k8SConfig = await LoadKubeConfigAsync(kubeconfig, useRelativePaths).ConfigureAwait(false);
            var k8SConfiguration = GetKubernetesClientConfiguration(currentContext, masterUrl, k8SConfig);

            return k8SConfiguration;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="KubernetesClientConfiguration" /> from config file
        /// </summary>
        /// <param name="kubeconfig">Stream of the kubeconfig, cannot be null</param>
        /// <param name="currentContext">Override the current context in config, set null if do not want to override</param>
        /// <param name="masterUrl">Override the Kubernetes API server endpoint, set null if do not want to override</param>
        /// <returns>Instance of the<see cref="KubernetesClientConfiguration"/> class</returns>
        public static KubernetesClientConfiguration BuildConfigFromConfigFile(
            Stream kubeconfig,
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
        /// <returns>Instance of the<see cref="KubernetesClientConfiguration"/> class</returns>
        public static async Task<KubernetesClientConfiguration> BuildConfigFromConfigFileAsync(
            Stream kubeconfig,
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

            var k8SConfig = await Yaml.LoadFromStreamAsync<K8SConfiguration>(kubeconfig).ConfigureAwait(false);
            var k8SConfiguration = GetKubernetesClientConfiguration(currentContext, masterUrl, k8SConfig);

            return k8SConfiguration;
        }

        /// <summary>
        /// Initializes a new instance of <see cref="KubernetesClientConfiguration"/> from pre-loaded config object.
        /// </summary>
        /// <param name="k8SConfig">A <see cref="K8SConfiguration"/>, for example loaded from <see cref="LoadKubeConfigAsync(string, bool)" /></param>
        /// <param name="currentContext">Override the current context in config, set null if do not want to override</param>
        /// <param name="masterUrl">Override the Kubernetes API server endpoint, set null if do not want to override</param>
        /// <returns>Instance of the<see cref="KubernetesClientConfiguration"/> class</returns>
        public static KubernetesClientConfiguration BuildConfigFromConfigObject(
            K8SConfiguration k8SConfig,
            string currentContext = null, string masterUrl = null)
            => GetKubernetesClientConfiguration(currentContext, masterUrl, k8SConfig);

        private static KubernetesClientConfiguration GetKubernetesClientConfiguration(
            string currentContext,
            string masterUrl, K8SConfiguration k8SConfig)
        {
            if (k8SConfig == null)
            {
                throw new ArgumentNullException(nameof(k8SConfig));
            }

            var k8SConfiguration = new KubernetesClientConfiguration();

            currentContext = currentContext ?? k8SConfig.CurrentContext;
            // only init context if context is set
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
        ///     Validates and Initializes Client Configuration
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

            if (string.IsNullOrEmpty(activeContext.ContextDetails?.Cluster))
            {
                // This serves as validation for any of the properties of ContextDetails being set.
                // Other locations in code assume that ContextDetails is non-null.
                throw new KubeConfigException($"Cluster not set for context `{currentContext}` in kubeconfig");
            }

            CurrentContext = activeContext.Name;

            // cluster
            SetClusterDetails(k8SConfig, activeContext);

            // user
            SetUserDetails(k8SConfig, activeContext);

            // namespace
            Namespace = activeContext.ContextDetails?.Namespace;
        }

        private void SetClusterDetails(K8SConfiguration k8SConfig, Context activeContext)
        {
            var clusterDetails =
                k8SConfig.Clusters.FirstOrDefault(c => c.Name.Equals(
                    activeContext.ContextDetails.Cluster,
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

            if (!Uri.TryCreate(Host, UriKind.Absolute, out var uri))
            {
                throw new KubeConfigException($"Bad server host URL `{Host}` (cannot be parsed)");
            }

            if (IPAddress.TryParse(uri.Host, out var ipAddress))
            {
                if (IPAddress.Equals(IPAddress.Any, ipAddress))
                {
                    var builder = new UriBuilder(Host);
                    builder.Host = $"{IPAddress.Loopback}";
                    Host = builder.ToString();
                }
                else if (IPAddress.Equals(IPAddress.IPv6Any, ipAddress))
                {
                    var builder = new UriBuilder(Host);
                    builder.Host = $"{IPAddress.IPv6Loopback}";
                    Host = builder.ToString();
                }
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
                    SslCaCerts = new X509Certificate2Collection(new X509Certificate2(GetFullPath(
                        k8SConfig,
                        clusterDetails.ClusterEndpoint.CertificateAuthority)));
                }
            }
        }

        private void SetUserDetails(K8SConfiguration k8SConfig, Context activeContext)
        {
            if (string.IsNullOrWhiteSpace(activeContext.ContextDetails.User))
            {
                return;
            }

            var userDetails = k8SConfig.Users.FirstOrDefault(c => c.Name.Equals(
                activeContext.ContextDetails.User,
                StringComparison.OrdinalIgnoreCase));

            if (userDetails == null)
            {
                throw new KubeConfigException($"User not found for context {activeContext.Name} in kubeconfig");
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
                    && (userDetails.UserCredentials.AuthProvider.Config.ContainsKey("access-token")
                        || userDetails.UserCredentials.AuthProvider.Config.ContainsKey("id-token")))
                {
                    switch (userDetails.UserCredentials.AuthProvider.Name)
                    {
                        case "azure":
                            {
                                var config = userDetails.UserCredentials.AuthProvider.Config;
                                if (config.ContainsKey("expires-on"))
                                {
                                    var expiresOn = int.Parse(config["expires-on"]);
                                    DateTimeOffset expires;
                                    expires = DateTimeOffset.FromUnixTimeSeconds(expiresOn);

                                    if (DateTimeOffset.Compare(
                                        expires,
                                        DateTimeOffset.Now)
                                        <= 0)
                                    {
                                        var tenantId = config["tenant-id"];
                                        var clientId = config["client-id"];
                                        var apiServerId = config["apiserver-id"];
                                        var refresh = config["refresh-token"];
                                        var newToken = RenewAzureToken(
                                            tenantId,
                                            clientId,
                                            apiServerId,
                                            refresh);
                                        config["access-token"] = newToken;
                                    }
                                }

                                AccessToken = config["access-token"];
                                userCredentialsFound = true;
                                break;
                            }

                        case "gcp":
                            {
                                // config
                                var config = userDetails.UserCredentials.AuthProvider.Config;
                                TokenProvider = new GcpTokenProvider(config["cmd-path"]);
                                userCredentialsFound = true;
                                break;
                            }

                        case "oidc":
                            {
                                var config = userDetails.UserCredentials.AuthProvider.Config;
                                AccessToken = config["id-token"];
                                if (config.ContainsKey("client-id")
                                    && config.ContainsKey("idp-issuer-url")
                                    && config.ContainsKey("id-token")
                                    && config.ContainsKey("refresh-token"))
                                {
                                    string clientId = config["client-id"];
                                    string clientSecret = config.ContainsKey("client-secret") ? config["client-secret"] : null;
                                    string idpIssuerUrl = config["idp-issuer-url"];
                                    string idToken = config["id-token"];
                                    string refreshToken = config["refresh-token"];

                                    TokenProvider = new OidcTokenProvider(clientId, clientSecret, idpIssuerUrl, idToken, refreshToken);

                                    userCredentialsFound = true;
                                }

                                break;
                            }
                    }
                }
            }

            if (userDetails.UserCredentials.ExternalExecution != null)
            {
                if (string.IsNullOrWhiteSpace(userDetails.UserCredentials.ExternalExecution.Command))
                {
                    throw new KubeConfigException(
                        "External command execution to receive user credentials must include a command to execute");
                }

                if (string.IsNullOrWhiteSpace(userDetails.UserCredentials.ExternalExecution.ApiVersion))
                {
                    throw new KubeConfigException("External command execution missing ApiVersion key");
                }

                var response = ExecuteExternalCommand(userDetails.UserCredentials.ExternalExecution);
                AccessToken = response.Status.Token;
                // When reading ClientCertificateData from a config file it will be base64 encoded, and code later in the system (see CertUtils.GeneratePfx)
                // expects ClientCertificateData and ClientCertificateKeyData to be base64 encoded because of this. However the string returned by external
                // auth providers is the raw certificate and key PEM text, so we need to take that and base64 encoded it here so it can be decoded later.
                ClientCertificateData = response.Status.ClientCertificateData == null ? null : Convert.ToBase64String(System.Text.Encoding.ASCII.GetBytes(response.Status.ClientCertificateData));
                ClientCertificateKeyData = response.Status.ClientKeyData == null ? null : Convert.ToBase64String(System.Text.Encoding.ASCII.GetBytes(response.Status.ClientKeyData));

                userCredentialsFound = true;

                // TODO: support client certificates here too.
                if (AccessToken != null)
                {
                    TokenProvider = new ExecTokenProvider(userDetails.UserCredentials.ExternalExecution);
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

        public static Process CreateRunnableExternalProcess(ExternalExecution config)
        {
            if (config == null)
            {
                throw new ArgumentNullException(nameof(config));
            }

            var execInfo = new Dictionary<string, dynamic>
            {
                { "apiVersion", config.ApiVersion },
                { "kind", "ExecCredentials" },
                { "spec", new Dictionary<string, bool> { { "interactive", Environment.UserInteractive } } },
            };

            var process = new Process();

            process.StartInfo.EnvironmentVariables.Add("KUBERNETES_EXEC_INFO", JsonSerializer.Serialize(execInfo));
            if (config.EnvironmentVariables != null)
            {
                foreach (var configEnvironmentVariable in config.EnvironmentVariables)
                {
                    if (configEnvironmentVariable.ContainsKey("name") && configEnvironmentVariable.ContainsKey("value"))
                    {
                        var name = configEnvironmentVariable["name"];
                        process.StartInfo.EnvironmentVariables[name] = configEnvironmentVariable["value"];
                    }
                    else
                    {
                        var badVariable = string.Join(",", configEnvironmentVariable.Select(x => $"{x.Key}={x.Value}"));
                        throw new KubeConfigException($"Invalid environment variable defined: {badVariable}");
                    }
                }
            }

            process.StartInfo.FileName = config.Command;
            if (config.Arguments != null)
            {
                process.StartInfo.Arguments = string.Join(" ", config.Arguments);
            }

            process.StartInfo.RedirectStandardOutput = true;
            process.StartInfo.RedirectStandardError = true;
            process.StartInfo.UseShellExecute = false;

            return process;
        }

        /// <summary>
        /// Implementation of the proposal for out-of-tree client
        /// authentication providers as described here --
        /// https://github.com/kubernetes/community/blob/master/contributors/design-proposals/auth/kubectl-exec-plugins.md
        /// Took inspiration from python exec_provider.py --
        /// https://github.com/kubernetes-client/python-base/blob/master/config/exec_provider.py
        /// </summary>
        /// <param name="config">The external command execution configuration</param>
        /// <returns>
        /// The token, client certificate data, and the client key data received from the external command execution
        /// </returns>
        public static ExecCredentialResponse ExecuteExternalCommand(ExternalExecution config)
        {
            if (config == null)
            {
                throw new ArgumentNullException(nameof(config));
            }

            var process = CreateRunnableExternalProcess(config);

            try
            {
                process.Start();
            }
            catch (Exception ex)
            {
                throw new KubeConfigException($"external exec failed due to: {ex.Message}");
            }

            var stdout = process.StandardOutput.ReadToEnd();
            var stderr = process.StandardError.ReadToEnd();
            if (string.IsNullOrWhiteSpace(stderr) == false)
            {
                throw new KubeConfigException($"external exec failed due to: {stderr}");
            }

            // Wait for a maximum of 5 seconds, if a response takes longer probably something went wrong...
            process.WaitForExit(5);

            try
            {
                var responseObject = KubernetesJson.Deserialize<ExecCredentialResponse>(stdout);
                if (responseObject == null || responseObject.ApiVersion != config.ApiVersion)
                {
                    throw new KubeConfigException(
                        $"external exec failed because api version {responseObject.ApiVersion} does not match {config.ApiVersion}");
                }

                if (responseObject.Status.IsValid())
                {
                    return responseObject;
                }
                else
                {
                    throw new KubeConfigException($"external exec failed missing token or clientCertificateData field in plugin output");
                }
            }
            catch (JsonException ex)
            {
                throw new KubeConfigException($"external exec failed due to failed deserialization process: {ex}");
            }
            catch (Exception ex)
            {
                throw new KubeConfigException($"external exec failed due to uncaught exception: {ex}");
            }
        }

        /// <summary>
        ///     Loads entire Kube Config from default or explicit file path
        /// </summary>
        /// <param name="kubeconfigPath">Explicit file path to kubeconfig. Set to null to use the default file path</param>
        /// <param name="useRelativePaths">When <see langword="true"/>, the paths in the kubeconfig file will be considered to be relative to the directory in which the kubeconfig
        /// file is located. When <see langword="false"/>, the paths will be considered to be relative to the current working directory.</param>
        /// <returns>Instance of the <see cref="K8SConfiguration"/> class</returns>
        public static async Task<K8SConfiguration> LoadKubeConfigAsync(
            string kubeconfigPath = null,
            bool useRelativePaths = true)
        {
            var fileInfo = new FileInfo(kubeconfigPath ?? KubeConfigDefaultLocation);

            return await LoadKubeConfigAsync(fileInfo, useRelativePaths).ConfigureAwait(false);
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
        public static async Task<K8SConfiguration> LoadKubeConfigAsync(
            FileInfo kubeconfig,
            bool useRelativePaths = true)
        {
            if (kubeconfig == null)
            {
                throw new ArgumentNullException(nameof(kubeconfig));
            }


            if (!kubeconfig.Exists)
            {
                throw new KubeConfigException($"kubeconfig file not found at {kubeconfig.FullName}");
            }

            using (var stream = kubeconfig.OpenRead())
            {
                var config = await Yaml.LoadFromStreamAsync<K8SConfiguration>(stream).ConfigureAwait(false);

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
            return await Yaml.LoadFromStreamAsync<K8SConfiguration>(kubeconfigStream).ConfigureAwait(false);
        }

        /// <summary>
        ///     Loads Kube Config
        /// </summary>
        /// <param name="kubeconfigStream">Kube config file contents stream</param>
        /// <returns>Instance of the <see cref="K8SConfiguration"/> class</returns>
        public static K8SConfiguration LoadKubeConfig(Stream kubeconfigStream)
        {
            return LoadKubeConfigAsync(kubeconfigStream).GetAwaiter().GetResult();
        }

        /// <summary>
        ///     Loads Kube Config
        /// </summary>
        /// <param name="kubeConfigs">List of kube config file contents</param>
        /// <param name="useRelativePaths">When <see langword="true"/>, the paths in the kubeconfig file will be considered to be relative to the directory in which the kubeconfig
        /// file is located. When <see langword="false"/>, the paths will be considered to be relative to the current working directory.</param>
        /// <returns>Instance of the <see cref="K8SConfiguration"/> class</returns>
        /// <remarks>
        ///     The kube config files will be merges into a single <see cref="K8SConfiguration"/>, where first occurrence wins.
        ///     See https://kubernetes.io/docs/concepts/configuration/organize-cluster-access-kubeconfig/#merging-kubeconfig-files.
        /// </remarks>
        internal static K8SConfiguration LoadKubeConfig(FileInfo[] kubeConfigs, bool useRelativePaths = true)
        {
            return LoadKubeConfigAsync(kubeConfigs, useRelativePaths).GetAwaiter().GetResult();
        }

        /// <summary>
        ///     Loads Kube Config
        /// </summary>
        /// <param name="kubeConfigs">List of kube config file contents</param>
        /// <param name="useRelativePaths">When <see langword="true"/>, the paths in the kubeconfig file will be considered to be relative to the directory in which the kubeconfig
        /// file is located. When <see langword="false"/>, the paths will be considered to be relative to the current working directory.</param>
        /// <returns>Instance of the <see cref="K8SConfiguration"/> class</returns>
        /// <remarks>
        ///     The kube config files will be merges into a single <see cref="K8SConfiguration"/>, where first occurrence wins.
        ///     See https://kubernetes.io/docs/concepts/configuration/organize-cluster-access-kubeconfig/#merging-kubeconfig-files.
        /// </remarks>
        internal static async Task<K8SConfiguration> LoadKubeConfigAsync(
            FileInfo[] kubeConfigs,
            bool useRelativePaths = true)
        {
            var basek8SConfig = await LoadKubeConfigAsync(kubeConfigs[0], useRelativePaths).ConfigureAwait(false);

            for (var i = 1; i < kubeConfigs.Length; i++)
            {
                var mergek8SConfig = await LoadKubeConfigAsync(kubeConfigs[i], useRelativePaths).ConfigureAwait(false);
                MergeKubeConfig(basek8SConfig, mergek8SConfig);
            }

            return basek8SConfig;
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

        /// <summary>
        /// Merges kube config files together, preferring configuration present in the base config over the merge config.
        /// </summary>
        /// <param name="basek8SConfig">The <see cref="K8SConfiguration"/> to merge into</param>
        /// <param name="mergek8SConfig">The <see cref="K8SConfiguration"/> to merge from</param>
        private static void MergeKubeConfig(K8SConfiguration basek8SConfig, K8SConfiguration mergek8SConfig)
        {
            // For scalar values, prefer local values
            basek8SConfig.CurrentContext = basek8SConfig.CurrentContext ?? mergek8SConfig.CurrentContext;
            basek8SConfig.FileName = basek8SConfig.FileName ?? mergek8SConfig.FileName;

            // Kinds must match in kube config, otherwise throw.
            if (basek8SConfig.Kind != mergek8SConfig.Kind)
            {
                throw new KubeConfigException(
                    $"kubeconfig \"kind\" are different between {basek8SConfig.FileName} and {mergek8SConfig.FileName}");
            }

            if (mergek8SConfig.Preferences != null)
            {
                foreach (var preference in mergek8SConfig.Preferences)
                {
                    if (basek8SConfig.Preferences?.ContainsKey(preference.Key) == false)
                    {
                        basek8SConfig.Preferences[preference.Key] = preference.Value;
                    }
                }
            }

            // Note, Clusters, Contexts, and Extensions are map-like in config despite being represented as a list here:
            // https://github.com/kubernetes/client-go/blob/ede92e0fe62deed512d9ceb8bf4186db9f3776ff/tools/clientcmd/api/types.go#L238
            basek8SConfig.Extensions = MergeLists(basek8SConfig.Extensions, mergek8SConfig.Extensions, (s) => s.Name);
            basek8SConfig.Clusters = MergeLists(basek8SConfig.Clusters, mergek8SConfig.Clusters, (s) => s.Name);
            basek8SConfig.Users = MergeLists(basek8SConfig.Users, mergek8SConfig.Users, (s) => s.Name);
            basek8SConfig.Contexts = MergeLists(basek8SConfig.Contexts, mergek8SConfig.Contexts, (s) => s.Name);
        }

        private static IEnumerable<T> MergeLists<T>(IEnumerable<T> baseList, IEnumerable<T> mergeList,
            Func<T, string> getNameFunc)
        {
            if (mergeList != null && mergeList.Any())
            {
                var mapping = new Dictionary<string, T>();
                foreach (var item in baseList)
                {
                    mapping[getNameFunc(item)] = item;
                }

                foreach (var item in mergeList)
                {
                    var name = getNameFunc(item);
                    if (!mapping.ContainsKey(name))
                    {
                        mapping[name] = item;
                    }
                }

                return mapping.Values;
            }

            return baseList;
        }
    }
}
