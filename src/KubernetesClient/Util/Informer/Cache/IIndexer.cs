using k8s.Models;

namespace k8s.Util.Informer.Cache
{
    public interface IIndexer<TApiType> : IStore<TApiType>
      where TApiType : class, IKubernetesObject<V1ObjectMeta>
    {
        /// <summary>
        /// Retrieve list of objects that match on the named indexing function.
        /// </summary>
        /// <param name="indexName">specific indexing function</param>
        /// <param name="obj"> . </param>
        /// <returns>matched objects</returns>
        IEnumerable<TApiType> Index(string indexName, TApiType obj);

        /// <summary>
        /// IndexKeys returns the set of keys that match on the named indexing function.
        /// </summary>
        /// <param name="indexName">specific indexing function</param>
        /// <param name="indexKey">specific index key</param>
        /// <returns>matched keys</returns>
        IEnumerable<string> IndexKeys(string indexName, string indexKey);

        /// <summary>
        /// ByIndex lists object that match on the named indexing function with the exact key.
        /// </summary>
        /// <param name="indexName">specific indexing function</param>
        /// <param name="indexKey">specific index key</param>
        /// <returns>matched objects</returns>
        IEnumerable<TApiType> ByIndex(string indexName, string indexKey);

        /// <summary>
        /// Return the indexers registered with the store.
        /// </summary>
        /// <returns>registered indexers</returns>
        IDictionary<string, Func<TApiType, List<string>>> GetIndexers();

        /// <summary>
        /// Add additional indexers to the store.
        /// </summary>
        /// <param name="indexers">indexers to add</param>
        void AddIndexers(IDictionary<string, Func<TApiType, List<string>>> indexers);
    }
}
