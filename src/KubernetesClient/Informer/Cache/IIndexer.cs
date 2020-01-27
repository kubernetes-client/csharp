using System;
using System.Collections.Generic;

namespace k8s.Informer.Cache
{
    public interface IIndexer<ApiType> : IStore<ApiType>
    {
        List<ApiType> Index(string indexName, ApiType obj);
        List<string> IndexKeys(string indexName, string indexKey);
        List<ApiType> ByIndex(string indexName, string indexKey);
        IDictionary<string, Func<ApiType, List<string>>> GetIndexers();
        void AddIndexers(IDictionary<string, Func<ApiType, List<string>>> indexers);
    }
}