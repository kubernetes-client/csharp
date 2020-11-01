using System;
using System.Collections.Generic;
using System.Linq;

namespace k8s.Models
{
    /// <summary>Adds convenient extensions for Kubernetes objects.</summary>
    public static class ModelExtensions
    {
        /// <summary>Adds the given finalizer to a Kubernetes object if it doesn't already exist.</summary>
        /// <returns>Returns true if the finalizer was added and false if it already existed.</returns>
        public static bool AddFinalizer(this IMetadata<V1ObjectMeta> obj, string finalizer)
        {
            if (string.IsNullOrEmpty(finalizer))
            {
                throw new ArgumentNullException(nameof(finalizer));
            }

            if (EnsureMetadata(obj).Finalizers == null)
            {
                obj.Metadata.Finalizers = new List<string>();
            }

            if (!obj.Metadata.Finalizers.Contains(finalizer))
            {
                obj.Metadata.Finalizers.Add(finalizer);
                return true;
            }

            return false;
        }

        /// <summary>Extracts the Kubernetes API group from the <see cref="IKubernetesObject.ApiVersion"/>.</summary>
        public static string ApiGroup(this IKubernetesObject obj)
        {
            if (obj == null)
            {
                throw new ArgumentNullException(nameof(obj));
            }

            if (obj.ApiVersion != null)
            {
                int slash = obj.ApiVersion.IndexOf('/');
                return slash < 0 ? string.Empty : obj.ApiVersion.Substring(0, slash);
            }

            return null;
        }

        /// <summary>Extracts the Kubernetes API version (excluding the group) from the <see cref="IKubernetesObject.ApiVersion"/>.</summary>
        public static string ApiGroupVersion(this IKubernetesObject obj)
        {
            if (obj == null)
            {
                throw new ArgumentNullException(nameof(obj));
            }

            if (obj.ApiVersion != null)
            {
                int slash = obj.ApiVersion.IndexOf('/');
                return slash < 0 ? obj.ApiVersion : obj.ApiVersion.Substring(slash + 1);
            }

            return null;
        }

        /// <summary>Splits the Kubernetes API version into the group and version.</summary>
        public static (string, string) ApiGroupAndVersion(this IKubernetesObject obj)
        {
            string group, version;
            GetApiGroupAndVersion(obj, out group, out version);
            return (group, version);
        }

        /// <summary>Splits the Kubernetes API version into the group and version.</summary>
        public static void GetApiGroupAndVersion(this IKubernetesObject obj, out string group, out string version)
        {
            if (obj == null)
            {
                throw new ArgumentNullException(nameof(obj));
            }

            if (obj.ApiVersion == null)
            {
                group = version = null;
            }
            else
            {
                int slash = obj.ApiVersion.IndexOf('/');
                if (slash < 0)
                {
                    (group, version) = (string.Empty, obj.ApiVersion);
                }
                else
                {
                    (group, version) = (obj.ApiVersion.Substring(0, slash), obj.ApiVersion.Substring(slash + 1));
                }
            }
        }

        /// <summary>Gets the continuation token version of a Kubernetes list.</summary>
        public static string Continue(this IMetadata<V1ListMeta> list) => list.Metadata?.ContinueProperty;

        /// <summary>Ensures that the <see cref="V1ListMeta"/> metadata field is set, and returns it.</summary>
        public static V1ListMeta EnsureMetadata(this IMetadata<V1ListMeta> obj)
        {
            if (obj == null)
            {
                throw new ArgumentNullException(nameof(obj));
            }

            if (obj.Metadata == null)
            {
                obj.Metadata = new V1ListMeta();
            }

            return obj.Metadata;
        }

        /// <summary>Gets the resource version of a Kubernetes list.</summary>
        public static string ResourceVersion(this IMetadata<V1ListMeta> list) => list.Metadata?.ResourceVersion;

        /// <summary>Adds an owner reference to the object. No attempt is made to ensure the reference is correct or fits with the
        /// other references.
        /// </summary>
        public static void AddOwnerReference(this IMetadata<V1ObjectMeta> obj, V1OwnerReference ownerRef)
        {
            if (ownerRef == null)
            {
                throw new ArgumentNullException(nameof(ownerRef));
            }

            if (EnsureMetadata(obj).OwnerReferences == null)
            {
                obj.Metadata.OwnerReferences = new List<V1OwnerReference>();
            }

            obj.Metadata.OwnerReferences.Add(ownerRef);
        }

