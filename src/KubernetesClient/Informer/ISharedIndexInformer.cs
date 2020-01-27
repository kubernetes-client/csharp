using System;
using System.Collections.Generic;
using k8s.Informer.Cache;

namespace k8s.Informer
{
    public interface ISharedIndexInformer<ApiType> : ISharedInformer<ApiType>
    {
        void AddIndexers(Dictionary<string, Func<ApiType, List<string>>> indexers);

        IIndexer<ApiType> Indexer { get; }
    }
}