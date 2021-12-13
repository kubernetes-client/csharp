using System.Collections.Generic;
using System.Text.Json;
using k8s.KubeConfigModels;
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

            var actualExecInfo = JsonSerializer.Deserialize<IDictionary<string, dynamic>>(actual.StartInfo.EnvironmentVariables["KUBERNETES_EXEC_INFO"]);
            Assert.Equal("testingversion", actualExecInfo["apiVersion"].ToString());
            Assert.Equal("ExecCredentials", actualExecInfo["kind"].ToString());

            Assert.Equal("command", actual.StartInfo.FileName);
            Assert.Equal("arg1 arg2", actual.StartInfo.Arguments);
            Assert.Equal("testvalue", actual.StartInfo.EnvironmentVariables["testkey"]);
        }
    }
}
