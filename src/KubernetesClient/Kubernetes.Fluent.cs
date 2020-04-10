using System;
using System.Net.Http;
using k8s.Models;

namespace k8s
{
    public partial class Kubernetes
    {
        /// <summary>Creates a new Kubernetes object of the given type and sets its <see cref="IKubernetesObject.ApiVersion"/> and
        /// <see cref="IKubernetesObject.Kind"/>.
        /// </summary>
        public T New<T>() where T : IKubernetesObject, new() => Scheme.New<T>();

        /// <summary>Creates a new Kubernetes object of the given type and sets its <see cref="IKubernetesObject.ApiVersion"/>,
        /// <see cref="IKubernetesObject.Kind"/>, and <see cref="V1ObjectMeta.Name"/>.
        /// </summary>
        public T New<T>(string name) where T : IKubernetesObject<V1ObjectMeta>, new() => Scheme.New<T>(name);

        /// <summary>Creates a new Kubernetes object of the given type and sets its <see cref="IKubernetesObject.ApiVersion"/>,
        /// <see cref="IKubernetesObject.Kind"/>, <see cref="V1ObjectMeta.Namespace"/>, and <see cref="V1ObjectMeta.Name"/>.
        /// </summary>
        public T New<T>(string ns, string name) where T : IKubernetesObject<V1ObjectMeta>, new() => Scheme.New<T>(ns, name);

        /// <summary>Creates a new <see cref="KubernetesRequest"/> using the given <see cref="HttpMethod"/>
        /// (<see cref="HttpMethod.Get"/> by default).
        /// </summary>
        public KubernetesRequest Request(HttpMethod method = null) => new KubernetesRequest(this).Method(method);

        /// <summary>Creates a new <see cref="KubernetesRequest"/> using the given <see cref="HttpMethod"/>
        /// and resource URI components.
        /// </summary>
        public KubernetesRequest Request(
            HttpMethod method, string type = null, string ns = null, string name = null, string group = null, string version = null) =>
            new KubernetesRequest(this).Method(method).Group(group).Version(version).Type(type).Namespace(ns).Name(name);

        /// <summary>Creates a new <see cref="KubernetesRequest"/> to access the given type of object.</summary>
        public KubernetesRequest Request(Type type) => new KubernetesRequest(this).GVK(type);

        /// <summary>Creates a new <see cref="KubernetesRequest"/> to access the given type of object with an optional name and namespace.</summary>
        public KubernetesRequest Request(HttpMethod method, Type type, string ns = null, string name = null) =>
            Request(method).GVK(type).Namespace(ns).Name(name);

        /// <summary>Creates a new <see cref="KubernetesRequest"/> to access the given type of object with an optional name and namespace.</summary>
        public KubernetesRequest Request<T>(string ns = null, string name = null) => Request(null, typeof(T), ns, name);

        /// <summary>Creates a new <see cref="KubernetesRequest"/> to access the given object.</summary>
        public KubernetesRequest Request(IKubernetesObject obj, bool setBody = true) => new KubernetesRequest(this).Set(obj, setBody);
    }
}
