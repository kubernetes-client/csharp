namespace k8s.kubectl.beta;

public partial class Kubectl
{
    /// <summary>
    /// Get a Kubernetes resource by name.
    /// </summary>
    /// <typeparam name="T">The type of Kubernetes resource to get.</typeparam>
    /// <param name="name">The name of the resource.</param>
    /// <param name="namespace">The namespace of the resource (for namespaced resources). Optional.</param>
    /// <returns>The requested resource.</returns>
    public T Get<T>(string name, string? @namespace = null)
        where T : IKubernetesObject
    {
        return client.GetAsync<T>(name, @namespace).GetAwaiter().GetResult();
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
    /// <returns>The list of requested resources.</returns>
    public T List<T>(
        string? @namespace = null,
        string? labelSelector = null,
        string? fieldSelector = null,
        int? limit = null,
        string? continueToken = null)
        where T : IKubernetesObject
    {
        return client.ListAsync<T>(@namespace, labelSelector, fieldSelector, limit, continueToken).GetAwaiter().GetResult();
    }
}
