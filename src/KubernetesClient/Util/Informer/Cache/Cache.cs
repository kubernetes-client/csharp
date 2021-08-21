using System;
using System.Collections.Generic;
using System.Linq;
using k8s.Models;
using k8s.Util.Common;

namespace k8s.Util.Informer.Cache
{
    /// <summary>
    /// Cache is a C# port of Java's Cache which is a port of k/client-go's ThreadSafeStore. It basically saves and indexes all the entries.
    /// </summary>
    /// <typeparam name="TApiType">The type of K8s object to save</typeparam>
    public class Cache<TApiType> : IIndexer<TApiType>
      where TApiType : class, IKubernetesObject<V1ObjectMeta>
    {
        /// <summary>
        /// keyFunc defines how to map index objects into indices
        /// </summary>
        private Func<IKubernetesObject<V1ObjectMeta>, string> _keyFunc;

        /// <summary>
        /// indexers stores index functions by their names
        /// </summary>
        /// <remarks>The indexer name(string) is a label marking the different ways it can be calculated.
        /// The default label is "namespace". The default func is to look in the object's metadata and combine the
        /// namespace and name values, as namespace/name.
        /// </remarks>
        private readonly Dictionary<string, Func<TApiType, List<string>>> _indexers = new Dictionary<string, Func<TApiType, List<string>>>();

        /// <summary>
        /// indices stores objects' keys by their indices
        /// </summary>
        /// <remarks>Similar to 'indexers', an indice has the same label as its corresponding indexer except it's value
        /// is the result of the func.
        /// if the indexer func is to calculate the namespace and name values as namespace/name, then the indice HashSet
        /// holds those values.
        /// </remarks>
        private Dictionary<string, Dictionary<string, HashSet<string>>> _indices = new Dictionary<string, Dictionary<string, HashSet<string>>>();

        /// <summary>
        /// items stores object instances
        /// </summary>
        /// <remarks>Indices hold the HashSet of calculated keys (namespace/name) for a given resource and items map each of
        /// those keys to actual K8s object that was originally returned.</remarks>
        private Dictionary<string, TApiType> _items = new Dictionary<string, TApiType>();

        /// <summary>
        /// object used to track locking
        /// </summary>
        /// <remarks>methods interacting with the store need to lock to secure the thread for race conditions,
        /// learn more: https://docs.microsoft.com/en-us/dotnet/csharp/language-reference/keywords/lock-statement</remarks>
        private readonly object _lock = new object();

        /// <summary>
        /// Initializes a new instance of the <see cref="Cache{TApiType}"/> class. Uses an object's namespace as the key.
        /// </summary>
        public Cache()
          : this(Caches.NamespaceIndex, Caches.MetaNamespaceIndexFunc, Caches.DeletionHandlingMetaNamespaceKeyFunc)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Cache{TApiType}"/> class.
        /// Constructor.
        /// </summary>
        /// <param name="indexName">the index name, an unique name representing the index</param>
        /// <param name="indexFunc">the index func by which we map multiple object to an index for querying</param>
        /// <param name="keyFunc">the key func by which we map one object to an unique key for storing</param>
        public Cache(string indexName, Func<TApiType, List<string>> indexFunc, Func<IKubernetesObject<V1ObjectMeta>, string> keyFunc)
        {
            _indexers[indexName] = indexFunc;
            _keyFunc = keyFunc;
            _indices[indexName] = new Dictionary<string, HashSet<string>>();
        }

        public void Clear()
        {
            lock (_lock)
            {
                _items?.Clear();
                _indices?.Clear();
                _indexers?.Clear();
            }
        }

        /// <summary>
        /// Add objects.
        /// </summary>
        /// <param name="obj">the obj</param>
        public void Add(TApiType obj)
        {
            var key = _keyFunc(obj);

            lock (_lock)
            {
                var oldObj = _items.GetValueOrDefault(key);
                _items[key] = obj;
                UpdateIndices(oldObj, obj, key);
            }
        }

        /// <summary>
        /// Update the object.
        /// </summary>
        /// <param name="obj">the obj</param>
        public void Update(TApiType obj)
        {
            var key = _keyFunc(obj);

            lock (_lock)
            {
                var oldObj = _items.GetValueOrDefault(key);
                _items[key] = obj;
                UpdateIndices(oldObj, obj, key);
            }
        }

        /// <summary>
        /// Delete the object.
        /// </summary>
        /// <param name="obj">the obj</param>
        public void Delete(TApiType obj)
        {
            var key = _keyFunc(obj);
            lock (_lock)
            {
                if (!_items.TryGetValue(key, out var value))
                {
                    return;
                }

                DeleteFromIndices(value, key);
                _items.Remove(key);
            }
        }

        /// <summary>
        /// Replace the content in the cache completely.
        /// </summary>
        /// <param name="list">the list</param>
        /// <param name="resourceVersion">optional, unused param from interface</param>
        /// <exception cref="ArgumentNullException">list is null</exception>
        public void Replace(IEnumerable<TApiType> list, string resourceVersion = default)
        {
            if (list is null)
            {
                throw new ArgumentNullException(nameof(list));
            }

            var newItems = new Dictionary<string, TApiType>();
            foreach (var item in list)
            {
                var key = _keyFunc(item);
                newItems[key] = item;
            }

            lock (_lock)
            {
                _items = newItems;

                // rebuild any index
                _indices = new Dictionary<string, Dictionary<string, HashSet<string>>>();
                foreach (var (key, value) in _items)
                {
                    UpdateIndices(default, value, key);
                }
            }
        }

        /// <summary>
        /// Resync.
        /// </summary>
        public void Resync()
        {
            // Do nothing by default
        }

        /// <summary>
        /// List keys.
        /// </summary>
        /// <returns>the list</returns>
        public IEnumerable<string> ListKeys()
        {
            return _items.Select(item => item.Key);
        }

        /// <summary>
        /// Get object t.
        /// </summary>
        /// <param name="obj">the obj</param>
        /// <returns>the t</returns>
        public TApiType Get(TApiType obj)
        {
            var key = _keyFunc(obj);

            lock (_lock)
            {
                // Todo: to make this lock striped or reader/writer (or use ConcurrentDictionary)
                return _items.GetValueOrDefault(key);
            }
        }

        /// <summary>
        /// List all objects in the cache.
        /// </summary>
        /// <returns>all items</returns>
        public IEnumerable<TApiType> List()
        {
            lock (_lock)
            {
                return _items.Select(item => item.Value);
            }
        }

        /// <summary>
        /// Get object t.
        /// </summary>
        /// <param name="key">the key</param>
        /// <returns>the get by key</returns>
        public TApiType GetByKey(string key)
        {
            lock (_lock)
            {
                _items.TryGetValue(key, out var value);
                return value;
            }
        }

        /// <summary>
        /// Get objects.
        /// </summary>
        /// <param name="indexName">the index name</param>
        /// <param name="obj">the obj</param>
        /// <returns>the list</returns>
        /// <exception cref="ArgumentException">indexers does not contain the provided index name</exception>
        public IEnumerable<TApiType> Index(string indexName, TApiType obj)
        {
            if (!_indexers.ContainsKey(indexName))
            {
                throw new ArgumentException($"index {indexName} doesn't exist!", nameof(indexName));
            }

            lock (_lock)
            {
                var indexFunc = _indexers[indexName];
                var indexKeys = indexFunc(obj);
                var index = _indices.GetValueOrDefault(indexName);
                if (index is null || index.Count == 0)
                {
                    return new List<TApiType>();
                }

                var returnKeySet = new HashSet<string>();
                foreach (var set in indexKeys.Select(indexKey => index.GetValueOrDefault(indexKey)).Where(set => set != null && set.Count != 0))
                {
                    returnKeySet.AddRange(set);
                }

                var items = new List<TApiType>(returnKeySet.Count);
                items.AddRange(returnKeySet.Select(absoluteKey => _items[absoluteKey]));

                return items;
            }
        }

        /// <summary>
        /// Index keys list.
        /// </summary>
        /// <param name="indexName">the index name</param>
        /// <param name="indexKey">the index key</param>
        /// <returns>the list</returns>
        /// <exception cref="ArgumentException">indexers does not contain the provided index name</exception>
        /// <exception cref="KeyNotFoundException">indices collection does not contain the provided index name</exception>
        public IEnumerable<string> IndexKeys(string indexName, string indexKey)
        {
            if (!_indexers.ContainsKey(indexName))
            {
                throw new ArgumentException($"index {indexName} doesn't exist!", nameof(indexName));
            }

            lock (_lock)
            {
                var index = _indices.GetValueOrDefault(indexName);

                if (index is null)
                {
                    throw new KeyNotFoundException($"no value could be found for name '{indexName}'");
                }

                return index[indexKey];
            }
        }

        /// <summary>
        /// By index list.
        /// </summary>
        /// <param name="indexName">the index name</param>
        /// <param name="indexKey">the index key</param>
        /// <returns>the list</returns>
        /// <exception cref="ArgumentException">indexers does not contain the provided index name</exception>
        /// <exception cref="KeyNotFoundException">indices collection does not contain the provided index name</exception>
        public IEnumerable<TApiType> ByIndex(string indexName, string indexKey)
        {
            if (!_indexers.ContainsKey(indexName))
            {
                throw new ArgumentException($"index {indexName} doesn't exist!", nameof(indexName));
            }

            var index = _indices.GetValueOrDefault(indexName);

            if (index is null)
            {
                throw new KeyNotFoundException($"no value could be found for name '{indexName}'");
            }

            var set = index[indexKey];
            return set is null ? new List<TApiType>() : set.Select(key => _items[key]);
        }

        /// <summary>
        /// Return the indexers registered with the cache.
        /// </summary>
        /// <returns>registered indexers</returns>
        public IDictionary<string, Func<TApiType, List<string>>> GetIndexers() => _indexers;

        /// <summary>
        /// Add additional indexers to the cache.
        /// </summary>
        /// <param name="newIndexers">indexers to add</param>
        /// <exception cref="ArgumentNullException">newIndexers is null</exception>
        /// <exception cref="InvalidOperationException">items collection is not empty</exception>
        /// <exception cref="ArgumentException">conflict between keys in existing index and new indexers provided</exception>
        public void AddIndexers(IDictionary<string, Func<TApiType, List<string>>> newIndexers)
        {
            if (newIndexers is null)
            {
                throw new ArgumentNullException(nameof(newIndexers));
            }

            if (_items.Any())
            {
                throw new InvalidOperationException("cannot add indexers to a non-empty cache");
            }

            var oldKeys = _indexers.Keys;
            var newKeys = newIndexers.Keys;
            var intersection = oldKeys.Intersect(newKeys);

            if (intersection.Any())
            {
                throw new ArgumentException("indexer conflict: " + intersection);
            }

            foreach (var (key, value) in newIndexers)
            {
                AddIndexFunc(key, value);
            }
        }

        /// <summary>
        /// UpdateIndices modifies the objects location in the managed indexes, if this is an update, you
        /// must provide an oldObj.
        /// </summary>
        /// <remarks>UpdateIndices must be called from a function that already has a lock on the cache.</remarks>
        /// <param name="oldObj"> the old obj</param>
        /// <param name="newObj"> the new obj</param>
        /// <param name="key">the key</param>
        private void UpdateIndices(TApiType oldObj, TApiType newObj, string key)
        {
            // if we got an old object, we need to remove it before we can add
            // it again.
            if (oldObj != null)
            {
                DeleteFromIndices(oldObj, key);
            }

            foreach (var (indexName, indexFunc) in _indexers)
            {
                var indexValues = indexFunc(newObj);
                if (indexValues is null || indexValues.Count == 0)
                {
                    continue;
                }

                var index = _indices.ComputeIfAbsent(indexName, _ => new Dictionary<string, HashSet<string>>());

                foreach (var indexValue in indexValues)
                {
                    HashSet<string> indexSet = index.ComputeIfAbsent(indexValue, k => new HashSet<string>());
                    indexSet.Add(key);

                    index[indexValue] = indexSet;
                }
            }
        }

        /// <summary>
        /// DeleteFromIndices removes the object from each of the managed indexes.
        /// </summary>
        /// <remarks>It is intended to be called from a function that already has a lock on the cache.</remarks>
        /// <param name="oldObj">the old obj</param>
        /// <param name="key">the key</param>
        private void DeleteFromIndices(TApiType oldObj, string key)
        {
            foreach (var (s, indexFunc) in _indexers)
            {
                var indexValues = indexFunc(oldObj);
                if (indexValues is null || indexValues.Count == 0)
                {
                    continue;
                }

                var index = _indices.GetValueOrDefault(s);
                if (index is null)
                {
                    continue;
                }

                foreach (var indexSet in indexValues.Select(indexValue => index[indexValue]))
                {
                    indexSet?.Remove(key);
                }
            }
        }

        /// <summary>
        /// Add index func.
        /// </summary>
        /// <param name="indexName">the index name</param>
        /// <param name="indexFunc">the index func</param>
        public void AddIndexFunc(string indexName, Func<TApiType, List<string>> indexFunc)
        {
            _indices[indexName] = new Dictionary<string, HashSet<string>>();
            _indexers[indexName] = indexFunc;
        }

        public Func<IKubernetesObject<V1ObjectMeta>, string> KeyFunc => _keyFunc;

        public void SetKeyFunc(Func<IKubernetesObject<V1ObjectMeta>, string> keyFunc)
        {
            _keyFunc = keyFunc;
        }
    }
}
