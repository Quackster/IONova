using System.Collections.Concurrent;
using System.Collections.Generic;

namespace Ion.Specialized.Utilities.Extensions
{
    public static class ConcurrentDictionaryExtensions
    {
        /// <summary>
        /// Remove from a list, return if successful
        /// </summary>
        public static bool Remove<K, V>(this ConcurrentDictionary<K, V> dictionary, K key)
        {
            dictionary.Remove(key, out V value);
            return value != null;
        }
    }
}
