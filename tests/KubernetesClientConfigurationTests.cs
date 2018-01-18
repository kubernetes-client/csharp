using System.IO;
using k8s.Exceptions;
using Xunit;

namespace k8s.Tests
{
    public class KubernetesClientConfigurationTests
    {
        /// <summary>
        ///     Check if host is properly loaded, per context
        /// </summary>
        [Theory]
        [InlineData("federal-context", "https://horse.org:4443")]
        [InlineData("queen-anne-context", "https://pig.org:443")]
        public void ContextHost(string context, string host)
        {
            var fi = new FileInfo("assets/kubeconfig.yml");
            var cfg = KubernetesClientConfiguration.BuildConfigFromConfigFile(fi, context);
            Assert.Equal(host, cfg.Host);
        }

        /// <summary>
        ///     Checks if user-based token is loaded properly from the config file, per context
        /// </summary>
        /// <param name="context"></param>
        /// <param name="token"></param>
        [Theory]
        [InlineData("queen-anne-context", "black-token")]
        public void ContextUserToken(string context, string token)
        {
            var fi = new FileInfo("assets/kubeconfig.yml");
            var cfg = KubernetesClientConfiguration.BuildConfigFromConfigFile(fi, context);
            Assert.Equal(context, cfg.CurrentContext);
            Assert.Null(cfg.Username);
            Assert.Equal(token, cfg.AccessToken);
        }

        /// <summary>
        ///     Checks if certificate-based authentication is loaded properly from the config file, per context
        /// </summary>
        /// <param name="context">Context to retreive the configuration</param>
        /// <param name="clientCert">'client-certificate-data' node content</param>
        /// <param name="clientCertKey">'client-key-data' content</param>
        [Theory]
        [InlineData("federal-context", "assets/client.crt", "assets/client.key")]
        public void ContextCertificate(string context, string clientCert, string clientCertKey)
        {
            var fi = new FileInfo("assets/kubeconfig.yml");
            var cfg = KubernetesClientConfiguration.BuildConfigFromConfigFile(fi, context);
            Assert.Equal(context, cfg.CurrentContext);
            Assert.Equal(cfg.ClientCertificateFilePath, clientCert);
            Assert.Equal(cfg.ClientKeyFilePath, clientCertKey);
        }

        /// <summary>
        ///     Checks if certificate-based authentication is loaded properly from the config file, per context
        /// </summary>
        /// <param name="context">Context to retreive the configuration</param>
        [Theory]
        [InlineData("victorian-context")]
        public void ClientData(string context)
        {
            var fi = new FileInfo("assets/kubeconfig.yml");
            var cfg = KubernetesClientConfiguration.BuildConfigFromConfigFile(fi, context);
            Assert.Equal(context, cfg.CurrentContext);
            Assert.NotNull(cfg.SslCaCert);
            Assert.Equal(File.ReadAllText("assets/client-certificate-data.txt"), cfg.ClientCertificateData);
            Assert.Equal(File.ReadAllText("assets/client-key-data.txt"), cfg.ClientCertificateKeyData);
        }

        /// <summary>
        ///     Checks that a KubeConfigException is thrown when no certificate-authority-data is set and user do not require tls
        ///     skip
        /// </summary>
        [Fact]
        public void CheckClusterTlsCorrectness()
        {
            var fi = new FileInfo("assets/kubeconfig.tls-no-skip-error.yml");
            Assert.Throws<KubeConfigException>(() => KubernetesClientConfiguration.BuildConfigFromConfigFile(fi));
        }

        /// <summary>
        ///     Checks that a KubeConfigException is thrown when no certificate-authority-data is set and user do not require tls
        ///     skip
        /// </summary>
        [Fact]
        public void CheckClusterTlsSkipCorrectness()
        {
            var fi = new FileInfo("assets/kubeconfig.tls-skip.yml");
            var cfg = KubernetesClientConfiguration.BuildConfigFromConfigFile(fi);
            Assert.NotNull(cfg.Host);
            Assert.Null(cfg.SslCaCert);
            Assert.True(cfg.SkipTlsVerify);
        }

