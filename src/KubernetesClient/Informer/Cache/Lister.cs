using System;
using System.Collections.Generic;
using System.Linq;
using Org.BouncyCastle.Utilities;

namespace k8s.Informer.Cache
{
    public class Lister<TApiType>
    {

        private string _namespace;

        private string _indexName;

        private IIndexer<TApiType> _indexer;

        public Lister(IIndexer<TApiType> indexer) : this(indexer, null, Caches.NamespaceIndex)
        {
        }

        public Lister(IIndexer<TApiType> indexer, string @namespace) : this(indexer, @namespace, Caches.NamespaceIndex)
        {
        }
        public Lister(IIndexer<TApiType> indexer, string @namespace, string indexName)
        {
            _indexer = indexer;
            _namespace = @namespace;
            _indexName = indexName;
        }

        public List<TApiType> List()
        {
            if (_namespace?.Length == 0)
            {
                return _indexer.ToList();
            }

            return _indexer.ByIndex(_indexName, _namespace);
        }

        public TApiType Get(string name)
        {
            string key = name;
            if (_namespace?.Length != 0)
            {
                key = _namespace + "/" + name;
            }

            return _indexer[key];
        }

        public Lister<TApiType> Namespace(string @namespace)
        {
            return new Lister<TApiType>(_indexer, @namespace, Caches.NamespaceIndex);
        }
        
    }
}