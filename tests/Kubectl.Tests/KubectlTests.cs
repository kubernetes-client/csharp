using k8s.E2E;
using k8s.kubectl.beta;
using System.Diagnostics;

namespace k8s.kubectl.Tests;

public partial class KubectlTests
{
    private Kubectl CreateClient()
    {
        return new Kubectl(MinikubeTests.CreateClient());
    }

    private string RunKubectl(string args)
    {
        var p = new Process
        {
            StartInfo = new ProcessStartInfo
            {
                FileName = "kubectl",
                Arguments = args,
                UseShellExecute = false,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                CreateNoWindow = true,
            },
        };

        p.Start();

        try
        {
            if (!p.WaitForExit((int)TimeSpan.FromSeconds(30).TotalMilliseconds))
            {
                throw new Exception("kubectl timed out");
            }

            if (p.ExitCode != 0)
            {
                throw new Exception(p.StandardError.ReadToEnd());
            }

            return p.StandardOutput.ReadToEnd();
        }
        finally
        {
            p.Kill(true);
        }
    }
}
