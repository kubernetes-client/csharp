using System;
using System.Threading;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

namespace k8s.cache
{
    public class ThreadSafeStore : IThreadSafeStore
    {
        private ConcurrentDictionary<string, IKubernetesObject> _store;

        public ThreadSafeStore()
        {
            _store = new ConcurrentDictionary<string, IKubernetesObject>();
        }

        public void Add(string key, IKubernetesObject value)
        {
            if (!_store.TryAdd(key, value))
            {
                throw new Exception(String.Format("Could not add value for key: {0}", key));
            }
        }

        public void Delete(string key)
        {
            IKubernetesObject oldVal;
            if (!_store.TryRemove(key, out oldVal))
            {
                // Could not remove the value. Raise an exception.
                throw new Exception(String.Format("Could not remove value with key: {0}", key));
            }
        }

        public IKubernetesObject Get(string key)
        {
            IKubernetesObject v;
            v = _store.TryGetValue(key, out v) ? v : null;
            return v;
        }

        public IReadOnlyList<IKubernetesObject> List()
        {
            return _store.Values.ToList().AsReadOnly();
        }

        public IReadOnlyList<string> ListKeys()
        {
            return _store.Keys.ToList().AsReadOnly();
        }

        public void Resync()
        {
            throw new NotImplementedException();
        }

        public IKubernetesObject Update(string key, IKubernetesObject value)
        {
            IKubernetesObject oldVal = null;
            _store.AddOrUpdate(key, value, (curKey, existingVal) =>
            {
                oldVal = existingVal;
                return value;
            });
            return oldVal;
        }
    }
}