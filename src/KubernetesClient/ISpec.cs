namespace k8s
{
    /// <summary>
    /// Represents a Kubernetes object that has a spec
    /// </summary>
    /// <typeparam name="T">type of Kubernetes object</typeparam>
    public interface ISpec<T>
    {
        /// <summary>
        /// Gets or sets specification of the desired behavior of the entity. More
        /// info:
        /// https://git.k8s.io/community/contributors/devel/sig-architecture/api-conventions.md#spec-and-status
        /// </summary>
        T Spec { get; set; }
    }
}
