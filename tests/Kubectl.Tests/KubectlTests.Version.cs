using k8s.E2E;
using System.Text.Json;
using Xunit;

namespace k8s.kubectl.Tests;

public partial class KubectlTests
{
    [MinikubeFact]
    public void Version()
    {
        var client = CreateClient();
        var version = client.Version();
        var serverobj = version.ServerVersion;

        var output = RunKubectl("version");

        var serverstr = output.Split('\n').Skip(1).First().Trim();

        Assert.Equal(serverstr, $"Server Version: version.Info{{Major:\"{serverobj.Major}\", Minor:\"{serverobj.Minor}\", GitVersion:\"{serverobj.GitVersion}\", GitCommit:\"{serverobj.GitCommit}\", GitTreeState:\"{serverobj.GitTreeState}\", BuildDate:\"{serverobj.BuildDate}\", GoVersion:\"{serverobj.GoVersion}\", Compiler:\"{serverobj.Compiler}\", Platform:\"{serverobj.Platform}\"}}");

        dynamic? swagger = JsonSerializer.Deserialize(File.OpenRead("swagger.json"), new
        {
            info = new
            {
                version = "",
            },
        }.GetType());

        Assert.Equal(swagger?.info.version, version.ClientSwaggerVersion);
    }
}
