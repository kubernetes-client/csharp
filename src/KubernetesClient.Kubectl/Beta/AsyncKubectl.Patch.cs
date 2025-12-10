using k8s.Models;

namespace k8s.kubectl.beta;

public partial class AsyncKubectl
{
    /// <summary>
    /// Patch a cluster-scoped Kubernetes resource.
    /// </summary>
    /// <typeparam name="T">The type of Kubernetes resource to patch.</typeparam>
    /// <param name="patch">The patch to apply.</param>
    /// <param name="name">The name of the resource.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The patched resource.</returns>
    public async Task<T> PatchAsync<T>(V1Patch patch, string name, CancellationToken cancellationToken = default)
        where T : IKubernetesObject
    {
        var metadata = typeof(T).GetKubernetesTypeMetadata();
        using var genericClient = new GenericClient(client, metadata.Group, metadata.ApiVersion, metadata.PluralName, disposeClient: false);

        return await genericClient.PatchAsync<T>(patch, name, cancellationToken).ConfigureAwait(false);
    }

    /// <summary>
    /// Patch a namespaced Kubernetes resource.
    /// </summary>
    /// <typeparam name="T">The type of Kubernetes resource to patch.</typeparam>
    /// <param name="patch">The patch to apply.</param>
    /// <param name="name">The name of the resource.</param>
    /// <param name="namespace">The namespace of the resource.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The patched resource.</returns>
    public async Task<T> PatchNamespacedAsync<T>(V1Patch patch, string name, string @namespace, CancellationToken cancellationToken = default)
        where T : IKubernetesObject
    {
        var metadata = typeof(T).GetKubernetesTypeMetadata();
        using var genericClient = new GenericClient(client, metadata.Group, metadata.ApiVersion, metadata.PluralName, disposeClient: false);

        return await genericClient.PatchNamespacedAsync<T>(patch, @namespace, name, cancellationToken).ConfigureAwait(false);
    }
}
