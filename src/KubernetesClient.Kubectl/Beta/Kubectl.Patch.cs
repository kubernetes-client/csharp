using k8s.Models;

namespace k8s.kubectl.beta;

public partial class Kubectl
{
    /// <summary>
    /// Patch a cluster-scoped Kubernetes resource.
    /// </summary>
    /// <typeparam name="T">The type of Kubernetes resource to patch.</typeparam>
    /// <param name="patch">The patch to apply.</param>
    /// <param name="name">The name of the resource.</param>
    /// <returns>The patched resource.</returns>
    public T Patch<T>(V1Patch patch, string name)
        where T : IKubernetesObject
    {
        return client.PatchAsync<T>(patch, name).GetAwaiter().GetResult();
    }

    /// <summary>
    /// Patch a namespaced Kubernetes resource.
    /// </summary>
    /// <typeparam name="T">The type of Kubernetes resource to patch.</typeparam>
    /// <param name="patch">The patch to apply.</param>
    /// <param name="name">The name of the resource.</param>
    /// <param name="namespace">The namespace of the resource.</param>
    /// <returns>The patched resource.</returns>
    public T PatchNamespaced<T>(V1Patch patch, string name, string @namespace)
        where T : IKubernetesObject
    {
        return client.PatchNamespacedAsync<T>(patch, name, @namespace).GetAwaiter().GetResult();
    }
}
