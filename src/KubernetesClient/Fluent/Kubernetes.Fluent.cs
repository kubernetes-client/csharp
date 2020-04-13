using System;
using System.Net.Http;
using k8s.Models;

namespace k8s.Fluent
{
    public static class KubernetesFluent
    {
        /// <summary>Creates a new Kubernetes object of the given type and sets its <see cref="IKubernetesObject.ApiVersion"/> and
        /// <see cref="IKubernetesObject.Kind"/>.
        /// </summary>
        public static T New<T>(this Kubernetes client) where T : IKubernetesObject, new() => client.Scheme.New<T>();

        /// <summary>Creates a new Kubernetes object of the given type and sets its <see cref="IKubernetesObject.ApiVersion"/>,
        /// <see cref="IKubernetesObject.Kind"/>, and <see cref="V1ObjectMeta.Name"/>.
        /// </summary>
        public static T New<T>(this Kubernetes client, string name) where T : IKubernetesObject<V1ObjectMeta>, new() => client.Scheme.New<T>(name);

        /// <summary>Creates a new Kubernetes object of the given type and sets its <see cref="IKubernetesObject.ApiVersion"/>,
        /// <see cref="IKubernetesObject.Kind"/>, <see cref="V1ObjectMeta.Namespace"/>, and <see cref="V1ObjectMeta.Name"/>.
        /// </summary>
        public static T New<T>(this Kubernetes client, string ns, string name) where T : IKubernetesObject<V1ObjectMeta>, new() => client.Scheme.New<T>(ns, name);

        /// <summary>Creates a new <see cref="KubernetesRequest"/> using the given <see cref="HttpMethod"/>
        /// (<see cref="HttpMethod.Get"/> by default).
        /// </summary>
        public static KubernetesRequest Request(this Kubernetes client, HttpMethod method = null) => new KubernetesRequest(client).Method(method);

        /// <summary>Creates a new <see cref="KubernetesRequest"/> using the given <see cref="HttpMethod"/>
        /// and resource URI components.
        /// </summary>
        public static KubernetesRequest Request(this Kubernetes client, 
            HttpMethod method, string type = null, string ns = null, string name = null, string group = null, string version = null) =>
            new KubernetesRequest(client).Method(method).Group(group).Version(version).Type(type).Namespace(ns).Name(name);

        /// <summary>Creates a new <see cref="KubernetesRequest"/> to access the given type of object.</summary>
        public static KubernetesRequest Request(this Kubernetes client, Type type) => new KubernetesRequest(client).GVK(type);

        /// <summary>Creates a new <see cref="KubernetesRequest"/> to access the given type of object with an optional name and namespace.</summary>
        public static KubernetesRequest Request(this Kubernetes client, HttpMethod method, Type type, string ns = null, string name = null) =>
            Request(client, method).GVK(type).Namespace(ns).Name(name);

        /// <summary>Creates a new <see cref="KubernetesRequest"/> to access the given type of object with an optional name and namespace.</summary>
        public static KubernetesRequest Request<T>(this Kubernetes client, string ns = null, string name = null) => Request(client, null, typeof(T), ns, name);

        /// <summary>Creates a new <see cref="KubernetesRequest"/> to access the given object.</summary>
        public static KubernetesRequest Request(this Kubernetes client, IKubernetesObject obj, bool setBody = true) => new KubernetesRequest(client).Set(obj, setBody);
    }
}
