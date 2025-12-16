namespace k8s.kubectl.beta;

public partial class Kubectl
{
    /// <summary>
    /// Delete a Kubernetes resource by name.
    /// </summary>
    /// <typeparam name="T">The type of Kubernetes resource to delete.</typeparam>
    /// <param name="name">The name of the resource.</param>
    /// <param name="namespace">The namespace of the resource (for namespaced resources). Optional.</param>
    /// <returns>The deleted resource.</returns>
    public T Delete<T>(string name, string? @namespace = null)
        where T : IKubernetesObject
    {
        return client.DeleteAsync<T>(name, @namespace).GetAwaiter().GetResult();
    }
}
