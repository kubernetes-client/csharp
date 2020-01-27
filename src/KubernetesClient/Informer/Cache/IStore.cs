using System.Collections.Generic;

namespace k8s.Informer.Cache
{
    public interface IStore<TRead,TWrite> :  IEnumerable<TRead>
    {
        List<string> Keys { get; }
        TRead this[TWrite obj] { get; }
        TRead this[string key] { get; }
        void Add(TWrite obj);
        void Update(TWrite obj);
        void Delete(TWrite obj);
        void Replace(List<TWrite> obj, string resourceVersion);
        void Resync();
    }

    public interface IStore<TApi> : IStore<TApi, TApi>
    {
        
    }
    
}