        /// <summary>Gets the annotations of a Kubernetes object.</summary>
        public static IDictionary<string, string> Annotations(this IMetadata<V1ObjectMeta> obj) =>
            obj.Metadata?.Annotations;

        /// <summary>Gets the creation time of a Kubernetes object, or null if it hasn't been created yet.</summary>
        public static DateTime? CreationTimestamp(this IMetadata<V1ObjectMeta> obj) => obj.Metadata?.CreationTimestamp;

        /// <summary>Gets the deletion time of a Kubernetes object, or null if it hasn't been scheduled for deletion.</summary>
        public static DateTime? DeletionTimestamp(this IMetadata<V1ObjectMeta> obj) => obj.Metadata?.DeletionTimestamp;

        /// <summary>Ensures that the <see cref="V1ObjectMeta"/> metadata field is set, and returns it.</summary>
        public static V1ObjectMeta EnsureMetadata(this IMetadata<V1ObjectMeta> obj)
        {
            if (obj == null)
            {
                throw new ArgumentNullException(nameof(obj));
            }

            if (obj.Metadata == null)
            {
                obj.Metadata = new V1ObjectMeta();
            }

            return obj.Metadata;
        }

        /// <summary>Gets the <see cref="V1ObjectMeta.Finalizers"/> of a Kubernetes object.</summary>
        public static IList<string> Finalizers(this IMetadata<V1ObjectMeta> obj) => obj.Metadata?.Finalizers;

        /// <summary>Gets the index of the <see cref="V1OwnerReference"/> that matches the given object, or -1 if no such
        /// reference could be found.
        /// </summary>
        public static int FindOwnerReference(this IMetadata<V1ObjectMeta> obj, IKubernetesObject<V1ObjectMeta> owner) =>
            FindOwnerReference(obj, r => r.Matches(owner));

        /// <summary>Gets the index of the <see cref="V1OwnerReference"/> that matches the given predicate, or -1 if no such
        /// reference could be found.
        /// </summary>
        public static int FindOwnerReference(this IMetadata<V1ObjectMeta> obj, Predicate<V1OwnerReference> predicate)
        {
            if (obj == null)
            {
                throw new ArgumentNullException(nameof(obj));
            }

            if (predicate == null)
            {
                throw new ArgumentNullException(nameof(predicate));
            }

            var ownerRefs = obj.OwnerReferences();
            if (ownerRefs != null)
            {
                for (int i = 0; i < ownerRefs.Count; i++)
                {
                    if (predicate(ownerRefs[i]))
                    {
                        return i;
                    }
                }
            }

            return -1;
        }

        /// <summary>Gets the generation a Kubernetes object.</summary>
        public static long? Generation(this IMetadata<V1ObjectMeta> obj) => obj.Metadata?.Generation;

        /// <summary>Returns the given annotation from a Kubernetes object or null if the annotation was not found.</summary>
        public static string GetAnnotation(this IMetadata<V1ObjectMeta> obj, string key)
        {
            if (obj == null)
            {
                throw new ArgumentNullException(nameof(obj));
            }

            if (key == null)
            {
                throw new ArgumentNullException(nameof(key));
            }

            IDictionary<string, string> annotations = obj.Annotations();
            return annotations != null && annotations.TryGetValue(key, out string value) ? value : null;
        }

        /// <summary>Gets the <see cref="V1OwnerReference"/> for the controller of this object, or null if it couldn't be found.</summary>
        public static V1OwnerReference GetController(this IMetadata<V1ObjectMeta> obj) =>
            obj.OwnerReferences()?.FirstOrDefault(r => r.Controller.GetValueOrDefault());

        /// <summary>Returns the given label from a Kubernetes object or null if the label was not found.</summary>
        public static string GetLabel(this IMetadata<V1ObjectMeta> obj, string key)
        {
            if (obj == null)
            {
                throw new ArgumentNullException(nameof(obj));
            }

            if (key == null)
            {
                throw new ArgumentNullException(nameof(key));
            }

            IDictionary<string, string> labels = obj.Labels();
            return labels != null && labels.TryGetValue(key, out string value) ? value : null;
        }

        /// <summary>Gets <see cref="V1OwnerReference"/> that matches the given object, or null if no matching reference exists.</summary>
        public static V1OwnerReference GetOwnerReference(
            this IMetadata<V1ObjectMeta> obj,
            IKubernetesObject<V1ObjectMeta> owner) =>
            GetOwnerReference(obj, r => r.Matches(owner));

