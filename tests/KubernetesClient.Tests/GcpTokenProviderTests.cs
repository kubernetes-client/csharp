using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using k8s.Authentication;

namespace k8s.Tests
{
    public class GcpTokenProviderTests
    {
        [OperatingSystemDependentFact(Exclude = OperatingSystem.OSX)]
        public async Task GetToken()
        {
            var isWindows = Environment.OSVersion.Platform == PlatformID.Win32NT;
            var cmd = Path.Combine(Directory.GetCurrentDirectory(), "assets", isWindows ? "mock-gcloud.cmd" : "mock-gcloud.sh");
            if (!isWindows)
            {
                System.Diagnostics.Process.Start("chmod", $"+x {cmd}").WaitForExit();
            }

            var sut = new GcpTokenProvider(cmd);
            var result = await sut.GetAuthenticationHeaderAsync(CancellationToken.None).ConfigureAwait(false);
            result.Scheme.Should().Be("Bearer");
            result.Parameter.Should().Be("ACCESS-TOKEN");
        }
    }
}
