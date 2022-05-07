using k8s.Models;

namespace k8s.kubectl.beta;

public partial class AsyncKubectl
{
    private const string AsssemblyVersion = ThisAssembly.AssemblyInformationalVersion;

    public record KubernetesSDKVersion
    {
        public string ClientVersion { get; init; } = AsssemblyVersion;

        public string ClientSwaggerVersion { get; init; } = GeneratedApiVersion.SwaggerVersion;

        public VersionInfo ServerVersion { get; init; } = default!;
    }

    public async Task<KubernetesSDKVersion> Version(CancellationToken cancellationToken = default)
    {
        var serverVersion = await client.Version.GetCodeAsync(cancellationToken).ConfigureAwait(false);
        return new KubernetesSDKVersion { ServerVersion = serverVersion };
    }
}
