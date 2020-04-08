using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;

namespace k8s.Models
{
    public static class ModelExtensions
    {
        /// <summary>Extracts the Kubernetes API group from the <see cref="IKubernetesObject.ApiVersion"/>.</summary>
        public static string ApiGroup(this IKubernetesObject obj)
        {
            if(obj == null) throw new ArgumentNullException(nameof(obj));
            if(obj.ApiVersion == null) return null;
            int slash = obj.ApiVersion.IndexOf('/');
            return slash < 0 ? string.Empty : obj.ApiVersion.Substring(0, slash);
        }

        /// <summary>Extracts the Kubernetes API version (excluding the group) from the <see cref="IKubernetesObject.ApiVersion"/>.</summary>
        public static string ApiGroupVersion(this IKubernetesObject obj)
        {
            if(obj == null) throw new ArgumentNullException(nameof(obj));
            if(obj.ApiVersion == null) return null;
            int slash = obj.ApiVersion.IndexOf('/');
            return slash < 0 ? obj.ApiVersion : obj.ApiVersion.Substring(slash+1);
        }

        /// <summary>Splits the Kubernetes API version into the group and version.</summary>
        public static (string, string) ApiGroupAndVersion(this IKubernetesObject obj)
        {
            string group, version;
            obj.GetApiGroupAndVersion(out group, out version);
            return (group, version);
        }

        /// <summary>Splits the Kubernetes API version into the group and version.</summary>
        public static void GetApiGroupAndVersion(this IKubernetesObject obj, out string group, out string version)
        {
            if(obj == null) throw new ArgumentNullException(nameof(obj));
            if(obj.ApiVersion == null)
            {
                group = version = null;
            }
            else
            {
                int slash = obj.ApiVersion.IndexOf('/');
                if(slash < 0) (group, version) = (string.Empty, obj.ApiVersion);
                else (group, version) = (obj.ApiVersion.Substring(0, slash), obj.ApiVersion.Substring(slash+1));
            }
        }

        /// <summary>Gets the continuation token version of a Kubernetes list.</summary>
        public static string Continue(this IMetadata<V1ListMeta> list) => list.Metadata?.ContinueProperty;

        /// <summary>Ensures that the <see cref="V1ListMeta"/> metadata field is set, and returns it.</summary>
        public static V1ListMeta EnsureMetadata(this IMetadata<V1ListMeta> obj)
        {
            if(obj.Metadata == null) obj.Metadata = new V1ListMeta();
            return obj.Metadata;
        }

        /// <summary>Gets the resource version of a Kubernetes list.</summary>
        public static string ResourceVersion(this IMetadata<V1ListMeta> list) => list.Metadata?.ResourceVersion;

        /// <summary>Adds an owner reference to the object. No attempt is made to ensure the reference is correct or fits with the
        /// other references.
        /// </summary>
        public static void AddOwnerReference(this IMetadata<V1ObjectMeta> obj, V1OwnerReference ownerRef)
        {
            if(obj == null) throw new ArgumentNullException(nameof(obj));
            if(ownerRef == null) throw new ArgumentNullException(nameof(ownerRef));
            if(obj.EnsureMetadata().OwnerReferences == null) obj.Metadata.OwnerReferences = new List<V1OwnerReference>();
            obj.Metadata.OwnerReferences.Add(ownerRef);
        }

        /// <summary>Gets the annotations of a Kubernetes object.</summary>
        public static IDictionary<string, string> Annotations(this IMetadata<V1ObjectMeta> obj) => obj.Metadata?.Annotations;

        /// <summary>Gets the creation time of a Kubernetes object, or null if it hasn't been created yet.</summary>
        public static DateTime? CreationTimestamp(this IMetadata<V1ObjectMeta> obj) => obj.Metadata?.CreationTimestamp;

        /// <summary>Gets the deletion time of a Kubernetes object, or null if it hasn't been scheduled for deletion.</summary>
        public static DateTime? DeletionTimestamp(this IMetadata<V1ObjectMeta> obj) => obj.Metadata?.DeletionTimestamp;

