namespace k8s.Informers.Cache
{
    public struct CacheSynchronized<T>
    {
        public CacheSynchronized(long messageNumber, long cacheVersion, T value)
        {
            MessageNumber = messageNumber;
            CacheVersion = cacheVersion;
            Value = value;
        }

        /// <summary>
        ///     Message number in the sequencer
        /// </summary>
        public long MessageNumber { get; }

        /// <summary>
        ///     The version of cache this message was included in
        /// </summary>
        public long CacheVersion { get; }

        public T Value { get; }

        public override string ToString()
        {
            return $"MessageNumber: {MessageNumber}, IncludedInCache: {CacheVersion}: {Value}";
        }
    }
}
