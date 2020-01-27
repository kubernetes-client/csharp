using System.Collections.Generic;

namespace System.Collections.Generic
{
    public static class CollectionsExtensions
    {
        public static TValue GetOrDefault<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TKey key, TValue @default = default)
        {
            if (dictionary.TryGetValue(key, out var value))
                return value;
            return @default;
        }

        public static void AddRange<T>(this HashSet<T> hashSet, ICollection<T> items)
        {
            foreach (var item in items)
            {
                hashSet.Add(item);
            }
        }

        public static TValue ComputeIfAbsent<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TKey key, Func<TKey, TValue> mappingFunction)
        {
            TValue value;
            if (dictionary.TryGetValue(key, out value))
                return value;
            value = mappingFunction(key);
            dictionary[key] = value;
            return value;
        }
    }
}