        /// <summary>Ensures that the <see cref="V1ObjectMeta"/> metadata field is set, and returns it.</summary>
        public static V1ObjectMeta EnsureMetadata(this IMetadata<V1ObjectMeta> obj)
        {
            if(obj.Metadata == null) obj.Metadata = new V1ObjectMeta();
            return obj.Metadata;
        }

        /// <summary>Gets the index of the <see cref="V1OwnerReference"/> that matches the given object, or -1 if no such
        /// reference could be found.
        /// </summary>
        public static int FindOwnerRef(this IMetadata<V1ObjectMeta> obj, IKubernetesObject<V1ObjectMeta> owner)
        {
            var ownerRefs = obj.OwnerReferences();
            if(ownerRefs != null)
            {
                for(int i = 0; i < ownerRefs.Count; i++)
                {
                    if(ownerRefs[i].Matches(owner)) return i;
                }
            }
            return -1;
        }

        /// <summary>Gets the generation a Kubernetes object.</summary>
        public static long? Generation(this IMetadata<V1ObjectMeta> obj) => obj.Metadata?.Generation;

        /// <summary>Returns the given annotation from a Kubernetes object or null if the annotation was not found.</summary>
        public static string GetAnnotation(this IMetadata<V1ObjectMeta> obj, string key)
        {
            if(obj == null) throw new ArgumentNullException(nameof(obj));
            if(key == null) throw new ArgumentNullException(nameof(key));
            IDictionary<string, string> annotations = obj.Annotations();
            return annotations != null && annotations.TryGetValue(key, out string value) ? value : null;
        }

        /// <summary>Gets the <see cref="V1OwnerReference"/> for the controller of this object, or null if it couldn't be found.</summary>
        public static V1OwnerReference GetController(this IMetadata<V1ObjectMeta> obj) =>
            obj.OwnerReferences()?.FirstOrDefault(r => r.Controller.GetValueOrDefault());

        /// <summary>Returns the given label from a Kubernetes object or null if the label was not found.</summary>
        public static string GetLabel(this IMetadata<V1ObjectMeta> obj, string key)
        {
            if(obj == null) throw new ArgumentNullException(nameof(obj));
            if(key == null) throw new ArgumentNullException(nameof(key));
            IDictionary<string, string> labels = obj.Labels();
            return labels != null && labels.TryGetValue(key, out string value) ? value : null;
        }

        /// <summary>Creates a <see cref="V1ObjectReference"/> that refers to the given object.</summary>
        public static V1ObjectReference GetObjectReference<T>(this T obj) where T : IKubernetesObject, IMetadata<V1ObjectMeta>
        {
            if(obj == null) throw new ArgumentNullException(nameof(obj));
            string apiVersion = obj.ApiVersion, kind = obj.Kind; // default to using the API version and kind from the object
            if(string.IsNullOrEmpty(apiVersion) || string.IsNullOrEmpty(kind)) // but if either of them is missing...
            {
                object[] attrs = typeof(T).GetCustomAttributes(typeof(KubernetesEntityAttribute), true);
                if(attrs.Length == 0) throw new ArgumentException("Unable to determine the object's API version and Kind.");
                var attr = (KubernetesEntityAttribute)attrs[0];
                (apiVersion, kind) = (string.IsNullOrEmpty(attr.Group) ? attr.ApiVersion : attr.Group + "/" + attr.ApiVersion, attr.Kind);
            }
            return new V1ObjectReference()
            {
                ApiVersion = apiVersion, Kind = kind, Name = obj.Name(), NamespaceProperty = obj.Namespace(), Uid = obj.Uid(),
                ResourceVersion = obj.ResourceVersion()
            };
        }

        /// <summary>Gets the labels of a Kubernetes object.</summary>
        public static IDictionary<string, string> Labels(this IMetadata<V1ObjectMeta> obj) => obj.Metadata?.Labels;

        /// <summary>Gets the name of a Kubernetes object.</summary>
        public static string Name(this IMetadata<V1ObjectMeta> obj) => obj.Metadata?.Name;

        /// <summary>Gets the namespace of a Kubernetes object.</summary>
        public static string Namespace(this IMetadata<V1ObjectMeta> obj) => obj.Metadata?.NamespaceProperty;

