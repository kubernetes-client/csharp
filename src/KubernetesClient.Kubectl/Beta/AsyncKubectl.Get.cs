namespace k8s.kubectl.beta;

public partial class AsyncKubectl
{
    /// <summary>
    /// Get a Kubernetes resource by name.
    /// </summary>
    /// <typeparam name="T">The type of Kubernetes resource to get.</typeparam>
    /// <param name="name">The name of the resource.</param>
    /// <param name="namespace">The namespace of the resource (for namespaced resources). Optional.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The requested resource.</returns>
    public async Task<T> GetAsync<T>(string name, string? @namespace = null, CancellationToken cancellationToken = default)
        where T : IKubernetesObject
    {
        var metadata = typeof(T).GetKubernetesTypeMetadata();
        using var genericClient = new GenericClient(client, metadata.Group, metadata.ApiVersion, metadata.PluralName, disposeClient: false);

        return @namespace != null
            ? await genericClient.ReadNamespacedAsync<T>(@namespace, name, cancellationToken).ConfigureAwait(false)
            : await genericClient.ReadAsync<T>(name, cancellationToken).ConfigureAwait(false);
    }
}
