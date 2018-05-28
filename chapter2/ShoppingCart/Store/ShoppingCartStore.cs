using System.Collections.Generic;

namespace ShoppingCart.Store
{
    public class ShoppingCartStore : IShoppingCartStore
    {
        private static readonly Dictionary<int, Domains.ShoppingCart> database =
            new Dictionary<int, Domains.ShoppingCart>();

        public Domains.ShoppingCart Get(int userId)
        {
            if (!database.ContainsKey(userId))
                database[userId] = new Domains.ShoppingCart(userId);

            return database[userId];
        }

        public void Save(Domains.ShoppingCart shoppingCart)
        {
            // Nothing needed. Saving would be needed with a real DB
        }
    }
}
