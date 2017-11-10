using Xunit;
using System.IO;

namespace k8s.Tests
{
    public class KubernetesClientConfigurationTests
    {

        public static string readLine(string fileName)
        {
            StreamReader reader = new StreamReader(new FileStream(fileName, FileMode.Open, FileAccess.Read));
            return reader.ReadLine();
        }

        /// <summary>
        /// This file contains a sample kubeconfig file
        /// </summary>
        private static readonly string kubeConfigFileName = "assets/kubeconfig.yml";

        /// <summary>
        /// Invalid test file with no context on purpose
        /// </summary>
        private static readonly string kubeConfigNoContexts = "assets/kubeconfig-no-context.yml";

        /// <summary>
        /// Sample configuration file with user/password authentication
        /// </summary>
        private static readonly string kubeConfigUserPassword = "assets/kubeconfig.user-pass.yml";

        /// <summary>
        /// Sample configuration file with incorrect user credentials structures on purpose
        /// </summary>
        private static readonly string kubeConfigNoCredentials = "assets/kubeconfig.no-credentials.yml";

        /// <summary>
        /// Sample configuration file with incorrect cluster/server structure on purpose
        /// </summary>
        private static readonly string kubeConfigNoServer = "assets/kubeconfig.no-server.yml";

        /// <summary>
        /// Sample configuration file with incorrect cluster/server structure on purpose
        /// </summary>
        private static readonly string kubeConfigNoCluster = "assets/kubeconfig.no-cluster.yml";

        /// <summary>
        /// Sample configuration file with incorrect match in cluster name
        /// </summary>
        private static readonly string kubeConfigClusterMissmatch = "assets/kubeconfig.cluster-missmatch.yml";

        /// <summary>
        /// Sample configuration file with incorrect TLS configuration in cluster section
        /// </summary>
        private static readonly string kubeConfigTlsNoSkipError = "assets/kubeconfig.tls-no-skip-error.yml";

        /// <summary>
        /// Sample configuration file with incorrect TLS configuration in cluster section
        /// </summary>
        private static readonly string kubeConfigTlsSkip = "assets/kubeconfig.tls-skip.yml";

        /// <summary>
        /// The configuration file is not present. An KubeConfigException should be thrown
        /// </summary>
        [Fact]
        public void ConfigurationFileNotFound()
        {
            var fi = new FileInfo("/path/to/nowhere");
            Assert.Throws<k8s.Exceptions.KubeConfigException>(() => new KubernetesClientConfiguration(fi));
        }

        /// <summary>
        /// Checks Host is loaded from the default configuration file
        /// </summary>
        [Fact]
        public void DefaultConfigurationLoaded()
        {
            var cfg = new KubernetesClientConfiguration(new FileInfo(kubeConfigFileName));
            Assert.NotNull(cfg.Host);
        }

        /// <summary>
        /// Check if host is properly loaded, per context
        /// </summary>
        [Theory]
        [InlineData("federal-context", "https://horse.org:4443")]
        [InlineData("queen-anne-context", "https://pig.org:443")]
        public void ContextHost(string context, string host)
        {
            var fi = new FileInfo(kubeConfigFileName);
            var cfg = new KubernetesClientConfiguration(fi, context);
            Assert.Equal(host, cfg.Host);
        }

        /// <summary>
        /// Checks if user-based token is loaded properly from the config file, per context
        /// </summary>
        /// <param name="context"></param>
        /// <param name="token"></param>
        [Theory]
        [InlineData("queen-anne-context", "black-token")]
        public void ContextUserToken(string context, string token)
        {
            var fi = new FileInfo(kubeConfigFileName);
            var cfg = new KubernetesClientConfiguration(fi, context);
            Assert.Equal(context, cfg.CurrentContext);
            Assert.Null(cfg.Username);
            Assert.Equal(token, cfg.AccessToken);
        }

        /// <summary>
        /// Checks if certificate-based authentication is loaded properly from the config file, per context
        /// </summary>
        /// <param name="context">Context to retreive the configuration</param>
        /// <param name="clientCertData">'client-certificate-data' node content</param>
        /// <param name="clientCertKey">'client-key-data' content</param>
        [Theory]
        [InlineData("federal-context", "assets/client.crt", "assets/client.key")]
        public void ContextCertificateTest(string context, string clientCert, string clientCertKey)
        {
            var fi = new FileInfo(kubeConfigFileName);
            var cfg = new KubernetesClientConfiguration(fi, context);
            Assert.Equal(context, cfg.CurrentContext);
            Assert.Equal(cfg.ClientCertificateFilePath, clientCert);
            Assert.Equal(cfg.ClientKeyFilePath, clientCertKey);
        }

