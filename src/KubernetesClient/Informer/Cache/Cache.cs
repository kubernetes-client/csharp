using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace k8s.Informer.Cache
{
    public class Cache<ApiType> : IIndexer<ApiType> where ApiType : class
    {
        public Func<ApiType, string> KeyFunc { get; set; }
        private Dictionary<String, Func<ApiType, List<String>>> _indexers = new Dictionary<string,Func<ApiType,List<string>>>();
        private Dictionary<String, ApiType> _items = new Dictionary<string,ApiType>();
        private Dictionary<String, Dictionary<String, HashSet<String>>> _indices = new Dictionary<string, Dictionary<string, HashSet<string>>>();
        private object _lock = new object();

        public Cache() : this(Caches.NamespaceIndex, Caches.MetaNamespaceIndexFunc, Caches.DeletionHandlingMetaNamespaceKeyFunc<ApiType>)
        {
        }
        public Cache(string indexName, Func<ApiType, List<string>> indexFunc, Func<ApiType, string> keyFunc)
        {
            _indexers[indexName] = indexFunc;
            KeyFunc = keyFunc;
            _indices[indexName] = new Dictionary<string, HashSet<string>>();
        }

        public void Add(ApiType obj)
        {
            var key = KeyFunc(obj);
            lock (_lock)
            {
                var oldObj = _items.GetOrDefault(key);
                _items[key] = obj;
                UpdateIndices(oldObj, obj, key);
            }
        }

        public void Update(ApiType obj)
        {
            var key = KeyFunc(obj);
            lock (_lock)
            {
                var oldObj = _items.GetOrDefault(key);
                _items[key] = obj;
                UpdateIndices(oldObj, obj, key);
            }
        }

        public void Delete(ApiType obj)
        {
            var key = KeyFunc(obj);
            lock (_lock)
            {
                
                var exists = _items.ContainsKey(key);
                if (_items.TryGetValue(key, out var value)) 
                {
                    DeleteFromIndices(value, key);
                    _items.Remove(key);
                }
            }
        }

        public void Replace(List<ApiType> list, string resourceVersion)
        {
            lock (_lock)
            {
                var newItems = new Dictionary<string, ApiType>();
                foreach (var item in list) 
                {
                    var key = KeyFunc(item);
                    newItems[key] = item;
                }
                _items = newItems;

                // rebuild any index
                _indices = new Dictionary<string, Dictionary<string, HashSet<string>>>();
                foreach (var itemEntry in _items) 
                {
                    UpdateIndices(default, itemEntry.Value, itemEntry.Key);
                }
            }
        }

        public void Resync()
        {
            // Do nothing by default
        }

        public List<string> Keys
        {
            get
            {
                lock (_lock)
                {
                    return _items.Select(x => x.Key).ToList();
                }
            }
        }

        public ApiType this[ApiType obj]
        {
            get
            {
                var key = KeyFunc(obj);
                lock (_lock)
                {
                    return this[key];
                }
            }
        }

        public ApiType this[string key] 
        {
            get
            {
                lock (_lock)
                {
                    return _items[key];
                }
            }
        }

        public List<ApiType> Index(string indexName, ApiType obj)
        {
            lock (_lock)
            {
                if (!_indexers.ContainsKey(indexName)) 
                {
                    throw new ArgumentException($"index {indexName} doesn't exist!", nameof(indexName));
                }
                var indexFunc = _indexers[indexName];
                var indexKeys = indexFunc(obj);
                var index = _indices.GetOrDefault(indexName);
                if (index.Count == 0) 
                {
                    return new List<ApiType>();
                }
                var returnKeySet = new HashSet<string>();
                foreach (var indexKey in indexKeys) 
                {
                    var set = index.GetOrDefault(indexKey);
                    if (set?.Count == 0) {
                        continue;
                    }
                    returnKeySet.AddRange(set);
                }
                
                var items = new List<ApiType>(returnKeySet.Count);
                foreach (var absoluteKey in returnKeySet) {
                    items.Add(_items[absoluteKey]);
                }
                return items;
            }
        }

        public List<string> IndexKeys(string indexName, string indexKey)
        {
            lock (_lock)
            {
                if (!_indexers.ContainsKey(indexName)) 
                {
                    throw new ArgumentException($"index {indexName} doesn't exist!", nameof(indexName));
                }

                var index = _indices.GetOrDefault(indexName);
                var set = index.GetOrDefault(indexKey);
                return set.ToList();
            }
        }

        public List<ApiType> ByIndex(string indexName, string indexKey)
        {
            if (!_indexers.ContainsKey(indexName)) 
            {
                throw new ArgumentException($"index {indexName} doesn't exist!", nameof(indexName));
            }
            var index = _indices.GetOrDefault(indexName);
            var set = index.GetOrDefault(indexKey);
            if (set == null) {
                return new List<ApiType>();
            }
            var items = new List<ApiType>(set.Count);
            foreach (var key in set) {
                items.Add(_items[key]);
            }
            return items;
        }

        public IDictionary<string, Func<ApiType, List<string>>> GetIndexers() => _indexers;

        public void AddIndexers(IDictionary<string, Func<ApiType, List<string>>> newIndexers)
        {
            if (_items.Any()) 
            {
                
                throw new InvalidOperationException("cannot add indexers to a non-empty cache");
            }
            var oldKeys = _indexers.Keys;
            var newKeys = newIndexers.Keys;
            var intersection = oldKeys.Intersect(newKeys);
            
            if (intersection.Any()) {
                throw new ArgumentException("indexer conflict: " + intersection);
            }
            foreach (var indexEntry in newIndexers) 
            {
                AddIndexFunc(indexEntry.Key, indexEntry.Value);
            }
        }

        public void UpdateIndices(ApiType oldObj, ApiType newObj, string key)
        {
            if (oldObj != null) 
            {
                DeleteFromIndices(oldObj, key);
            }
            foreach (var indexEntry in _indexers) 
            {
                var indexName = indexEntry.Key;
                var indexFunc = indexEntry.Value;
                var indexValues = indexFunc(newObj);
                if (indexValues?.Count == 0) {
                    continue;
                }

                var index = _indices.ComputeIfAbsent(indexName, _ => new Dictionary<String, HashSet<String>>());
                foreach (var indexValue in indexValues) 
                {
                    var indexSet = index.ComputeIfAbsent(indexValue, _ => new HashSet<string>());
                    indexSet.Add(key);
                }
            }
        }

        public void DeleteFromIndices(ApiType oldObj, string key)
        {
            foreach (var indexEntry in _indexers) 
            {
                var indexFunc = indexEntry.Value;
                var indexValues = indexFunc(oldObj);
                if (indexValues?.Count == 0) 
                {
                    continue;
                }

                var index = _indices.GetOrDefault(indexEntry.Key);
                if (index == null) 
                {
                    continue;
                }
                foreach (var indexValue in indexValues) 
                {
                    var indexSet = index[indexValue];
                    if (indexSet != null) 
                    {
                        indexSet.Remove(key);
                    }
                }
            }
        }
        public void AddIndexFunc(String indexName, Func<ApiType, List<String>> indexFunc) 
        {
            _indices[indexName] = new Dictionary<string, HashSet<string>>();
            _indexers[indexName] = indexFunc;
        }

        public IEnumerator<ApiType> GetEnumerator()
        {
            List<ApiType> list;
            lock (_lock)
            {
                list = _items.Select(x => x.Value).ToList();
            }

            return list.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}