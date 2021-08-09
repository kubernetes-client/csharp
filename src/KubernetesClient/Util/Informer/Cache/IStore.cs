using System.Collections.Generic;
using k8s.Models;

namespace k8s.Util.Informer.Cache
{
    public interface IStore<TApiType>
      where TApiType : class, IKubernetesObject<V1ObjectMeta>
    {
        /// <summary>
        /// add inserts an item into the store.
        /// </summary>
        /// <param name="obj">specific obj</param>
        void Add(TApiType obj);

        /// <summary>
        /// update sets an item in the store to its updated state.
        /// </summary>
        /// <param name="obj">specific obj</param>
        void Update(TApiType obj);

        /// <summary>
        /// delete removes an item from the store.
        /// </summary>
        /// <param name="obj">specific obj</param>
        void Delete(TApiType obj);

        /// <summary>
        /// Replace will delete the contents of 'c', using instead the given list.
        /// </summary>
        /// <param name="list">list of objects</param>
        /// <param name="resourceVersion">specific resource version</param>
        void Replace(IEnumerable<TApiType> list, string resourceVersion);

        /// <summary>
        /// resync will send a resync event for each item.
        /// </summary>
        void Resync();

        /// <summary>
        /// listKeys returns a list of all the keys of the object currently in the store.
        /// </summary>
        /// <returns>list of all keys</returns>
        IEnumerable<string> ListKeys();

        /// <summary>
        /// get returns the requested item.
        /// </summary>
        /// <param name="obj">specific obj</param>
        /// <returns>the requested item if exist</returns>
        TApiType Get(TApiType obj);

        /// <summary>
        /// getByKey returns the request item with specific key.
        /// </summary>
        /// <param name="key">specific key</param>
        /// <returns>the request item</returns>
        TApiType GetByKey(string key);

        /// <summary>
        /// list returns a list of all the items.
        /// </summary>
        /// <returns>list of all the items</returns>
        IEnumerable<TApiType> List();
    }
}
