namespace k8s.Informers
{
    /// <summary>
    ///     An informer that serves kubernetes resources
    /// </summary>
    /// <typeparam name="TResource">The type of Kubernetes resource</typeparam>
    public interface IKubernetesInformer<TResource> : IInformer<TResource, KubernetesInformerOptions>, IInformer<TResource> where TResource : IKubernetesObject
    {
    }
}