        /// <summary>
        ///     Checks that a KubeConfigException is thrown when the cluster defined in clusters and contexts do not match
        /// </summary>
        [Fact]
        public void ClusterNameMissmatch()
        {
            var fi = new FileInfo("assets/kubeconfig.cluster-missmatch.yml");
            Assert.Throws<KubeConfigException>(() => KubernetesClientConfiguration.BuildConfigFromConfigFile(fi));
        }

        /// <summary>
        ///     Checks that a KubeConfigException is thrown when the clusters section is missing
        /// </summary>
        [Fact]
        public void ClusterNotFound()
        {
            var fi = new FileInfo("assets/kubeconfig.no-cluster.yml");
            Assert.Throws<KubeConfigException>(() => KubernetesClientConfiguration.BuildConfigFromConfigFile(fi));
        }

        /// <summary>
        ///     The configuration file is not present. An KubeConfigException should be thrown
        /// </summary>
        [Fact]
        public void ConfigurationFileNotFound()
        {
            var fi = new FileInfo("/path/to/nowhere");
            Assert.Throws<KubeConfigException>(() => KubernetesClientConfiguration.BuildConfigFromConfigFile(fi));
        }


        /// <summary>
        ///     Test that an Exception is thrown when initializating a KubernetClientConfiguration whose config file Context is not
        ///     present
        /// </summary>
        [Fact]
        public void ContextNotFound()
        {
            var fi = new FileInfo("assets/kubeconfig.yml");
            Assert.Throws<KubeConfigException>(() =>
                KubernetesClientConfiguration.BuildConfigFromConfigFile(fi, "context-not-found"));
        }

        /// <summary>
        ///     Checks Host is loaded from the default configuration file
        /// </summary>
        [Fact]
        public void DefaultConfigurationLoaded()
        {
            var cfg = KubernetesClientConfiguration.BuildConfigFromConfigFile(new FileInfo("assets/kubeconfig.yml"));
            Assert.NotNull(cfg.Host);
        }

        /// <summary>
        ///     Checks that a KubeConfigException is thrown when incomplete user credentials
        /// </summary>
        [Fact]
        public void IncompleteUserCredentials()
        {
            var fi = new FileInfo("assets/kubeconfig.no-credentials.yml");
            Assert.Throws<KubeConfigException>(() => KubernetesClientConfiguration.BuildConfigFromConfigFile(fi));
        }

        /// <summary>
        ///     Test if KubeConfigException is thrown when no Contexts and we use the default context name
        /// </summary>
        [Fact]
        public void NoContexts()
        {
            var fi = new FileInfo("assets/kubeconfig.no-context.yml");
            Assert.Throws<KubeConfigException>(() => KubernetesClientConfiguration.BuildConfigFromConfigFile(fi));
        }

        /// <summary>
        ///     Test if KubeConfigException is thrown when no Contexts are set and we specify a concrete context name
        /// </summary>
        [Fact]
        public void NoContextsExplicit()
        {
            var fi = new FileInfo("assets/kubeconfig-no-context.yml");
            Assert.Throws<KubeConfigException>(() =>
                KubernetesClientConfiguration.BuildConfigFromConfigFile(fi, "context"));
        }

        /// <summary>
        ///     Checks that a KubeConfigException is thrown when the server property is not set in cluster
        /// </summary>
        [Fact]
        public void ServerNotFound()
        {
            var fi = new FileInfo("assets/kubeconfig.no-server.yml");
            Assert.Throws<KubeConfigException>(() => KubernetesClientConfiguration.BuildConfigFromConfigFile(fi));
        }

        /// <summary>
        ///     Checks user/password authentication information is read properly
        /// </summary>
        [Fact]
        public void UserPasswordAuthentication()
        {
            var fi = new FileInfo("assets/kubeconfig.user-pass.yml");
            var cfg = KubernetesClientConfiguration.BuildConfigFromConfigFile(fi);
            Assert.Equal("admin", cfg.Username);
            Assert.Equal("secret", cfg.Password);
        }