        /// <summary>
        /// Checks if certificate-based authentication is loaded properly from the config file, per context
        /// </summary>
        /// <param name="context">Context to retreive the configuration</param>
        [Theory]
        [InlineData("victorian-context")]
        public void ClientDataTest(string context)
        {
            var fi = new FileInfo(kubeConfigFileName);
            var cfg = new KubernetesClientConfiguration(fi, context);
            Assert.Equal(context, cfg.CurrentContext);
            Assert.NotNull(cfg.SslCaCert);
            Assert.Equal(readLine("assets/client-certificate-data.txt"), cfg.ClientCertificateData);
            Assert.Equal(readLine("assets/client-key-data.txt"), cfg.ClientCertificateKeyData);
        }


        /// <summary>
        /// Test that an Exception is thrown when initializating a KubernetClientConfiguration whose config file Context is not present
        /// </summary>
        [Fact]
        public void ContextNotFound()
        {
            var fi = new FileInfo(kubeConfigFileName);
            Assert.Throws<k8s.Exceptions.KubeConfigException>(() => new KubernetesClientConfiguration(fi, "context-not-found"));
        }

        /// <summary>
        /// Test if KubeConfigException is thrown when no Contexts and we use the default context name
        /// </summary>
        [Fact]
        public void NoContexts()
        {
            var fi = new FileInfo(kubeConfigNoContexts);
            Assert.Throws<k8s.Exceptions.KubeConfigException>(() => new KubernetesClientConfiguration(fi));
        }

        /// <summary>
        /// Test if KubeConfigException is thrown when no Contexts are set and we specify a concrete context name
        /// </summary>
        [Fact]
        public void NoContextsExplicit()
        {
            var fi = new FileInfo(kubeConfigNoContexts);
            Assert.Throws<k8s.Exceptions.KubeConfigException>(() => new KubernetesClientConfiguration(fi, "context"));
        }

        /// <summary>
        /// Checks user/password authentication information is read properly
        /// </summary>
        [Fact]
        public void UserPasswordAuthentication()
        {
            var fi = new FileInfo(kubeConfigUserPassword);
            var cfg = new KubernetesClientConfiguration(fi);
            Assert.Equal("admin", cfg.Username);
            Assert.Equal("secret", cfg.Password);
        }

        /// <summary>
        /// Checks that a KubeConfigException is thrown when incomplete user credentials
        /// </summary>
        [Fact]
        public void IncompleteUserCredentials()
        {
            var fi = new FileInfo(kubeConfigNoCredentials);
            Assert.Throws<k8s.Exceptions.KubeConfigException>(() => new KubernetesClientConfiguration(fi));
        }

        /// <summary>
        /// Checks that a KubeConfigException is thrown when the server property is not set in cluster
        /// </summary>
        [Fact]
        public void ServerNotFound()
        {
            var fi = new FileInfo(kubeConfigNoServer);
            Assert.Throws<k8s.Exceptions.KubeConfigException>(() => new KubernetesClientConfiguration(fi));
        }

        /// <summary>
        /// Checks that a KubeConfigException is thrown when the clusters section is missing
        /// </summary>
        [Fact]
        public void ClusterNotFound()
        {
            var fi = new FileInfo(kubeConfigNoCluster);
            Assert.Throws<k8s.Exceptions.KubeConfigException>(() => new KubernetesClientConfiguration(fi));
        }

        /// <summary>
        /// Checks that a KubeConfigException is thrown when the cluster defined in clusters and contexts do not match
        /// </summary>
        [Fact]
        public void ClusterNameMissmatch()
        {
            var fi = new FileInfo(kubeConfigClusterMissmatch);
            Assert.Throws<k8s.Exceptions.KubeConfigException>(() => new KubernetesClientConfiguration(fi));
        }

        /// <summary>
        /// Checks that a KubeConfigException is thrown when no certificate-authority-data is set and user do not require tls skip 
        /// </summary>
        [Fact]
        public void CheckClusterTlsCorrectness()
        {
            var fi = new FileInfo(kubeConfigTlsNoSkipError);
            Assert.Throws<k8s.Exceptions.KubeConfigException>(() => new KubernetesClientConfiguration(fi));
        }

        /// <summary>
        /// Checks that a KubeConfigException is thrown when no certificate-authority-data is set and user do not require tls skip 
        /// </summary>
        [Fact]
        public void CheckClusterTlsSkipCorrectness()
        {
            var fi = new FileInfo(kubeConfigTlsSkip);
            var cfg = new KubernetesClientConfiguration(fi);
            Assert.NotNull(cfg.Host);
            Assert.Null(cfg.SslCaCert);
            Assert.True(cfg.SkipTlsVerify);
        }

        // /// <summary>
        // /// Checks if the are pods
        // /// </summary>
        // [Fact]
        // public void ListDefaultNamespacedPod()
        // {
        //     var k8sClientConfig = new KubernetesClientConfiguration();
        //     IKubernetes client = new Kubernetes(k8sClientConfig);
        //     var listTask = client.ListNamespacedPodWithHttpMessagesAsync("default").Result;
        //     var list = listTask.Body;
        //     Assert.NotEqual(0, list.Items.Count);            
        // }        
    }
}
