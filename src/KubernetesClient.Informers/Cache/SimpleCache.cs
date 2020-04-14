using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace k8s.Informers.Cache
{
    public class SimpleCache<TKey, TResource> : ICache<TKey, TResource>
    {
        private readonly IDictionary<TKey, TResource> _items;
        private readonly object _syncRoot = new object();

        public SimpleCache()
        {
            _items = new Dictionary<TKey, TResource>();
        }

        public SimpleCache(IDictionary<TKey, TResource> items, long version)
        {
            Version = version;
            _items = new Dictionary<TKey, TResource>(items);
        }

        public void Reset(IDictionary<TKey, TResource> newValues)
        {
            lock (_syncRoot)
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
            lock (_syncRoot)
            {
                return new SimpleCacheSnapshot(this, Version);
            }
        }

        public IEnumerator<KeyValuePair<TKey, TResource>> GetEnumerator()
        {
            lock (_syncRoot)
            {
                return _items.ToList().GetEnumerator();
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            lock (_syncRoot)
            {
                return _items.ToList().GetEnumerator();
            }
        }

        public void Add(KeyValuePair<TKey, TResource> item)
        {
            lock (_syncRoot)
            {
                _items.Add(item);
            }
        }

        public void Clear()
        {
            lock (_syncRoot)
            {
                _items.Clear();
            }
        }

        public bool Contains(KeyValuePair<TKey, TResource> item)
        {
            lock (_syncRoot)
            {
                return _items.Contains(item);
            }
        }

        public void CopyTo(KeyValuePair<TKey, TResource>[] array, int arrayIndex)
        {
            lock (_syncRoot)
            {
                _items.CopyTo(array, arrayIndex);
            }
        }

        public bool Remove(KeyValuePair<TKey, TResource> item)
        {
            lock (_syncRoot)
            {
                return _items.Remove(item.Key);
            }
        }

        public int Count
        {
            get
            {
                lock (_syncRoot)
                {
                    return _items.Count;
                }
            }
        }

        public bool IsReadOnly => false;

        public void Add(TKey key, TResource value)
        {
            lock (_syncRoot)
            {
                _items.Add(key, value);
            }
        }

        public bool ContainsKey(TKey key)
        {
            lock (_syncRoot)
            {
                return _items.ContainsKey(key);
            }
        }

        public bool Remove(TKey key)
        {
            lock (_syncRoot)
            {
                if (!_items.Remove(key, out var existing))
                {
                    return false;
                }
                return true;
            }
        }

        public bool TryGetValue(TKey key, out TResource value)
        {
            lock (_syncRoot)
            {
                return _items.TryGetValue(key, out value);
            }
        }

        public TResource this[TKey key]
        {
            get
            {
                lock (_syncRoot)
                {
                    return _items[key];
                }
            }
            set
            {
                lock (_syncRoot)
                {
                    _items[key] = value;
                }
            }
        }

        public ICollection<TKey> Keys
        {
            get
            {
                lock (_syncRoot)
                {
                    return _items.Keys.ToList();
                }
            }
        }

        public ICollection<TResource> Values
        {
            get
            {
                lock (_syncRoot)
                {
                    return _items.Values.ToList();
                }
            }
        }

        public void Dispose()
        {
        }

        public long Version { get; set; }

        internal sealed class SimpleCacheSnapshot : ICacheSnapshot<TKey, TResource>
        {
            private readonly Dictionary<TKey, TResource> _items;
            public SimpleCacheSnapshot(IDictionary<TKey, TResource> cache, long version)
            {
                _items = new Dictionary<TKey, TResource>(cache);
                Version = version;
            }
            public IEnumerator<KeyValuePair<TKey, TResource>> GetEnumerator() => _items.GetEnumerator();
            IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
            public int Count => _items.Count;
            public bool ContainsKey(TKey key) => _items.ContainsKey(key);
            public bool TryGetValue(TKey key, out TResource value) => _items.TryGetValue(key, out value);
            public TResource this[TKey key] => _items[key];
            public IEnumerable<TKey> Keys => _items.Keys;
            public IEnumerable<TResource> Values => _items.Values;
            public long Version { get; }
        }
    }
}
