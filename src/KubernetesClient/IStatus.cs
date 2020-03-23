namespace k8s
{
    /// <summary>
    /// Kubernetes object that exposes status
    /// </summary>
    /// <typeparam name="T">The type of status object</typeparam>
    public interface IStatus<T>
    {
        /// <summary>
        /// Gets or sets most recently observed status of the object. This data
        /// may not be up to date. Populated by the system. Read-only. More
        /// info:
        /// https://git.k8s.io/community/contributors/devel/sig-architecture/api-conventions.md#spec-and-status
        /// </summary>
        T Status { get; set; }
    }
}