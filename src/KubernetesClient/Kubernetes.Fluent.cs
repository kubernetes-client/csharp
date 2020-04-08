using System;
using System.Net.Http;

namespace k8s
{
    public partial class Kubernetes
    {
        /// <summary>Creates a new <see cref="KubernetesRequest"/> using the given <see cref="HttpMethod"/>
        /// (<see cref="HttpMethod.Get"/> by default).
        /// </summary>
        public KubernetesRequest New(HttpMethod method = null) => new KubernetesRequest(this).Method(method);

        /// <summary>Creates a new <see cref="KubernetesRequest"/> using the given <see cref="HttpMethod"/>
        /// and resource URI components.
        /// </summary>
        public KubernetesRequest New(
            HttpMethod method, string type = null, string ns = null, string name = null, string group = null, string version = null) =>
            new KubernetesRequest(this).Method(method).Group(group).Version(version).Type(type).Namespace(ns).Name(name);

        /// <summary>Creates a new <see cref="KubernetesRequest"/> to access the given type of object.</summary>
        public KubernetesRequest New(Type type) => new KubernetesRequest(this).GVK(type);

        /// <summary>Creates a new <see cref="KubernetesRequest"/> to access the given type of object with an optional name and namespace.</summary>
        public KubernetesRequest New(HttpMethod method, Type type, string ns = null, string name = null) =>
            New(method).GVK(type).Namespace(ns).Name(name);

        /// <summary>Creates a new <see cref="KubernetesRequest"/> to access the given type of object with an optional name and namespace.</summary>
        public KubernetesRequest New<T>(string ns = null, string name = null) => New(null, typeof(T), ns, name);

        /// <summary>Creates a new <see cref="KubernetesRequest"/> to access the given object.</summary>
        public KubernetesRequest New(IKubernetesObject obj, bool setBody = true) => new KubernetesRequest(this).Set(obj, setBody);
    }
}
