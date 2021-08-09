using System.Collections.Generic;
using k8s.Models;

namespace k8s.Util.Informer.Cache
{
    /// <summary>
    /// Lister interface is used to list cached items from a running informer.
    /// </summary>
    /// <typeparam name="TApiType">the type</typeparam>
    public class Lister<TApiType>
      where TApiType : class, IKubernetesObject<V1ObjectMeta>
    {
        private readonly string _namespace;
        private readonly string _indexName;
        private readonly IIndexer<TApiType> _indexer;

        public Lister(IIndexer<TApiType> indexer, string @namespace = default, string indexName = Caches.NamespaceIndex)
        {
            _indexer = indexer;
            _namespace = @namespace;
            _indexName = indexName;
        }

        public IEnumerable<TApiType> List()
        {
            return string.IsNullOrEmpty(_namespace) ? _indexer.List() : _indexer.ByIndex(_indexName, _namespace);
        }

        public TApiType Get(string name)
        {
            var key = name;
            if (!string.IsNullOrEmpty(_namespace))
            {
                key = _namespace + "/" + name;
            }

            return _indexer.GetByKey(key);
        }

        public Lister<TApiType> Namespace(string @namespace)
        {
            return new Lister<TApiType>(_indexer, @namespace, Caches.NamespaceIndex);
        }
    }
}
