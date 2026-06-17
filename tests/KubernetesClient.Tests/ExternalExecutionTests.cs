using k8s.KubeConfigModels;
using System.Collections.Generic;
using System.Text.Json.Nodes;
using Xunit;

namespace k8s.Tests
{
    public class ExternalExecutionTests
    {
        [Fact]
        public void CreateRunnableExternalProcess()
        {
            var actual = KubernetesClientConfiguration.CreateRunnableExternalProcess(new ExternalExecution
            {
                ApiVersion = "testingversion",
                Command = "command",
                Arguments = new List<string> { "arg1", "arg2" },
                EnvironmentVariables = new List<Dictionary<string, string>>
                    { new Dictionary<string, string> { { "name", "testkey" }, { "value", "testvalue" } } },
            });

            var json = JsonNode.Parse(actual.StartInfo.EnvironmentVariables["KUBERNETES_EXEC_INFO"]);
            Assert.Equal("testingversion", json["apiVersion"]?.GetValue<string>());
            Assert.Equal("ExecCredentials", json["kind"]?.GetValue<string>());
            Assert.False(json["spec"].AsObject().ContainsKey("cluster"));

            Assert.Equal("command", actual.StartInfo.FileName);
            Assert.Equal("arg1 arg2", actual.StartInfo.Arguments);
            Assert.Equal("testvalue", actual.StartInfo.EnvironmentVariables["testkey"]);
        }

        [Fact]
        public void ToExecClusterInfoMapsFieldsCorrectly()
        {
            var cluster = new ClusterEndpoint
            {
                Server = "https://my-cluster.example.com:6443",
                CertificateAuthorityData = "LS0tLS1CRUdJTi0t",
                SkipTlsVerify = false,
                TlsServerName = "my-cluster.example.com",
                Extensions = new List<NamedExtension>
                {
                    new NamedExtension
                    {
                        Name = "client.authentication.k8s.io/exec",
                        Extension = new Dictionary<object, object> { { "audience", "06e3fbd18de8" } },
                    },
                },
            };

            var result = KubernetesClientConfiguration.ToExecClusterInfo(cluster);

            Assert.NotNull(result);
            Assert.Equal("https://my-cluster.example.com:6443", result["server"]?.GetValue<string>());
            Assert.False(result.AsObject().ContainsKey("insecure-skip-tls-verify"));
            Assert.Equal("LS0tLS1CRUdJTi0t", result["certificate-authority-data"]?.GetValue<string>());
            Assert.Equal("my-cluster.example.com", result["tls-server-name"]?.GetValue<string>());
            Assert.Equal("06e3fbd18de8", result["config"]?["audience"]?.GetValue<string>());
        }

        [Fact]
        public void ToExecClusterInfoReturnsNullForNullCluster()
        {
            Assert.Null(KubernetesClientConfiguration.ToExecClusterInfo(null));
        }

        [Fact]
        public void ToExecClusterInfoOmitsOptionalEmptyFields()
        {
            var cluster = new ClusterEndpoint
            {
                Server = "https://my-cluster.example.com",
                SkipTlsVerify = false,
            };

            var result = KubernetesClientConfiguration.ToExecClusterInfo(cluster);

            Assert.NotNull(result);
            Assert.Equal("https://my-cluster.example.com", result["server"]?.GetValue<string>());
            Assert.False(result.AsObject().ContainsKey("insecure-skip-tls-verify"));
            Assert.False(result.AsObject().ContainsKey("certificate-authority-data"));
            Assert.False(result.AsObject().ContainsKey("tls-server-name"));
            Assert.False(result.AsObject().ContainsKey("config"));
        }

        [Fact]
        public void CreateRunnableExternalProcessIncludesClusterWhenProvideClusterInfoIsTrue()
        {
            var cluster = new ClusterEndpoint
            {
                Server = "https://my-cluster.example.com",
                SkipTlsVerify = false,
            };

            var actual = KubernetesClientConfiguration.CreateRunnableExternalProcess(
                new ExternalExecution
                {
                    ApiVersion = "client.authentication.k8s.io/v1",
                    Command = "my-credential-plugin",
                    ProvideClusterInfo = true,
                },
                cluster: cluster);

            var json = JsonNode.Parse(actual.StartInfo.EnvironmentVariables["KUBERNETES_EXEC_INFO"]);
            Assert.Equal("https://my-cluster.example.com", json["spec"]?["cluster"]?["server"]?.GetValue<string>());
        }

        [Fact]
        public void CreateRunnableExternalProcessOmitsClusterWhenProvideClusterInfoIsFalse()
        {
            var cluster = new ClusterEndpoint
            {
                Server = "https://my-cluster.example.com",
                SkipTlsVerify = false,
            };

            var actual = KubernetesClientConfiguration.CreateRunnableExternalProcess(
                new ExternalExecution
                {
                    ApiVersion = "client.authentication.k8s.io/v1",
                    Command = "my-credential-plugin",
                    ProvideClusterInfo = false,
                },
                cluster: cluster);

            var json = JsonNode.Parse(actual.StartInfo.EnvironmentVariables["KUBERNETES_EXEC_INFO"]);
            Assert.False(json["spec"].AsObject().ContainsKey("cluster"));
        }

        [Fact]
        public void ToExecClusterInfoHandlesNestedExtensions()
        {
            var cluster = new ClusterEndpoint
            {
                Server = "https://my-cluster.example.com",
                Extensions = new List<NamedExtension>
                {
                    new NamedExtension
                    {
                        Name = "client.authentication.k8s.io/exec",
                        Extension = BuildNestedExtension(),
                    },
                },
            };

            var result = KubernetesClientConfiguration.ToExecClusterInfo(cluster);

            Assert.NotNull(result);
            Assert.Equal("06e3fbd18de8", result["config"]?["audience"]?.GetValue<string>());
            Assert.Equal("value1", result["config"]?["nested"]?["key1"]?.GetValue<string>());
            Assert.Equal("innervalue", result["config"]?["nested"]?["deep"]?["inner"]?.GetValue<string>());
            Assert.Equal("a", result["config"]?["tags"]?[0]?.GetValue<string>());
            Assert.Equal("b", result["config"]?["tags"]?[1]?.GetValue<string>());
            Assert.Equal("c", result["config"]?["tags"]?[2]?.GetValue<string>());
        }

        [Fact]
        public void ToExecClusterInfoEmitsInsecureSkipTlsVerifyWhenTrue()
        {
            var cluster = new ClusterEndpoint
            {
                Server = "https://my-cluster.example.com",
                SkipTlsVerify = true,
            };

            var result = KubernetesClientConfiguration.ToExecClusterInfo(cluster);

            Assert.NotNull(result);
            Assert.True(result["insecure-skip-tls-verify"]?.GetValue<bool>());
        }

        private static Dictionary<object, object> BuildNestedExtension()
        {
            var deep = new Dictionary<object, object>
            {
                { "inner", "innervalue" },
            };
            var nested = new Dictionary<object, object>
            {
                { "key1", "value1" },
                { "deep", deep },
            };
            return new Dictionary<object, object>
            {
                { "audience", "06e3fbd18de8" },
                { "nested", nested },
                { "tags", new List<object> { "a", "b", "c" } },
            };
        }
    }
}
