using k8s.Authentication;
using k8s.Exceptions;
using k8s.KubeConfigModels;
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace k8s.Tests
{
    public class ExecCredentialPluginPolicyTests
    {
        [Fact]
        public void AllowlistMatchesBasenamesAndExactPaths()
        {
            var path = Path.Combine(Path.GetTempPath(), "trusted-plugin");
            var otherPath = Path.Combine(Path.GetTempPath(), "other", "trusted-plugin");
            var basenamePolicy = ExecCredentialPluginPolicy.Allowlist("trusted-plugin");
            Assert.True(basenamePolicy.Allows("trusted-plugin"));
            Assert.True(basenamePolicy.Allows(path));
            Assert.True(basenamePolicy.Allows(otherPath));
            Assert.False(basenamePolicy.Allows("untrusted-plugin"));
            Assert.False(basenamePolicy.Allows("trusted-plugin-other"));

            var pathPolicy = ExecCredentialPluginPolicy.Allowlist(path);
            Assert.True(pathPolicy.Allows(path));
            Assert.False(pathPolicy.Allows("trusted-plugin"));
            Assert.False(pathPolicy.Allows(otherPath));
            Assert.False(pathPolicy.Allows(Path.Combine(Path.GetDirectoryName(path), ".", "trusted-plugin")));
            Assert.Equal(RuntimeInformation.IsOSPlatform(OSPlatform.Windows), pathPolicy.Allows(path.ToUpperInvariant()));
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData(" ")]
        [InlineData(".")]
        [InlineData("..")]
        [InlineData("plugins/trusted-plugin")]
        [InlineData("*")]
        [InlineData("trusted?plugin")]
        [InlineData("trusted\0plugin")]
        public void InvalidAllowlistEntriesAreRejected(string entry)
        {
            Assert.Throws<ArgumentException>(() => ExecCredentialPluginPolicy.Allowlist("trusted-plugin", entry));
        }

        [Fact]
        public void EmptyAndNullAllowlistsFailClosed()
        {
            Assert.False(ExecCredentialPluginPolicy.Allowlist().Allows("trusted-plugin"));
            Assert.Throws<ArgumentNullException>(() => ExecCredentialPluginPolicy.Allowlist(null));
        }

        [Fact]
        public void AllowlistCopiesApplicationInput()
        {
            var entries = new[] { "trusted-plugin" };
            var policy = ExecCredentialPluginPolicy.Allowlist(entries);
            entries[0] = "untrusted-plugin";
            Assert.True(policy.Allows("trusted-plugin"));
            Assert.False(policy.Allows("untrusted-plugin"));
        }

        [Theory]
        [InlineData("deny")]
        [InlineData("empty")]
        [InlineData("unmatched")]
        public async Task DeniedPluginNeverStarts(string mode)
        {
            using var plugin = new TestPlugin();
            var policy = mode == "deny" ? ExecCredentialPluginPolicy.DenyAll :
                mode == "empty" ? ExecCredentialPluginPolicy.Allowlist() :
                ExecCredentialPluginPolicy.Allowlist("another-plugin");

            var exception = Assert.Throws<ExecCredentialPluginDeniedException>(() =>
                KubernetesClientConfiguration.ExecuteExternalCommand(plugin.Exec, policy));
            Assert.Equal(plugin.Exec.Command, exception.Command);
            Assert.Contains("denied by the application policy", exception.Message);
            Assert.False(File.Exists(plugin.Marker));

            var provider = new ExecTokenProvider(plugin.Exec, policy);
            await Assert.ThrowsAsync<ExecCredentialPluginDeniedException>(() =>
                provider.GetAuthenticationHeaderAsync(CancellationToken.None));
            Assert.False(File.Exists(plugin.Marker));
        }

        [Theory]
        [InlineData("default")]
        [InlineData("all")]
        [InlineData("basename")]
        [InlineData("path")]
        public async Task AllowedPluginRunsAndRefreshes(string mode)
        {
            using var plugin = new TestPlugin();
            var policy = mode == "default" ? null :
                mode == "all" ? ExecCredentialPluginPolicy.AllowAll :
                mode == "basename" ? ExecCredentialPluginPolicy.Allowlist(Path.GetFileName(plugin.Exec.Command)) :
                ExecCredentialPluginPolicy.Allowlist(plugin.Exec.Command);
            var kubeconfig = plugin.LoadConfig();
            var config = mode == "default" ? KubernetesClientConfiguration.BuildConfigFromConfigObject(kubeconfig) :
                KubernetesClientConfiguration.BuildConfigFromConfigObject(kubeconfig, execCredentialPluginPolicy: policy);
            Assert.Equal("test-token", config.AccessToken);
            Assert.True(File.Exists(plugin.Marker));
            File.Delete(plugin.Marker);
            var header = await config.TokenProvider.GetAuthenticationHeaderAsync(CancellationToken.None);
            Assert.Equal("test-token", header.Parameter);
            Assert.True(File.Exists(plugin.Marker));
        }

        [Fact]
        public async Task RefreshRetainsPolicyAndChecksTheCurrentCommand()
        {
            using var plugin = new TestPlugin();
            var kubeconfig = plugin.LoadConfig();
            var config = KubernetesClientConfiguration.BuildConfigFromConfigObject(
                kubeconfig,
                execCredentialPluginPolicy: ExecCredentialPluginPolicy.Allowlist(plugin.Exec.Command));
            File.Delete(plugin.Marker);
            foreach (var user in kubeconfig.Users)
            {
                user.UserCredentials.ExternalExecution.Command = "unapproved-plugin";
            }

            await Assert.ThrowsAsync<ExecCredentialPluginDeniedException>(() =>
                config.TokenProvider.GetAuthenticationHeaderAsync(CancellationToken.None));
            Assert.False(File.Exists(plugin.Marker));
        }

        [Theory]
        [InlineData("path")]
        [InlineData("file")]
        [InlineData("async-file")]
        [InlineData("stream")]
        [InlineData("async-stream")]
        [InlineData("object")]
        public async Task ConfigBuildersEnforcePolicy(string builder)
        {
            using var plugin = new TestPlugin();
            using var stream = File.OpenRead(plugin.KubeconfigPath);
            await Assert.ThrowsAsync<ExecCredentialPluginDeniedException>(async () =>
            {
                switch (builder)
                {
                    case "path":
                        KubernetesClientConfiguration.BuildConfigFromConfigFile(
                            plugin.KubeconfigPath,
                            execCredentialPluginPolicy: ExecCredentialPluginPolicy.DenyAll);
                        break;
                    case "file":
                        KubernetesClientConfiguration.BuildConfigFromConfigFile(
                            new FileInfo(plugin.KubeconfigPath),
                            execCredentialPluginPolicy: ExecCredentialPluginPolicy.DenyAll);
                        break;
                    case "async-file":
                        await KubernetesClientConfiguration.BuildConfigFromConfigFileAsync(
                            new FileInfo(plugin.KubeconfigPath),
                            execCredentialPluginPolicy: ExecCredentialPluginPolicy.DenyAll).ConfigureAwait(false);
                        break;
                    case "stream":
                        KubernetesClientConfiguration.BuildConfigFromConfigFile(
                            stream,
                            execCredentialPluginPolicy: ExecCredentialPluginPolicy.DenyAll);
                        break;
                    case "async-stream":
                        await KubernetesClientConfiguration.BuildConfigFromConfigFileAsync(
                            stream,
                            execCredentialPluginPolicy: ExecCredentialPluginPolicy.DenyAll).ConfigureAwait(false);
                        break;
                    case "object":
                        KubernetesClientConfiguration.BuildConfigFromConfigObject(
                            plugin.LoadConfig(),
                            execCredentialPluginPolicy: ExecCredentialPluginPolicy.DenyAll);
                        break;
                }
            });
            Assert.False(File.Exists(plugin.Marker));
        }

        [Fact]
        public void DenyAllDoesNotAffectNonExecCredentials()
        {
            var config = KubernetesClientConfiguration.BuildConfigFromConfigFile(
                new FileInfo("assets/kubeconfig.yml"), "queen-anne-context",
                execCredentialPluginPolicy: ExecCredentialPluginPolicy.DenyAll);
            Assert.Equal("black-token", config.AccessToken);
        }

        private sealed class TestPlugin : IDisposable
        {
            private readonly string directory = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());

            public TestPlugin()
            {
                Directory.CreateDirectory(directory);
                Marker = Path.Combine(directory, "started");
                var windows = RuntimeInformation.IsOSPlatform(OSPlatform.Windows);
                var script = Path.Combine(directory, windows ? "plugin.cmd" : "plugin.sh");
                var response = "{\"apiVersion\":\"client.authentication.k8s.io/v1beta1\",\"kind\":\"ExecCredential\",\"status\":{\"token\":\"test-token\"}}";
                var scriptContent = windows ?
                    $"@echo off\r\necho started > \"{Marker}\"\r\necho {response}\r\n" :
                    $"printf started > '{Marker}'\nprintf '%s\\n' '{response}'\n";
                File.WriteAllText(script, scriptContent);
                Exec = new ExternalExecution
                {
                    ApiVersion = "client.authentication.k8s.io/v1beta1",
                    Command = windows ? Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.System), "cmd.exe") : "/bin/sh",
                    Arguments = windows ? new List<string> { "/c", $"\"{script}\"" } : new List<string> { $"\"{script}\"" },
                };
                KubeconfigPath = Path.Combine(directory, "config");
                File.WriteAllText(KubeconfigPath, KubernetesYaml.Serialize(new K8SConfiguration
                {
                    CurrentContext = "test",
                    Contexts = new[] { new Context { Name = "test", ContextDetails = new ContextDetails { Cluster = "test", User = "test" } } },
                    Clusters = new[] { new Cluster { Name = "test", ClusterEndpoint = new ClusterEndpoint { Server = "https://localhost", SkipTlsVerify = true } } },
                    Users = new[] { new User { Name = "test", UserCredentials = new UserCredentials { ExternalExecution = Exec } } },
                    Preferences = new Dictionary<string, object> { { "execCredentialPluginPolicy", "AllowAll" } },
                }));
            }

            public ExternalExecution Exec { get; }

            public string Marker { get; }

            public string KubeconfigPath { get; }

            public K8SConfiguration LoadConfig() => KubernetesClientConfiguration.LoadKubeConfig(KubeconfigPath);

            public void Dispose() => Directory.Delete(directory, true);
        }
    }
}
