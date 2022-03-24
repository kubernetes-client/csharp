using k8s.Models;

namespace k8s
{
    /// <summary>
    /// Kubernetes object that exposes metadata
    /// </summary>
    /// <typeparam name="T">Type of metadata exposed. Usually this will be either
    /// <see cref="V1ListMeta"/> for lists or <see cref="V1ObjectMeta"/> for objects</typeparam>
    public interface IMetadata<T>
    {
        /// <summary>
        /// Gets or sets standard object's metadata. More info:
        /// https://git.k8s.io/community/contributors/devel/sig-architecture/api-conventions.md#metadata
        /// </summary>
        T Metadata { get; set; }
    }
}