        /// <summary>Gets the <see cref="V1OwnerReference"/> that matches the given predicate, or null if no matching reference exists.</summary>
        public static V1OwnerReference GetOwnerReference(
            this IMetadata<V1ObjectMeta> obj,
            Predicate<V1OwnerReference> predicate)
        {
            int index = FindOwnerReference(obj, predicate);
            return index >= 0 ? obj.Metadata.OwnerReferences[index] : null;
        }

        /// <summary>Determines whether the Kubernetes object has the given finalizer.</summary>
        public static bool HasFinalizer(this IMetadata<V1ObjectMeta> obj, string finalizer)
        {
            if (obj == null)
            {
                throw new ArgumentNullException(nameof(obj));
            }

            if (string.IsNullOrEmpty(finalizer))
            {
                throw new ArgumentNullException(nameof(finalizer));
            }

            return obj.Finalizers() != null && obj.Metadata.Finalizers.Contains(finalizer);
        }

        /// <summary>Determines whether one object is owned by another.</summary>
        public static bool IsOwnedBy(this IMetadata<V1ObjectMeta> obj, IKubernetesObject<V1ObjectMeta> owner) =>
            FindOwnerReference(obj, owner) >= 0;

        /// <summary>Gets the labels of a Kubernetes object.</summary>
        public static IDictionary<string, string> Labels(this IMetadata<V1ObjectMeta> obj) => obj.Metadata?.Labels;

        /// <summary>Gets the name of a Kubernetes object.</summary>
        public static string Name(this IMetadata<V1ObjectMeta> obj) => obj.Metadata?.Name;

        /// <summary>Gets the namespace of a Kubernetes object.</summary>
        public static string Namespace(this IMetadata<V1ObjectMeta> obj) => obj.Metadata?.NamespaceProperty;

        /// <summary>Gets the owner references of a Kubernetes object.</summary>
        public static IList<V1OwnerReference> OwnerReferences(this IMetadata<V1ObjectMeta> obj) =>
            obj.Metadata?.OwnerReferences;

        /// <summary>Removes the given finalizer from a Kubernetes object if it exists.</summary>
        /// <returns>Returns true if the finalizer was removed and false if it didn't exist.</returns>
        public static bool RemoveFinalizer(this IMetadata<V1ObjectMeta> obj, string finalizer)
        {
            if (obj == null)
            {
                throw new ArgumentNullException(nameof(obj));
            }

            if (string.IsNullOrEmpty(finalizer))
            {
                throw new ArgumentNullException(nameof(finalizer));
            }

            return obj.Finalizers() != null && obj.Metadata.Finalizers.Remove(finalizer);
        }

        /// <summary>Removes the first <see cref="V1OwnerReference"/> that matches the given object and returns it, or returns null if no
        /// matching reference could be found.
        /// </summary>
        public static V1OwnerReference RemoveOwnerReference(
            this IMetadata<V1ObjectMeta> obj,
            IKubernetesObject<V1ObjectMeta> owner)
        {
            int index = FindOwnerReference(obj, owner);
            V1OwnerReference ownerRef = index >= 0 ? obj.Metadata.OwnerReferences[index] : null;
            if (index >= 0)
            {
                obj.Metadata.OwnerReferences.RemoveAt(index);
            }

            return ownerRef;
        }

        /// <summary>Removes all <see cref="V1OwnerReference">owner references</see> that match the given predicate, and returns true if
        /// any were removed.
        /// </summary>
        public static bool RemoveOwnerReferences(
            this IMetadata<V1ObjectMeta> obj,
            Predicate<V1OwnerReference> predicate)
        {
            if (obj == null)
            {
                throw new ArgumentNullException(nameof(obj));
            }

            if (predicate == null)
            {
                throw new ArgumentNullException(nameof(predicate));
            }

            bool removed = false;
            IList<V1OwnerReference> refs = obj.Metadata?.OwnerReferences;
            if (refs != null)
            {
                for (int i = refs.Count - 1; i >= 0; i--)
                {
                    if (predicate(refs[i]))
                    {
                        refs.RemoveAt(i);
                        removed = true;
                    }
                }
            }

            return removed;
        }

        /// <summary>Removes all <see cref="V1OwnerReference">owner references</see> that match the given object, and returns true if
        /// any were removed.
        /// </summary>
        public static bool RemoveOwnerReferences(
            this IMetadata<V1ObjectMeta> obj,
            IKubernetesObject<V1ObjectMeta> owner) =>
            RemoveOwnerReferences(obj, r => r.Matches(owner));

