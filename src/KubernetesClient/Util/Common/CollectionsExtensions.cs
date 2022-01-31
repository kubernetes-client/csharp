namespace k8s.Util.Common
{
    internal static class CollectionsExtensions
    {
        public static void AddRange<T>(this HashSet<T> hashSet, ICollection<T> items)
        {
            if (items == null || hashSet == null)
            {
                return;
            }

            foreach (var item in items)
            {
                hashSet.Add(item);
            }
        }

        internal static TValue ComputeIfAbsent<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TKey key, Func<TKey, TValue> mappingFunction)
        {
            if (dictionary is null)
            {
                throw new ArgumentNullException(nameof(dictionary));
            }

            if (dictionary.TryGetValue(key, out var value))
            {
                return value;
            }

            if (mappingFunction == null)
            {
                throw new ArgumentNullException(nameof(mappingFunction));
            }

            var newKey = mappingFunction(key);
            dictionary[key] = newKey;
            return newKey;
        }
    }
}
