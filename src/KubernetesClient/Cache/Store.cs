using System;
using k8s.Models;
using System.Collections.Generic;
using System.Reflection;

namespace k8s.cache
{
    public class Store : IStore
    {
        private IThreadSafeStore _threadSafeStorage;
        private Func<IKubernetesObject, string> _metaNamespaceFunc;
        public string MetaNamespaceKeyFunc(IKubernetesObject o)
        {
            try
            {
                var typedObject = (V1ObjectMeta)o.GetType().GetProperty("Metadata").GetValue(o, null);
                if (typedObject.NamespaceProperty == "")
                {
                    return typedObject.Name;
                }
                return typedObject.NamespaceProperty + "/" + typedObject.Name;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return "";
            }
        }

        public Store()
        {
            _threadSafeStorage = new ThreadSafeStore();
            _metaNamespaceFunc = MetaNamespaceKeyFunc;
        }
        public void Add(IKubernetesObject o)
        {
            var key = _metaNamespaceFunc(o);
            _threadSafeStorage.Add(key, o);
        }

        public Tuple<IKubernetesObject, bool> Get(IKubernetesObject o)
        {
            var key = _metaNamespaceFunc(o);
            var val = _threadSafeStorage.Get(key);
            if (val == null)
            {
                return new Tuple<IKubernetesObject, bool>(null, false);
            }
            return new Tuple<IKubernetesObject, bool>(val, true);
        }

        public Tuple<IKubernetesObject, bool> GetByKey(string key)
        {
            var val = _threadSafeStorage.Get(key);
            if (val == null)
            {
                return new Tuple<IKubernetesObject, bool>(null, false);
            }
            return new Tuple<IKubernetesObject, bool>(val, true);
        }

        public IReadOnlyList<IKubernetesObject> List()
        {
            return _threadSafeStorage.List();
        }

        public IReadOnlyList<String> ListKeys()
        {
            return _threadSafeStorage.ListKeys();
        }

        public void Replace(IKubernetesObject[] objects)
        {
            throw new NotImplementedException();
        }

        public void Resync()
        {
            throw new NotImplementedException();
        }

        public IKubernetesObject Update(IKubernetesObject o)
        {
            var key = _metaNamespaceFunc(o);
            return _threadSafeStorage.Update(key, o);
        }

        public void Delete(IKubernetesObject o)
        {
            var key = _metaNamespaceFunc(o);
            _threadSafeStorage.Delete(key);
        }
    }
}