        /// <summary>Gets the owner references of a Kubernetes object.</summary>
        public static IList<V1OwnerReference> OwnerReferences(this IMetadata<V1ObjectMeta> obj) => obj.Metadata?.OwnerReferences;

        /// <summary>Gets the resource version of a Kubernetes object.</summary>
        public static string ResourceVersion(this IMetadata<V1ObjectMeta> obj) => obj.Metadata?.ResourceVersion;

        /// <summary>Sets or removes an annotation on a Kubernetes object.</summary>
        public static void SetAnnotation(this IMetadata<V1ObjectMeta> obj, string key, string value)
        {
            if(obj == null) throw new ArgumentNullException(nameof(obj));
            if(key == null) throw new ArgumentNullException(nameof(key));
            if(value != null) obj.EnsureMetadata().EnsureAnnotations()[key] = value;
            else obj.Metadata?.Annotations?.Remove(key);
        }

        /// <summary>Sets or removes a label on a Kubernetes object.</summary>
        public static void SetLabel(this IMetadata<V1ObjectMeta> obj, string key, string value)
        {
            if(obj == null) throw new ArgumentNullException(nameof(obj));
            if(key == null) throw new ArgumentNullException(nameof(key));
            if(value != null) obj.EnsureMetadata().EnsureLabels()[key] = value;
            else obj.Metadata?.Labels?.Remove(key);
        }

        /// <summary>Gets the unique ID of a Kubernetes object.</summary>
        public static string Uid(this IMetadata<V1ObjectMeta> obj) => obj.Metadata?.Uid;

        /// <summary>Ensures that the <see cref="V1ObjectMeta.Annotations"/> field is not null, and returns it.</summary>
        public static IDictionary<string, string> EnsureAnnotations(this V1ObjectMeta meta)
        {
            if(meta.Annotations == null) meta.Annotations = new Dictionary<string, string>();
            return meta.Annotations;
        }

        /// <summary>Ensures that the <see cref="V1ObjectMeta.Labels"/> field is not null, and returns it.</summary>
        public static IDictionary<string, string> EnsureLabels(this V1ObjectMeta meta)
        {
            if(meta.Labels == null) meta.Labels = new Dictionary<string, string>();
            return meta.Labels;
        }

        /// <summary>Gets the namespace from Kubernetes metadata.</summary>
        public static string Namespace(this V1ObjectMeta meta) => meta.NamespaceProperty;

        /// <summary>Sets the namespace from Kubernetes metadata.</summary>
        public static void SetNamespace(this V1ObjectMeta meta, string ns) => meta.NamespaceProperty = ns;

        /// <summary>Determines whether an object reference references the given object.</summary>
        public static bool Matches(this V1ObjectReference objref, IKubernetesObject<V1ObjectMeta> obj)
        {
            if(objref == null) throw new ArgumentNullException(nameof(objref));
            if(obj == null) throw new ArgumentNullException(nameof(obj));
            return objref.ApiVersion == obj.ApiVersion && objref.Kind == obj.Kind && objref.Name == obj.Name() && objref.Uid == obj.Uid() &&
                   objref.NamespaceProperty == obj.Namespace();
        }

        /// <summary>Determines whether an owner reference references the given object.</summary>
        public static bool Matches(this V1OwnerReference owner, IKubernetesObject<V1ObjectMeta> obj)
        {
            if(owner == null) throw new ArgumentNullException(nameof(owner));
            if(obj == null) throw new ArgumentNullException(nameof(obj));
            return owner.ApiVersion == obj.ApiVersion && owner.Kind == obj.Kind && owner.Name == obj.Name() && owner.Uid == obj.Uid();
        }
    }

    public partial class V1Status
    {
        /// <summary>Converts a <see cref="V1Status"/> object representing an error into a short description of the error.</summary>
        public override string ToString()
        {
            string reason = Reason;
            if(string.IsNullOrEmpty(reason) && Code.GetValueOrDefault() != 0)
            {
                reason = ((HttpStatusCode)Code.Value).ToString();
            }
            return string.IsNullOrEmpty(Message) ? reason : string.IsNullOrEmpty(reason) ? Message : $"{reason} - {Message}";
        }
    }
}
