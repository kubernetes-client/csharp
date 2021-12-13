using k8s.Models;

namespace k8s.Util.Informer.Cache
{
    /// <summary>
    /// A set of helper utilities for constructing a cache.
    /// </summary>
    public static class Caches
    {
        /// <summary>
        /// NamespaceIndex is the default index function for caching objects
        /// </summary>
        public const string NamespaceIndex = "namespace";

        /// <summary>
        /// deletionHandlingMetaNamespaceKeyFunc checks for DeletedFinalStateUnknown objects before calling
        /// metaNamespaceKeyFunc.
        /// </summary>
        /// <param name="obj">specific object</param>
        /// <typeparam name="TApiType">the type parameter</typeparam>
        /// <exception cref="ArgumentNullException">if obj is null</exception>
        /// <returns>the key</returns>
        public static string DeletionHandlingMetaNamespaceKeyFunc<TApiType>(TApiType obj)
          where TApiType : class, IKubernetesObject<V1ObjectMeta>
        {
            if (obj is null)
            {
                throw new ArgumentNullException(nameof(obj));
            }

            if (obj.GetType() == typeof(DeletedFinalStateUnknown<TApiType>))
            {
                var deleteObj = obj as DeletedFinalStateUnknown<TApiType>;
                return deleteObj.GetKey();
            }

            return MetaNamespaceKeyFunc(obj);
        }

        /// <summary>
        /// MetaNamespaceKeyFunc is a convenient default KeyFunc which knows how to make keys for API
        /// objects which implement V1ObjectMeta Interface. The key uses the format &lt;namespace&gt;/&lt;name&gt;
        /// unless &lt;namespace&gt; is empty, then it's just &lt;name&gt;.
        /// </summary>
        /// <param name="obj">specific object</param>
        /// <returns>the key</returns>
        /// <exception cref="ArgumentNullException">if obj is null</exception>
        /// <exception cref="InvalidOperationException">if metadata can't be found on obj</exception>
        public static string MetaNamespaceKeyFunc(IKubernetesObject<V1ObjectMeta> obj)
        {
            if (obj is null)
            {
                throw new ArgumentNullException(nameof(obj));
            }

            if (!string.IsNullOrEmpty(obj.Metadata.NamespaceProperty))
            {
                return obj.Metadata.NamespaceProperty + "/" + obj.Metadata.Name;
            }

            return obj.Metadata.Name;
        }

        /// <summary>
        /// MetaNamespaceIndexFunc is a default index function that indexes based on an object's namespace.
        /// </summary>
        /// <param name="obj">specific object</param>
        /// <typeparam name="TApiType">the type parameter</typeparam>
        /// <returns>the indexed value</returns>
        /// <exception cref="ArgumentNullException">if obj is null</exception>
        /// <exception cref="InvalidOperationException">if metadata can't be found on obj</exception>
        public static List<string> MetaNamespaceIndexFunc<TApiType>(TApiType obj)
          where TApiType : IKubernetesObject<V1ObjectMeta>
        {
            if (obj is null)
            {
                throw new ArgumentNullException(nameof(obj));
            }

            return obj.Metadata is null ? new List<string>() : new List<string>() { obj.Metadata.NamespaceProperty };
        }
    }
}
