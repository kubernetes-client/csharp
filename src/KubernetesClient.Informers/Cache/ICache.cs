using System;
using System.Collections.Generic;

namespace k8s.Informers.Cache
{
    /// <summary>
    ///     Maintains cache of objects of type <typeparamref name="TResource" />.
    /// </summary>
    /// <typeparam name="TKey">The type of key</typeparam>
    /// <typeparam name="TResource">The type of resource</typeparam>
    public interface ICache<TKey, TResource> : IDictionary<TKey, TResource>, IDisposable
    {
        /// <summary>
        ///     Current version of cache
        /// </summary>
        long Version { get; set; }

        /// <summary>
        ///     Replace all values in cache with new values
        /// </summary>
        /// <param name="newValues"></param>
        void Reset(IDictionary<TKey, TResource> newValues);

        /// Takes a snapshot of the current cache that is version locked
        /// </summary>
        /// <returns>Copy of current cache locked to the version at the time cache is snapshot is taken</returns>
        ICacheSnapshot<TKey, TResource> Snapshot();
    }

    // A readonly snapshot of cache at a point in time
    public interface ICacheSnapshot<TKey, TResource> : IReadOnlyDictionary<TKey, TResource>
    {
        /// <summary>
        ///     Current version of cache
        /// </summary>
        long Version { get; }
    }
}
