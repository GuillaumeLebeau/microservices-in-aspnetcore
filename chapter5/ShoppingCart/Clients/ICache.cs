using System;

namespace ShoppingCart.Clients
{
    public interface ICache
    {
        void Add(string key, object value, TimeSpan ttl);
        T Get<T>(string key) where T: class;
    }
}
