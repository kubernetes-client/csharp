using k8s.Exceptions;
using k8s.KubeConfigModels;
using System;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
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
            var cfg = KubernetesClientConfiguration.BuildConfigFromConfigFile(fi, context, useRelativePaths: false);
            Assert.Equal(host, cfg.Host);
        }

        /// <summary>
        ///     Check if namespace is properly loaded, per context
        /// </summary>
        [Theory]
        [InlineData("federal-context", "chisel-ns")]
        [InlineData("queen-anne-context", "saw-ns")]
        public void ContextNamespace(string context, string @namespace)
        {
            var fi = new FileInfo("assets/kubeconfig.yml");
            var cfg = KubernetesClientConfiguration.BuildConfigFromConfigFile(fi, context, useRelativePaths: false);
            Assert.Equal(@namespace, cfg.Namespace);
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
            var cfg = KubernetesClientConfiguration.BuildConfigFromConfigFile(fi, context, useRelativePaths: false);
            Assert.Equal(context, cfg.CurrentContext);
            Assert.Equal(cfg.ClientCertificateFilePath, clientCert);
            Assert.Equal(cfg.ClientKeyFilePath, clientCertKey);
        }

        /// <summary>
        ///     Checks for loading of elliptical curve keys
        /// </summary>
        /// <param name="context"></param>
        [Theory]
        [InlineData("elliptic-context")]
        public void ContextEllipticKey(string context)
        {
            var fi = new FileInfo("assets/kubeconfig.yml");
            var cfg = KubernetesClientConfiguration.BuildConfigFromConfigFile(fi, context, useRelativePaths: false);
            var pfx = CertUtils.GeneratePfx(cfg);
            Assert.NotNull(pfx);
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
            Assert.NotNull(cfg.SslCaCerts);
            Assert.Equal(File.ReadAllText("assets/client-certificate-data.txt"), cfg.ClientCertificateData);
            Assert.Equal(File.ReadAllText("assets/client-key-data.txt"), cfg.ClientCertificateKeyData);
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
            Assert.Null(cfg.SslCaCerts);
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

        [Fact]
        public void CreatedFromPreLoadedConfig()
        {
            var k8sConfig =
                KubernetesClientConfiguration.LoadKubeConfig(
                    new FileInfo("assets/kubeconfig.yml"),
                    false);
            var cfg = KubernetesClientConfiguration.BuildConfigFromConfigObject(k8sConfig);
            Assert.NotNull(cfg.Host);
        }

        /// <summary>
        ///     Checks Host is loaded from the default configuration file
        /// </summary>
        [Fact]
        public void DefaultConfigurationLoaded()
        {
            var cfg = KubernetesClientConfiguration.BuildConfigFromConfigFile(
                new FileInfo("assets/kubeconfig.yml"),
                useRelativePaths: false);
            Assert.NotNull(cfg.Host);
        }

        /// <summary>
        ///     Checks that a KubeConfigException is thrown when incomplete user credentials
        /// </summary>
        [Fact]
        public void IncompleteUserCredentials()
        {
            var fi = new FileInfo("assets/kubeconfig.no-credentials.yml");
            Assert.Throws<KubeConfigException>(() =>
                KubernetesClientConfiguration.BuildConfigFromConfigFile(fi, useRelativePaths: false));
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
        ///     Checks that a KubeConfigException is thrown when the current context exists but has no details specified
        /// </summary>
        [Fact]
        public void ContextNoDetails()
        {
            var fi = new FileInfo("assets/kubeconfig.no-context-details.yml");
            Assert.Throws<KubeConfigException>(() => KubernetesClientConfiguration.BuildConfigFromConfigFile(fi));
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
            var cfg = KubernetesClientConfiguration.BuildConfigFromConfigFile(fi, useRelativePaths: false);
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
            Assert.Throws<KubeConfigException>(() =>
                KubernetesClientConfiguration.BuildConfigFromConfigFile(fi, useRelativePaths: false));
        }

        /// <summary>
        ///     Make sure that user is not a necessary field. set #issue 24
        /// </summary>
        [Fact]
        public void EmptyUserNotFound()
        {
            var fi = new FileInfo("assets/kubeconfig.no-user.yml");
            var cfg = KubernetesClientConfiguration.BuildConfigFromConfigFile(fi, useRelativePaths: false);

            Assert.NotEmpty(cfg.Host);
        }

        /// <summary>
        ///     Make sure Host is replaced by masterUrl
        /// </summary>
        [Fact]
        public void OverrideByMasterUrl()
        {
            var fi = new FileInfo("assets/kubeconfig.yml");
            var cfg = KubernetesClientConfiguration.BuildConfigFromConfigFile(fi, masterUrl: "http://test.server",
                useRelativePaths: false);
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
                config = KubernetesClientConfiguration.BuildConfigFromConfigFile(tempFileInfo, useRelativePaths: false);
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
            var filePath = "assets/kubeconfig.yml";
            var cfg = KubernetesClientConfiguration.BuildConfigFromConfigFile(filePath, null, null,
                false);
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

        /// <summary>
        ///     Checks users.as-user-extra is loaded correctly from a configuration file.
        /// </summary>
        [Fact]
        public void AsUserExtra()
        {
            var filePath = "assets/kubeconfig.as-user-extra.yml";

            var cfg = KubernetesClientConfiguration.BuildConfigFromConfigFile(filePath, null, null,
                false);
            Assert.NotNull(cfg.Host);
        }

        [Fact]
        public async Task ContextWithClusterExtensions()
        {
            var path = Path.GetFullPath("assets/kubeconfig.cluster-extensions.yml");

            var cfg = await KubernetesClientConfiguration.BuildConfigFromConfigFileAsync(new FileInfo(path)).ConfigureAwait(false);
        }

        /// <summary>
        ///     Ensures Kube config file is loaded from explicit file
        /// </summary>
        [Fact]
        public void LoadKubeConfigExplicitFilePath()
        {
            var txt = File.ReadAllText("assets/kubeconfig.yml");
            var expectedCfg = Yaml.LoadFromString<K8SConfiguration>(txt);

            var cfg = KubernetesClientConfiguration.LoadKubeConfig("assets/kubeconfig.yml");

            Assert.NotNull(cfg);
            AssertConfigEqual(expectedCfg, cfg);
        }

        [Fact]
        public void LoadKubeConfigFileInfo()
        {
            var filePath = "assets/kubeconfig.yml";
            var txt = File.ReadAllText(filePath);
            var expectedCfg = Yaml.LoadFromString<K8SConfiguration>(txt);

            var fileInfo = new FileInfo(filePath);
            var cfg = KubernetesClientConfiguration.LoadKubeConfig(fileInfo);

            Assert.NotNull(cfg);
            AssertConfigEqual(expectedCfg, cfg);
        }

        [Fact]
        public void LoadKubeConfigStream()
        {
            var filePath = "assets/kubeconfig.yml";
            var txt = File.ReadAllText(filePath);
            var expectedCfg = Yaml.LoadFromString<K8SConfiguration>(txt);

            var fileInfo = new FileInfo(filePath);
            K8SConfiguration cfg;
            using (var stream = fileInfo.OpenRead())
            {
                cfg = KubernetesClientConfiguration.LoadKubeConfig(stream);
            }

            Assert.NotNull(cfg);
            AssertConfigEqual(expectedCfg, cfg);
        }

        [Fact]
        public void LoadKubeConfigFromEnvironmentVariable()
        {
            // BuildDefaultConfig assumes UseRelativePaths: true, which isn't
            // done by any tests.
            var filePath = Path.GetFullPath("assets/kubeconfig.relative.yml");
            var environmentVariable = "KUBECONFIG_LoadKubeConfigFromEnvironmentVariable";

            Environment.SetEnvironmentVariable(environmentVariable, filePath);
            KubernetesClientConfiguration.KubeConfigEnvironmentVariable = environmentVariable;

            var cfg = KubernetesClientConfiguration.BuildDefaultConfig();

            Assert.NotNull(cfg);
        }

        [Fact]
        public void LoadKubeConfigFromEnvironmentVariableMultipleConfigs()
        {
            // This test makes sure that a list of environment variables works (no exceptions),
            // doesn't check validity of configuration, which is done in other tests.

            var filePath = Path.GetFullPath("assets/kubeconfig.relative.yml");
            var environmentVariable = "KUBECONFIG_LoadKubeConfigFromEnvironmentVariable_MultipleConfigs";

            Environment.SetEnvironmentVariable(
                environmentVariable,
                string.Concat(filePath, RuntimeInformation.IsOSPlatform(OSPlatform.Windows) ? ';' : ':', filePath));
            KubernetesClientConfiguration.KubeConfigEnvironmentVariable = environmentVariable;

            var cfg = KubernetesClientConfiguration.BuildDefaultConfig();

            Assert.NotNull(cfg);
        }

        [Fact]
        public void LoadSameKubeConfigFromEnvironmentVariableUnmodified()
        {
            var txt = File.ReadAllText("assets/kubeconfig.yml");
            var expectedCfg = Yaml.LoadFromString<K8SConfiguration>(txt);

            var fileInfo = new FileInfo(Path.GetFullPath("assets/kubeconfig.yml"));

            var cfg = KubernetesClientConfiguration.LoadKubeConfig(new FileInfo[] { fileInfo, fileInfo });

            AssertConfigEqual(expectedCfg, cfg);
        }

        [Fact]
        public void MergeKubeConfigNoDuplicates()
        {
            var firstPath = Path.GetFullPath("assets/kubeconfig.as-user-extra.yml");
            var secondPath = Path.GetFullPath("assets/kubeconfig.yml");

            var cfg = KubernetesClientConfiguration.LoadKubeConfig(new FileInfo[]
            {
                new FileInfo(firstPath), new FileInfo(secondPath),
            });

            // Merged file has 6 users now.
            Assert.Equal(6, cfg.Users.Count());
            Assert.Equal(5, cfg.Clusters.Count());
            Assert.Equal(5, cfg.Contexts.Count());
        }

        [Fact]
        public void AlwaysPicksFirstOccurence()
        {
            var firstPath = Path.GetFullPath("assets/kubeconfig.no-cluster.yml");
            var secondPath = Path.GetFullPath("assets/kubeconfig.no-context.yml");

            var cfg = KubernetesClientConfiguration.LoadKubeConfig(new FileInfo[]
            {
                new FileInfo(firstPath), new FileInfo(secondPath),
            });

            var user = cfg.Users.Where(u => u.Name == "green-user").Single();
            Assert.NotNull(user.UserCredentials.Password);
            Assert.Null(user.UserCredentials.ClientCertificate);
        }

        [Fact]
        public void ContextFromSecondWorks()
        {
            var firstPath = Path.GetFullPath("assets/kubeconfig.no-current-context.yml");
            var secondPath = Path.GetFullPath("assets/kubeconfig.no-user.yml");

            var cfg = KubernetesClientConfiguration.LoadKubeConfig(new FileInfo[]
            {
                new FileInfo(firstPath), new FileInfo(secondPath),
            });

            // green-user
            Assert.NotNull(cfg.CurrentContext);
        }

        [Fact]
        public void ContextPreferencesExtensionsMergeWithDuplicates()
        {
            var path = Path.GetFullPath("assets/kubeconfig.preferences-extensions.yml");

            var cfg = KubernetesClientConfiguration.LoadKubeConfig(new FileInfo[]
            {
                new FileInfo(path), new FileInfo(path),
            });

            Assert.Single(cfg.Extensions);
            Assert.Single(cfg.Preferences);
        }

        /// <summary>
        ///     Ensures Kube config file can be loaded from within a non-default <see cref="SynchronizationContext"/>.
        ///     The use of <see cref="UIFactAttribute"/> ensures the test is run from within a UI-like <see cref="SynchronizationContext"/>.
        /// </summary>
        [UIFact]
        public void BuildConfigFromConfigFileInfoOnNonDefaultSynchronizationContext()
        {
            var fi = new FileInfo("assets/kubeconfig.yml");
            KubernetesClientConfiguration.BuildConfigFromConfigFile(fi, "federal-context", useRelativePaths: false);
        }

        private void AssertConfigEqual(K8SConfiguration expected, K8SConfiguration actual)
        {
            Assert.Equal(expected.ApiVersion, actual.ApiVersion);
            Assert.Equal(expected.CurrentContext, actual.CurrentContext);

            foreach (var expectedContext in expected.Contexts)
            {
                // Will throw exception if not found
                var actualContext = actual.Contexts.First(c => c.Name.Equals(expectedContext.Name));
                AssertContextEqual(expectedContext, actualContext);
            }

            foreach (var expectedCluster in expected.Clusters)
            {
                // Will throw exception if not found
                var actualCluster = actual.Clusters.First(c => c.Name.Equals(expectedCluster.Name));
                AssertClusterEqual(expectedCluster, actualCluster);
            }

            foreach (var expectedUser in expected.Users)
            {
                // Will throw exception if not found
                var actualUser = actual.Users.First(u => u.Name.Equals(expectedUser.Name));
                AssertUserEqual(expectedUser, actualUser);
            }
        }

        private static void AssertContextEqual(Context expected, Context actual)
        {
            Assert.Equal(expected.Name, actual.Name);
            Assert.Equal(expected.ContextDetails.Cluster, actual.ContextDetails.Cluster);
            Assert.Equal(expected.ContextDetails.User, actual.ContextDetails.User);
            Assert.Equal(expected.ContextDetails.Namespace, actual.ContextDetails.Namespace);
        }

        private static void AssertClusterEqual(Cluster expected, Cluster actual)
        {
            Assert.Equal(expected.Name, actual.Name);
            Assert.Equal(expected.ClusterEndpoint.CertificateAuthority, actual.ClusterEndpoint.CertificateAuthority);
            Assert.Equal(
                expected.ClusterEndpoint.CertificateAuthorityData,
                actual.ClusterEndpoint.CertificateAuthorityData);
            Assert.Equal(expected.ClusterEndpoint.Server, actual.ClusterEndpoint.Server);
            Assert.Equal(expected.ClusterEndpoint.SkipTlsVerify, actual.ClusterEndpoint.SkipTlsVerify);
        }

        private static void AssertUserEqual(User expected, User actual)
        {
            Assert.Equal(expected.Name, actual.Name);

            var expectedCreds = expected.UserCredentials;
            var actualCreds = actual.UserCredentials;

            Assert.Equal(expectedCreds.ClientCertificateData, actualCreds.ClientCertificateData);
            Assert.Equal(expectedCreds.ClientCertificate, actualCreds.ClientCertificate);
            Assert.Equal(expectedCreds.ClientKeyData, actualCreds.ClientKeyData);
            Assert.Equal(expectedCreds.ClientKey, actualCreds.ClientKey);
            Assert.Equal(expectedCreds.Token, actualCreds.Token);
            Assert.Equal(expectedCreds.Impersonate, actualCreds.Impersonate);
            Assert.Equal(expectedCreds.UserName, actualCreds.UserName);
            Assert.Equal(expectedCreds.Password, actualCreds.Password);

            Assert.True(expectedCreds.ImpersonateGroups.All(x => actualCreds.ImpersonateGroups.Contains(x)));
            Assert.True(expectedCreds.ImpersonateUserExtra.All(x => actualCreds.ImpersonateUserExtra.Contains(x)));

            if (expectedCreds.AuthProvider != null)
            {
                Assert.True(expectedCreds.AuthProvider.Config.All(x => actualCreds.AuthProvider.Config.Contains(x)));
            }
        }
    }
}
