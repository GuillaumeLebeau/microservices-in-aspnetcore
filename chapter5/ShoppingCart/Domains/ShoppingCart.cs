using System.Collections.Generic;
using System.Linq;

using ShoppingCart.Stores;

namespace ShoppingCart.Domains
{
    public class ShoppingCart
    {
        private readonly HashSet<ShoppingCartItem> _items = new HashSet<ShoppingCartItem>();

        public ShoppingCart(int id, long userId)
        {
            Id = id;
            UserId = userId;
        }

        public ShoppingCart(int id, long userId, IEnumerable<ShoppingCartItem> items)
        {
            Id = id;
            UserId = userId;

            foreach (var item in items)
                _items.Add(item);
        }

        public int Id { get; set; }

        public long UserId { get; }

        public IEnumerable<ShoppingCartItem> Items => _items;

        public void AddItems(IEnumerable<ShoppingCartItem> shoppingCartItems, IEventStore eventStore)
        {
            foreach (var item in shoppingCartItems)
            {
                // Raises an event through the eventStore for each item added
                if (_items.Add(item))
                    eventStore.Raise("ShoppingCartItemAdded", new { UserId, item });
            }
        }

        public void RemoveItems(int[] productCatalogIds, IEventStore eventStore)
        {
            foreach (var item in _items.Where(i => productCatalogIds.Contains(i.ProductCatalogId)).ToList())
            {
                // Raises an event through the eventStore for each item removed
                if (_items.Remove(item))
                    eventStore.Raise("ShoppingCartItemRemoved", new { UserId, item });
            }
        }
    }
}
