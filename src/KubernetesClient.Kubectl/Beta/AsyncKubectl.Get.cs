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

    /// <summary>
    /// List Kubernetes resources of a specific type.
    /// </summary>
    /// <typeparam name="T">The type of Kubernetes resource list to get (e.g., V1PodList).</typeparam>
    /// <param name="namespace">The namespace to list resources from. If null, lists cluster-scoped resources or resources across all namespaces for namespaced resources.</param>
    /// <param name="labelSelector">A selector to restrict the list of returned objects by their labels. Defaults to everything.</param>
    /// <param name="fieldSelector">A selector to restrict the list of returned objects by their fields. Defaults to everything.</param>
    /// <param name="limit">Maximum number of responses to return for a list call.</param>
    /// <param name="continueToken">The continue option should be set when retrieving more results from the server.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The list of requested resources.</returns>
    public async Task<T> ListAsync<T>(
        string? @namespace = null,
        string? labelSelector = null,
        string? fieldSelector = null,
        int? limit = null,
        string? continueToken = null,
        CancellationToken cancellationToken = default)
        where T : IKubernetesObject
    {
        var metadata = typeof(T).GetKubernetesTypeMetadata();
        using var genericClient = new GenericClient(client, metadata.Group, metadata.ApiVersion, metadata.PluralName, disposeClient: false);

        return @namespace != null
            ? await genericClient.ListNamespacedAsync<T>(@namespace, labelSelector, fieldSelector, limit, continueToken, cancellationToken).ConfigureAwait(false)
            : await genericClient.ListAsync<T>(labelSelector, fieldSelector, limit, continueToken, cancellationToken).ConfigureAwait(false);
    }
}
