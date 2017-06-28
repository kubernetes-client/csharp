namespace k8s
{
    using System;
    using System.IO;
    using System.Linq;
    using k8s.Exceptions;
    using k8s.KubeConfigModels;
    using YamlDotNet.Serialization;
    using System.Runtime.InteropServices;

    /// <summary>
    /// Represents a set of kubernetes client configuration settings
    /// </summary>
    public class KubernetesClientConfiguration
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="KubernetesClientConfiguration"/> class.
        /// Initializes a new instance of the ClientConfiguration class
        /// </summary>
        /// <param name="kubeconfig">kubeconfig file info</param>
        /// <param name="currentContext">Context to use from kube config</param>
        public KubernetesClientConfiguration(FileInfo kubeconfig = null, string currentContext = null)
        {
            if (kubeconfig == null)
            {
                kubeconfig = new FileInfo(KubeConfigDefaultLocation);
            }
            var k8SConfig = this.LoadKubeConfig(kubeconfig);
            this.Initialize(k8SConfig, currentContext);
        }

        /// <summary>
        /// kubeconfig Default Location
        /// </summary>
        private static readonly string KubeConfigDefaultLocation = RuntimeInformation.IsOSPlatform(OSPlatform.Windows) ?
            Path.Combine(Environment.GetEnvironmentVariable("USERPROFILE"), @".kube\config") :
            Path.Combine(Environment.GetEnvironmentVariable("HOME"), ".kube/config");

        /// <summary>
        /// Gets CurrentContext
        /// </summary>
        public string CurrentContext { get; private set; }

        /// <summary>
        /// Gets Host
        /// </summary>
        public string Host { get; private set; }

        /// <summary>
        /// Gets SslCaCert
        /// </summary>
        public string SslCaCert { get; private set; }

        /// <summary>
        /// Gets ClientCertificateData
        /// </summary>
        public string ClientCertificateData { get; private set; }

        /// <summary>
        /// Gets ClientCertificate Key
        /// </summary>
        public string ClientCertificateKey { get; private set; }

        /// <summary>
        /// Gets a value indicating whether to skip ssl server cert validation
        /// </summary>
        public bool SkipTlsVerify { get; private set; }

        /// <summary>
        /// Gets or sets the HTTP user agent.
        /// </summary>
        /// <value>Http user agent.</value>
        public string UserAgent { get; set; }

        /// <summary>
        /// Gets or sets the username (HTTP basic authentication).
        /// </summary>
        /// <value>The username.</value>
        public string Username { get; set; }

        /// <summary>
        /// Gets or sets the password (HTTP basic authentication).
        /// </summary>
        /// <value>The password.</value>
        public string Password { get; set; }

        /// <summary>
        /// Gets or sets the access token for OAuth2 authentication.
        /// </summary>
        /// <value>The access token.</value>
        public string AccessToken { get; set; }

        /// <summary>
        /// Validates and Intializes Client Configuration
        /// </summary>
        /// <param name="k8SConfig">Kubernetes Configuration</param>
        /// <param name="currentContext">Current Context</param>
        private void Initialize(K8SConfiguration k8SConfig, string currentContext = null)
        {
            Context activeContext;

            // set the currentCOntext to passed context if not null
            if (!string.IsNullOrWhiteSpace(currentContext))
            {
                if (k8SConfig.Contexts == null)
                {
                    throw new KubeConfigException("No contexts found in kubeconfig");
                }

                activeContext = k8SConfig.Contexts.FirstOrDefault(c => c.Name.Equals(currentContext, StringComparison.OrdinalIgnoreCase));
                if (activeContext != null)
                {
                    this.CurrentContext = activeContext.Name;
                }
                else
                {
                    throw new KubeConfigException($"CurrentContext: {0} not found in contexts in kubeconfig");
                }
            }
            // otherwise set current context from kubeconfig
            else
            {
                activeContext = k8SConfig.Contexts.FirstOrDefault(c => c.Name.Equals(k8SConfig.CurrentContext, StringComparison.OrdinalIgnoreCase));

                if (activeContext == null)
                {
                    throw new KubeConfigException($"CurrentContext: {currentContext} not found in contexts in kubeconfig");
                }

                this.CurrentContext = activeContext.Name;
            }

            var clusterDetails = k8SConfig.Clusters.FirstOrDefault(c => c.Name.Equals(activeContext.ContextDetails.Cluster, StringComparison.OrdinalIgnoreCase));
            if (clusterDetails?.ClusterEndpoint != null)
            {
                if (string.IsNullOrWhiteSpace(clusterDetails.ClusterEndpoint.Server))
                {
                    throw new KubeConfigException($"server not found for current-context :{activeContext} in kubeconfig");
                }

                if (!clusterDetails.ClusterEndpoint.SkipTlsVerify &&
                    string.IsNullOrWhiteSpace(clusterDetails.ClusterEndpoint.CertificateAuthorityData))
                {
                    throw new KubeConfigException($"certificate-authority-data not found for current-context :{activeContext} in kubeconfig");
                }

                this.Host = clusterDetails.ClusterEndpoint.Server;
                this.SslCaCert = clusterDetails.ClusterEndpoint.CertificateAuthorityData;
                this.SkipTlsVerify = clusterDetails.ClusterEndpoint.SkipTlsVerify;
            }
            else
            {
                throw new KubeConfigException($"Cluster details not found for current-context: {activeContext} in kubeconfig");
            }

            // set user details from kubeconfig
            var userDetails = k8SConfig.Users.FirstOrDefault(c => c.Name.Equals(activeContext.ContextDetails.User, StringComparison.OrdinalIgnoreCase));

            this.SetUserDetails(userDetails);
        }

        private void SetUserDetails(User userDetails)
        {
            if (userDetails == null)
            {
                throw new KubeConfigException("User not found for the current context in kubeconfig");
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
                this.ClientCertificateKey = userDetails.UserCredentials.ClientKeyData;
                userCredentialsFound = true;
            }

            if (!userCredentialsFound)
            {
                throw new KubeConfigException($"User: {userDetails.Name} does not have appropriate auth credentials in kube config");
            }
        }

        /// <summary>
        /// Loads Kube Config
        /// </summary>
        /// <param name="config">Kube config file contents</param>
        /// <returns>Instance of the <see cref="K8SConfiguration"/> class</returns>
        private K8SConfiguration LoadKubeConfig(FileInfo kubeconfig)
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
