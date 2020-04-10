using System;
using System.Collections.Generic;
using System.Reflection;
using k8s.Models;

namespace k8s
{
    /// <summary>Represents a map between object types and Kubernetes group, version, and kind triplets.</summary>
    public sealed class KubernetesScheme
    {
        /// <summary>Gets the Kubernetes group, version, and kind for the given type of object.</summary>
        public void GetGVK(Type type, out string group, out string version, out string kind) =>
            GetGVK(type, out group, out version, out kind, out string path);

        /// <summary>Gets the Kubernetes group, version, kind, and API path segment for the given type of object.</summary>
        public void GetGVK(Type type, out string group, out string version, out string kind, out string path)
        {
            if (!TryGetGVK(type, out group, out version, out kind, out path))
            {
                throw new ArgumentException($"The GVK of type {type.Name} is unknown.");
            }
        }

        /// <summary>Gets the Kubernetes group, version, and kind for the given type of object.</summary>
        public void GetGVK<T>(out string group, out string version, out string kind) =>
            GetGVK(typeof(T), out group, out version, out kind, out string path);

        /// <summary>Gets the Kubernetes group, version, kind, and API path segment for the given type of object.</summary>
        public void GetGVK<T>(out string group, out string version, out string kind, out string path) =>
            GetGVK(typeof(T), out group, out version, out kind, out path);

        /// <summary>Gets the Kubernetes API version (including the group, if any), and kind for the given type of object.</summary>
        public void GetVK(Type type, out string apiVersion, out string kind) => GetVK(type, out apiVersion, out kind, out string path);

        /// <summary>Gets the Kubernetes API version (including the group, if any), kind, and API path segment for the given type of
        /// object.
        /// </summary>
        public void GetVK(Type type, out string apiVersion, out string kind, out string path)
        {
            string group, version;
            GetGVK(type, out group, out version, out kind, out path);
            apiVersion = string.IsNullOrEmpty(group) ? version : group + "/" + version;
        }

        /// <summary>Gets the Kubernetes API version (including the group, if any) and kind for the given type of object.</summary>
        public void GetVK<T>(out string apiVersion, out string kind) => GetVK(typeof(T), out apiVersion, out kind, out string path);

        /// <summary>Gets the Kubernetes API version (including the group, if any), kind, and API path segment for the given type of
        /// object.
        /// </summary>
        public void GetVK<T>(out string apiVersion, out string kind, out string path) =>
            GetVK(typeof(T), out apiVersion, out kind, out path);

        /// <summary>Creates a new Kubernetes object of the given type and sets its <see cref="IKubernetesObject.ApiVersion"/> and
        /// <see cref="IKubernetesObject.Kind"/>.
        /// </summary>
        public T New<T>() where T : IKubernetesObject, new()
        {
            string apiVersion, kind;
            GetVK(typeof(T), out apiVersion, out kind);
            return new T() { ApiVersion = apiVersion, Kind = kind };
        }

        /// <summary>Creates a new Kubernetes object of the given type and sets its <see cref="IKubernetesObject.ApiVersion"/>,
        /// <see cref="IKubernetesObject.Kind"/>, and <see cref="V1ObjectMeta.Name"/>.
        /// </summary>
        public T New<T>(string name) where T : IKubernetesObject<V1ObjectMeta>, new() => New<T>(null, name);

        /// <summary>Creates a new Kubernetes object of the given type and sets its <see cref="IKubernetesObject.ApiVersion"/>,
        /// <see cref="IKubernetesObject.Kind"/>, <see cref="V1ObjectMeta.Namespace"/>, and <see cref="V1ObjectMeta.Name"/>.
        /// </summary>
        public T New<T>(string ns, string name) where T : IKubernetesObject<V1ObjectMeta>, new()
        {
            T obj = New<T>();
            obj.Metadata = new V1ObjectMeta() { NamespaceProperty = ns, Name = name };
            return obj;
        }

        /// <summary>Removes GVK information about the given type of object.</summary>
        public void RemoveGVK(Type type)
        {
            lock (gvks) gvks.Remove(type);
        }

        /// <summary>Removes GVK information about the given type of object.</summary>
        public void RemoveGVK<T>() => RemoveGVK(typeof(T));

        /// <summary>Sets GVK information for the given type of object.</summary>
        public void SetGVK(Type type, string group, string version, string kind, string path)
        {
            if (type == null) throw new ArgumentNullException(nameof(type));
            if (string.IsNullOrEmpty(version)) throw new ArgumentNullException(nameof(version));
            if (string.IsNullOrEmpty(kind)) throw new ArgumentNullException(nameof(kind));
            if (string.IsNullOrEmpty(path)) throw new ArgumentNullException(nameof(path));
            lock (gvks) gvks[type] = Tuple.Create(group ?? string.Empty, version, kind, path);
        }

