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
}