        /// <summary>Gets the resource version of a Kubernetes object.</summary>
        public static string ResourceVersion(this IMetadata<V1ObjectMeta> obj) => obj.Metadata?.ResourceVersion;

        /// <summary>Sets or removes an annotation on a Kubernetes object.</summary>
        public static void SetAnnotation(this IMetadata<V1ObjectMeta> obj, string key, string value)
        {
            if (obj == null)
            {
                throw new ArgumentNullException(nameof(obj));
            }

            if (key == null)
            {
                throw new ArgumentNullException(nameof(key));
            }

            if (value != null)
            {
                obj.EnsureMetadata().EnsureAnnotations()[key] = value;
            }
            else
            {
                obj.Metadata?.Annotations?.Remove(key);
            }
        }

        /// <summary>Sets or removes a label on a Kubernetes object.</summary>
        public static void SetLabel(this IMetadata<V1ObjectMeta> obj, string key, string value)
        {
            if (obj == null)
            {
                throw new ArgumentNullException(nameof(obj));
            }

            if (key == null)
            {
                throw new ArgumentNullException(nameof(key));
            }

            if (value != null)
            {
                obj.EnsureMetadata().EnsureLabels()[key] = value;
            }
            else
            {
                obj.Metadata?.Labels?.Remove(key);
            }
        }

        /// <summary>Gets the unique ID of a Kubernetes object.</summary>
        public static string Uid(this IMetadata<V1ObjectMeta> obj) => obj.Metadata?.Uid;

        /// <summary>Ensures that the <see cref="V1ObjectMeta.Annotations"/> field is not null, and returns it.</summary>
        public static IDictionary<string, string> EnsureAnnotations(this V1ObjectMeta meta)
        {
            if (meta == null)
            {
                throw new ArgumentNullException(nameof(meta));
            }

            if (meta.Annotations == null)
            {
                meta.Annotations = new Dictionary<string, string>();
            }

            return meta.Annotations;
        }

        /// <summary>Ensures that the <see cref="V1ObjectMeta.Finalizers"/> field is not null, and returns it.</summary>
        public static IList<string> EnsureFinalizers(this V1ObjectMeta meta)
        {
            if (meta == null)
            {
                throw new ArgumentNullException(nameof(meta));
            }

            if (meta.Finalizers == null)
            {
                meta.Finalizers = new List<string>();
            }

            return meta.Finalizers;
        }

        /// <summary>Ensures that the <see cref="V1ObjectMeta.Labels"/> field is not null, and returns it.</summary>
        public static IDictionary<string, string> EnsureLabels(this V1ObjectMeta meta)
        {
            if (meta == null)
            {
                throw new ArgumentNullException(nameof(meta));
            }

            if (meta.Labels == null)
            {
                meta.Labels = new Dictionary<string, string>();
            }

            return meta.Labels;
        }

        /// <summary>Gets the namespace from Kubernetes metadata.</summary>
        public static string Namespace(this V1ObjectMeta meta) => meta.NamespaceProperty;

        /// <summary>Sets the namespace from Kubernetes metadata.</summary>
        public static void SetNamespace(this V1ObjectMeta meta, string ns) => meta.NamespaceProperty = ns;

        /// <summary>Determines whether an object reference references the given object.</summary>
        public static bool Matches(this V1ObjectReference objref, IKubernetesObject<V1ObjectMeta> obj)
        {
            if (objref == null)
            {
                throw new ArgumentNullException(nameof(objref));
            }

            if (obj == null)
            {
                throw new ArgumentNullException(nameof(obj));
            }

            return objref.ApiVersion == obj.ApiVersion && objref.Kind == obj.Kind && objref.Name == obj.Name() &&
                   objref.Uid == obj.Uid() &&
                   objref.NamespaceProperty == obj.Namespace();
        }

        /// <summary>Determines whether an owner reference references the given object.</summary>
        public static bool Matches(this V1OwnerReference owner, IKubernetesObject<V1ObjectMeta> obj)
        {
            if (owner == null)
            {
                throw new ArgumentNullException(nameof(owner));
            }

            if (obj == null)
            {
                throw new ArgumentNullException(nameof(obj));
            }

            return owner.ApiVersion == obj.ApiVersion && owner.Kind == obj.Kind && owner.Name == obj.Name() &&
                   owner.Uid == obj.Uid();
        }
    }
}
