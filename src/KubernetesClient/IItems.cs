using System.Collections.Generic;

namespace k8s
{
    /// <summary>
    /// Kubernetes object that exposes list of objects
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IItems<T>
    {
        /// <summary>
        /// Gets or sets list of objects. More info:
        /// https://git.k8s.io/community/contributors/devel/sig-architecture/api-conventions.md
        /// </summary>
        IList<T> Items { get; set; }
    }
}