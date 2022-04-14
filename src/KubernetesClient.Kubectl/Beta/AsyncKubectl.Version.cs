using System.Reflection;
using k8s.Models;

namespace k8s.kubectl.beta;

public partial class AsyncKubectl
{
    private static readonly Version AsssemblyVersion = Assembly.GetExecutingAssembly().GetName().Version ?? default!;

    public record KubeVersion
    {
        public Version ClientVersion { get; init; } = AsssemblyVersion;

        public VersionInfo ServerVersion { get; init; } = default!;
    }

    public async Task<KubeVersion> Version(CancellationToken cancellationToken = default)
    {
        var serverVersion = await client.GetCodeAsync(cancellationToken).ConfigureAwait(false);
        return new KubeVersion { ServerVersion = serverVersion };
    }
}
