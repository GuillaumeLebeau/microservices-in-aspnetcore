using System;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace ShoppingCart.Clients
{
    public class Cache : ICache
    {
        private static readonly IDictionary<string, Tuple<DateTimeOffset, object>> cache =
            new ConcurrentDictionary<string, Tuple<DateTimeOffset, object>>();

        public void Add(string key, object value, TimeSpan ttl)
        {
            cache[key] = Tuple.Create(DateTimeOffset.UtcNow.Add(ttl), value);
        }

        public T Get<T>(string key)
            where T : class
        {
            if (cache.TryGetValue(key, out Tuple<DateTimeOffset, object> value) && value.Item1 > DateTimeOffset.UtcNow)
                return (T) value.Item2;

            cache.Remove(key);
            return null;
        }
    }
}