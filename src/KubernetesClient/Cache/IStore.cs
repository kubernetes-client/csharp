using System;
using System.Collections.Generic;

namespace k8s.cache
{
    public interface IStore
    {
        void Add(IKubernetesObject o);
        void Delete(IKubernetesObject o);
        IKubernetesObject Update(IKubernetesObject o);
        IReadOnlyList<IKubernetesObject> List();
        IReadOnlyList<String> ListKeys();
        Tuple<IKubernetesObject, bool> Get(IKubernetesObject o);
        Tuple<IKubernetesObject, bool> GetByKey(string key);
        void Replace(IKubernetesObject[] objects);
        void Resync();
    }
}