        /// <summary>Sets GVK information for the given type of object.</summary>
        public void SetGVK<T>(string group, string version, string kind, string path) =>
            SetGVK(typeof(T), group, version, kind, path);

        /// <summary>Gets the Kubernetes group, version, and kind for the given type of object.</summary>
        public bool TryGetGVK(Type type, out string group, out string version, out string kind)
            => TryGetGVK(type, out group, out version, out kind, out string path);

        /// <summary>Gets the Kubernetes group, version, kind, and API path segment for the given type of object.</summary>
        public bool TryGetGVK(Type type, out string group, out string version, out string kind, out string path)
        {
            if (type == null) throw new ArgumentNullException(nameof(type));
            lock (gvks)
            {
                if (!gvks.TryGetValue(type, out Tuple<string,string,string,string> gvk))
                {
                    var attr = type.GetCustomAttribute<k8s.Models.KubernetesEntityAttribute>(); // newer types have this attribute
                    if (attr != null)
                    {
                        gvk = Tuple.Create(attr.Group, attr.ApiVersion, attr.Kind, attr.PluralName);
                    }
                    else // some older types (and ours) just have static/const fields
                    {
                        const BindingFlags Flags = BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static;
                        FieldInfo kindf = type.GetField("KubeKind", Flags), versionf = type.GetField("KubeApiVersion", Flags);
                        if (kindf != null && versionf != null)
                        {
                            FieldInfo groupf = type.GetField("KubeGroup", Flags);
                            string k = (string)kindf.GetValue(null);
                            gvk = Tuple.Create(
                                (string)groupf?.GetValue(null) ?? string.Empty, (string)versionf.GetValue(null), k, GuessPath(k));
                        }
                    }
                    gvks[type] = gvk;
                }

                if (gvk != null)
                {
                    (group, version, kind, path) = gvk;
                    return true;
                }
                else
                {
                    group = version = kind = path = null;
                    return false;
                }
            }
        }

        /// <summary>Gets the Kubernetes group, version, and kind for the given type of object.</summary>
        public bool TryGetGVK<T>(out string group, out string version, out string kind) =>
            TryGetGVK(typeof(T), out group, out version, out kind, out string path);

        /// <summary>Gets the Kubernetes group, version, kind, and API path segment for the given type of object.</summary>
        public bool TryGetGVK<T>(out string group, out string version, out string kind, out string path) =>
            TryGetGVK(typeof(T), out group, out version, out kind, out path);

        /// <summary>Gets the Kubernetes API version (including the group, if any) and kind for the given type of object.</summary>
        public bool TryGetVK(Type type, out string apiVersion, out string kind) =>
            TryGetVK(type, out apiVersion, out kind, out string path);

        /// <summary>Gets the Kubernetes API version (including the group, if any), kind, and API path segment for the given type of
        /// object.
        /// </summary>
        public bool TryGetVK(Type type, out string apiVersion, out string kind, out string path)
        {
            if (TryGetGVK(type, out string group, out string version, out kind, out path))
            {
                apiVersion = string.IsNullOrEmpty(group) ? version : group + "/" + version;
                return true;
            }
            else
            {
                apiVersion = null;
                return false;
            }
        }

        /// <summary>Gets the Kubernetes API version (including the group, if any) and kind for the given type of object.</summary>
        public bool TryGetVK<T>(out string apiVersion, out string kind) => TryGetVK(typeof(T), out apiVersion, out kind, out string path);

        /// <summary>Gets the Kubernetes API version (including the group, if any), kind, and API path segment for the given type of
        /// object.
        /// </summary>
        public bool TryGetVK<T>(out string apiVersion, out string kind, out string path) =>
            TryGetVK(typeof(T), out apiVersion, out kind, out path);

        /// <summary>Gets the default <see cref="KubernetesScheme"/>.</summary>
        public static readonly KubernetesScheme Default = new KubernetesScheme();

        /// <summary>Attempts to guess a type's API path segment based on its kind.</summary>
        internal static string GuessPath(string kind)
        {
            if (string.IsNullOrEmpty(kind)) return null;
            if (kind.Length > 4 && kind.EndsWith("List")) kind = kind.Substring(0, kind.Length-4); // e.g. PodList -> Pod
            kind = kind.ToLowerInvariant(); // e.g. Pod -> pod
            return kind + (kind[kind.Length-1] == 's' ? "es" : "s"); // e.g. pod -> pods
        }

        readonly Dictionary<Type,Tuple<string,string,string,string>> gvks = new Dictionary<Type, Tuple<string,string,string,string>>();
    }
}
