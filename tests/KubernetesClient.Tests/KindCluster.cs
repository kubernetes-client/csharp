using System;
using System.Diagnostics;
using FluentAssertions;

namespace k8s.Tests
{
    public class KindCluster : IDisposable
    {
        public KindCluster()
        {
            var (exitCode, stdout, stderr) = RunProcess("kind", "create cluster");

            if (exitCode != 0 && !stderr.Contains("node(s) already exist"))
            {
                exitCode.Should().Be(0, stderr);
            }
        }

        public void Dispose()
        {
            //RunProcess("kind", "delete cluster");
        }

        private (int exitCode, string stdout, string stderr) RunProcess(string filename, string args = null)
        {
            var process = new Process
            {
                StartInfo =
                {
                    FileName = filename,
                    Arguments = args,
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true
                }
            };
            process.Start();
            //* Read the output (or the error)
            var output = process.StandardOutput.ReadToEnd();
            var err = process.StandardError.ReadToEnd();
            process.WaitForExit();
            return (process.ExitCode, output, err);
        }
    }
}
