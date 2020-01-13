using System;
using System.Threading;
using System.Collections.Generic;
using System.Linq;

namespace k8s.cache
{
    public class ThreadSafeStore : IThreadSafeStore
    {
        private static Mutex _storageLock;
        private Dictionary<string, IKubernetesObject> _store;

        public ThreadSafeStore() {
            _store = new Dictionary<string, IKubernetesObject>();
            _storageLock = new Mutex();
        }
        public void Add(string key, IKubernetesObject value)
        {
            _storageLock.WaitOne();           
            try {                
                if (_store.ContainsKey(key))
                {
                    _store[key] = value;
                } else {
                    _store.Add(key, value);            
                }
            } catch (Exception ex) {
                Console.WriteLine(ex);    
            }           
            _storageLock.ReleaseMutex();            
        }

        public void Delete(string key)
        {
            _storageLock.WaitOne();            
            _store.Remove(key);            
            _storageLock.ReleaseMutex();            
        }

        public IKubernetesObject Get(string key)
        {
            _storageLock.WaitOne();
            IKubernetesObject v;
            v = _store.TryGetValue(key,out v)? v : null;           
            _storageLock.ReleaseMutex(); 
            return v;            
        }

        public List<IKubernetesObject> List()
        {
            _storageLock.WaitOne();
            var list = _store.Values.ToList();
            _storageLock.ReleaseMutex(); 
            return list;            
        }

        public List<string> ListKeys()
        {
            _storageLock.WaitOne();
            var list = _store.Keys.ToList();
            _storageLock.ReleaseMutex(); 
            return list;            
        }

        public void Resync()
        {
            throw new NotImplementedException();
        }

        public IKubernetesObject Update(string key, IKubernetesObject value)
        {            
            IKubernetesObject retVal = null;
            _storageLock.WaitOne();              
            if (_store.ContainsKey(key))
            {
                retVal = _store[key];
                _store[key] = value;
            } else {
                // Note: If we don't find it, we just add it.
                // TODO - evaulate if we need to raise an exception for this
                _store.Add(key, value);
            }            
            _storageLock.ReleaseMutex();     
            return retVal;
        }
    }
}

