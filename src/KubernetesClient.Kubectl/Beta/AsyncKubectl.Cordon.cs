using Json.Patch;
using k8s.Models;
using System.Text.Json;

namespace k8s.kubectl.beta;

public partial class AsyncKubectl
{
    public async Task Cordon(string nodeName, CancellationToken cancellationToken = default)
    {
        await PatchNodeUnschedulable(nodeName, true, cancellationToken).ConfigureAwait(false);
    }

    public async Task Uncordon(string nodeName, CancellationToken cancellationToken = default)
    {
        await PatchNodeUnschedulable(nodeName, false, cancellationToken).ConfigureAwait(false);
    }

    private async Task PatchNodeUnschedulable(string nodeName, bool desired, CancellationToken cancellationToken = default)
    {
        var node = await client.CoreV1.ReadNodeAsync(nodeName, cancellationToken: cancellationToken).ConfigureAwait(false);

        var old = JsonSerializer.SerializeToDocument(node);
        node.Spec.Unschedulable = desired;

        var patch = old.CreatePatch(node);

        await client.CoreV1.PatchNodeAsync(new V1Patch(patch, V1Patch.PatchType.JsonPatch), nodeName, cancellationToken: cancellationToken).ConfigureAwait(false);
    }
}