        /// <summary>
        ///     Checks that a KubeConfigException is thrown when user cannot be found in users
        /// </summary>
        [Fact]
        public void UserNotFound()
        {
            var fi = new FileInfo("assets/kubeconfig.user-not-found.yml");
            Assert.Throws<KubeConfigException>(() => KubernetesClientConfiguration.BuildConfigFromConfigFile(fi));
        }

        /// <summary>
        ///     Make sure that user is not a necessary field. set #issue 24
        /// </summary>
        [Fact]
        public void EmptyUserNotFound()
        {
            var fi = new FileInfo("assets/kubeconfig.no-user.yml");
            var cfg = KubernetesClientConfiguration.BuildConfigFromConfigFile(fi);

            Assert.NotEmpty(cfg.Host);
        }

        /// <summary>
        ///     Make sure Host is replaced by masterUrl
        /// </summary>
        [Fact]
        public void OverrideByMasterUrl()
        {
            var fi = new FileInfo("assets/kubeconfig.yml");
            var cfg = KubernetesClientConfiguration.BuildConfigFromConfigFile(fi, masterUrl: "http://test.server");
            Assert.Equal("http://test.server", cfg.Host);
        }

        /// <summary>
        ///     Make sure that http urls are loaded even if insecure-skip-tls-verify === true
        /// </summary>
        [Fact]
        public void SmartSkipTlsVerify()
        {
            var fi = new FileInfo("assets/kubeconfig.tls-skip-http.yml");
            var cfg = KubernetesClientConfiguration.BuildConfigFromConfigFile(fi);
            Assert.False(cfg.SkipTlsVerify);
            Assert.Equal("http://horse.org", cfg.Host);
        }

        /// <summary>
        ///     Checks config could work well when current-context is not set but masterUrl is set. #issue 24
        /// </summary>
        [Fact]
        public void NoCurrentContext()
        {
            var fi = new FileInfo("assets/kubeconfig.no-current-context.yml");

            // failed if cannot infer any server host
            Assert.Throws<KubeConfigException>(() => KubernetesClientConfiguration.BuildConfigFromConfigFile(fi));

            // survive when masterUrl is set
            var cfg = KubernetesClientConfiguration.BuildConfigFromConfigFile(fi, masterUrl: "http://test.server");
            Assert.Equal("http://test.server", cfg.Host);
        }

        /// <summary>
        ///     Checks that loading a configuration from a file leaves no outstanding handles to the file.
        /// </summary>
        /// <remarks>
        ///     This test fails only on Windows.
        /// </remarks>
        [Fact]
        public void DeletedConfigurationFile()
        {
            var assetFileInfo = new FileInfo("assets/kubeconfig.yml");
            var tempFileInfo = new FileInfo(Path.GetTempFileName());
            
            File.Copy(assetFileInfo.FullName, tempFileInfo.FullName, /* overwrite: */ true);

            KubernetesClientConfiguration config;

            try
            {
                config = KubernetesClientConfiguration.BuildConfigFromConfigFile(tempFileInfo);
            }
            finally
            {
                File.Delete(tempFileInfo.FullName);
            }
        }

        /// <summary>
        ///     Checks Host is loaded from the default configuration file as string
        /// </summary>
        [Fact]
        public void DefaultConfigurationAsStringLoaded()
        {
            var txt = File.ReadAllText("assets/kubeconfig.yml");

            var cfg = KubernetesClientConfiguration.BuildConfigFromConfigFile(txt, null, null);
            Assert.NotNull(cfg.Host);
        }


        /// <summary>
        ///     Checks Host is loaded from the default configuration file as stream
        /// </summary>
        [Fact]
        public void DefaultConfigurationAsStreamLoaded()
        {
            using (var stream = File.OpenRead("assets/kubeconfig.yml"))
            {
                var cfg = KubernetesClientConfiguration.BuildConfigFromConfigFile(stream);
                Assert.NotNull(cfg.Host);
            }
        }
    }
}
