using System.Collections.Generic;
using System.Linq;
using ShoppingCart.Store;

namespace ShoppingCart.Domains
{
    public class ShoppingCart
    {
        private readonly HashSet<ShoppingCartItem> _items = new HashSet<ShoppingCartItem>();

        public ShoppingCart(int userId)
        {
            UserId = userId;
        }

        public int UserId { get; }

        public IEnumerable<ShoppingCartItem> Items => _items;

        public void AddItems(IEnumerable<ShoppingCartItem> shoppingCartItems, IEventStore eventStore)
        {
            foreach (var item in shoppingCartItems)
            {
                if (_items.Add(item))
                    // Raises an event through the eventStore for each item added
                    eventStore.Raise("ShoppingCartItemAdded", new {UserId, item});
            }
        }

        public void RemoveItems(int[] productCatalogIds, IEventStore eventStore)
        {
            foreach (var item in _items.Where(i => productCatalogIds.Contains(i.ProductCatalogId)).ToList())
            {
                if (_items.Remove(item))
                    // Raises an event through the eventStore for each item removed
                    eventStore.Raise("ShoppingCartItemRemoved", new {UserId, item});
            }
        }
    }
}
