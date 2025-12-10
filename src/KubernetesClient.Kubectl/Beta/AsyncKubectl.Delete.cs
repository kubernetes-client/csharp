namespace k8s.kubectl.beta;

public partial class AsyncKubectl
{
    /// <summary>
    /// Delete a Kubernetes resource by name.
    /// </summary>
    /// <typeparam name="T">The type of Kubernetes resource to delete.</typeparam>
    /// <param name="name">The name of the resource.</param>
    /// <param name="namespace">The namespace of the resource (for namespaced resources). Optional.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The deleted resource.</returns>
    public async Task<T> DeleteAsync<T>(string name, string? @namespace = null, CancellationToken cancellationToken = default)
        where T : IKubernetesObject
    {
        var metadata = typeof(T).GetKubernetesTypeMetadata();
        using var genericClient = new GenericClient(client, metadata.Group, metadata.ApiVersion, metadata.PluralName, disposeClient: false);

        return @namespace != null
            ? await genericClient.DeleteNamespacedAsync<T>(@namespace, name, cancellationToken).ConfigureAwait(false)
            : await genericClient.DeleteAsync<T>(name, cancellationToken).ConfigureAwait(false);
    }
}
