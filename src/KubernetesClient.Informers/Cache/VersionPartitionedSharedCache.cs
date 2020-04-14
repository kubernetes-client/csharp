using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace k8s.Informers.Cache
{
    /// <summary>
    ///     Allows creating cache partitions for objects that have versioning semantics. Each partition will maintain its own view of its tracked objects,
    ///     but any items with same key and version will be shared across multiple cache partitions.
    /// </summary>
    /// <remarks>
    ///     The semantics of this class allows for object reuse between informers without compromising each informers ownership of its own cache. Primarily the issue
    ///     it solves is if multiple informers are created with different options, but the data they receive may overlap
    ///     (ex. overlapping labels, or informer scoped to namespace and another scoped globally). Since the master informer (actual connection to physical server) will receive
    ///     same notification over separate channels, we run the risk of informer desynchronization if they share the same cache. However, if each informer maintains it's own
    ///     cache, we may get multiple duplicate objects in memory. This allows any objects that share the same key/version to point to the same reference, while maintaining
    ///     integrity of each cache (dictionary). Note that unlike a regular dictionary, this does not allow updates to same key/version
    /// </remarks>
    /// <typeparam name="TKey">The type of the key uniquely identifying object</typeparam>
    /// <typeparam name="TResource">The type of resource</typeparam>
    /// <typeparam name="TVersion">The type of version associated with object</typeparam>
    public class VersionPartitionedSharedCache<TKey, TResource, TVersion>
    {
        private readonly Func<TResource, TKey> _keySelector;
        private readonly object _lock = new object();
        private readonly Func<TResource, TVersion> _versionSelector;

        private readonly HashSet<CacheView> _views = new HashSet<CacheView>();

        // internal to allow for unit testing
        internal readonly Dictionary<VersionResourceKey, TResource> Items = new Dictionary<VersionResourceKey, TResource>();

        public VersionPartitionedSharedCache(Func<TResource, TKey> keySelector, Func<TResource, TVersion> versionSelector)
        {
            _keySelector = keySelector;
            _versionSelector = versionSelector;
        }

        /// <summary>
        ///     Creates a unique cache partition that may share references to objects with same key/versions with other partitions
        /// </summary>
        /// <returns>Partitioned cache</returns>
        public ICache<TKey, TResource> CreatePartition()
        {
            lock (_lock)
            {
                var view = new CacheView(this);
                _views.Add(view);
                return view;
            }
        }


        private void Remove(TResource resource, CacheView originView)
        {
            var versionedKey = GetVersionKeyFor(resource);
            Remove(versionedKey, originView);
        }

        private void Remove(VersionResourceKey versionedKey, CacheView originView)
        {
            var otherViewsTrackingResource = _views
                .Except(new[] { originView })
                .Any(x => x.TryGetValue(versionedKey.Key, out var resource) && _versionSelector(resource).Equals(versionedKey.Version));
            if (!otherViewsTrackingResource)
            {
                Items.Remove(versionedKey);
            }
        }

        private TResource GetOrAdd(TResource resource)
        {
            var key = GetVersionKeyFor(resource);
            if (Items.TryGetValue(key, out var existingResource))
            {
                return existingResource;
            }
            Items.Add(key, resource);
            return resource;
        }

        private VersionResourceKey GetVersionKeyFor(TResource resource)
        {
            return new VersionResourceKey { Key = _keySelector(resource), Version = _versionSelector(resource) };
        }

        internal struct VersionResourceKey
        {
            public TKey Key;
            public TVersion Version;
        }

        private class CacheView : ICache<TKey, TResource>
        {
            private readonly Dictionary<TKey, TResource> _items = new Dictionary<TKey, TResource>();
            private readonly VersionPartitionedSharedCache<TKey, TResource, TVersion> _parent;

            public CacheView(VersionPartitionedSharedCache<TKey, TResource, TVersion> parent)
            {
                _parent = parent;
            }

            public long Version { get; set; } // = 1;

            public void Reset(IDictionary<TKey, TResource> newValues)
            {
                lock (_parent._lock)
                {
                    _items.Clear();
                    foreach (var item in newValues)
                    {
                        _items.Add(item.Key, item.Value);
                    }
                }
            }

            public ICacheSnapshot<TKey, TResource> Snapshot()
            {
                lock (_parent._lock)
                {
                    return new SimpleCache<TKey, TResource>.SimpleCacheSnapshot(this, Version);
                }
            }

            public IEnumerator<KeyValuePair<TKey, TResource>> GetEnumerator()
            {
                lock (_parent._lock)
                {
                    return _items.ToList().GetEnumerator();
                }
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                lock (_parent._lock)
                {
                    return _items.ToList().GetEnumerator();
                }
            }

            public void Add(KeyValuePair<TKey, TResource> item)
            {
                lock (_parent._lock)
                {
                    AssertMatchingKeys(item.Key, item.Value);
                    var cacheItem = _parent.GetOrAdd(item.Value);
                    _items.Add(_parent._keySelector(cacheItem), cacheItem);
                }
            }

            public void Clear()
            {
                lock (_parent._lock)
                {
                    foreach (var item in _items)
                    {
                        _parent.Remove(item.Value, this);
                    }

                    _items.Clear();
                }
            }

            public bool Contains(KeyValuePair<TKey, TResource> item)
            {
                lock (_parent._lock)
                {
                    return _items.Contains(item);
                }
            }

            public void CopyTo(KeyValuePair<TKey, TResource>[] array, int arrayIndex)
            {
                lock (_parent._lock)
                {
                    ((IDictionary<TKey, TResource>)_items).CopyTo(array, arrayIndex);
                }
            }

            public bool Remove(KeyValuePair<TKey, TResource> item)
            {
                lock (_parent._lock)
                {
                    _parent.Remove(item.Value, this);
                    return _items.Remove(item.Key);
                }
            }

            public int Count
            {
                get
                {
                    lock (_parent._lock)
                    {
                        return _items.Count;
                    }
                }
            }

            public bool IsReadOnly => false;

            public void Add(TKey key, TResource value)
            {
                lock (_parent._lock)
                {
                    AssertMatchingKeys(key, value);
                    value = _parent.GetOrAdd(value);
                    _items.Add(key, value);
                }
            }

            public bool ContainsKey(TKey key)
            {
                lock (_parent._lock)
                {
                    return _items.ContainsKey(key);
                }
            }

            public bool Remove(TKey key)
            {
                lock (_parent._lock)
                {
                    if (!_items.Remove(key, out var existing))
                    {
                        return false;
                    }
                    _parent.Remove(existing, this);
                    return true;
                }
            }

            public bool TryGetValue(TKey key, out TResource value)
            {
                lock (_parent._lock)
                {
                    return _items.TryGetValue(key, out value);
                }
            }

            public TResource this[TKey key]
            {
                get
                {
                    lock (_parent._lock)
                    {
                        return _items[key];
                    }
                }
                set
                {
                    // the semantics of set here are tricky because if the value already exists, it will reuse existing
                    // this means that consumers should not make assumption that the object that was passed as value to set
                    // is the one that got added to collection, and should always do a "get" operation if they plan on modifying it
                    lock (_parent._lock)
                    {
                        AssertMatchingKeys(key, value);
                        var existing = _parent.GetOrAdd(value);
                        _items[key] = existing;
                    }
                }
            }

            public ICollection<TKey> Keys
            {
                get
                {
                    lock (_parent._lock)
                    {
                        return _items.Keys.ToList();
                    }
                }
            }

            public ICollection<TResource> Values
            {
                get
                {
                    lock (_parent._lock)
                    {
                        return _items.Values.ToList();
                    }
                }
            }

            public void Dispose()
            {
                lock (_parent._lock)
                {
                    _parent._views.Remove(this);
                }
            }

            private void AssertMatchingKeys(TKey key, TResource resource)
            {
                if (!key.Equals(_parent._keySelector(resource)))
                {
                    throw new InvalidOperationException("The value of the key specified is not the same as the one inside the resource");
                }
            }
        }
    }
}
