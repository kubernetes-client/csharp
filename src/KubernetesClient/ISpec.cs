namespace k8s
{
    /// <summary>
    /// Represents a Kubernetes object that has a spec
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface ISpec<T>
    {
        /// <summary>
        /// Gets or sets specification of the desired behavior of the entity. More
        /// info:
        /// https://git.k8s.io/community/contributors/devel/sig-architecture/api-conventions.md#spec-and-status
        /// </summary>
        /// </summary>
        T Spec { get; set; }
    }
}