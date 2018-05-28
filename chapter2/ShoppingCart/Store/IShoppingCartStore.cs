namespace ShoppingCart.Store
{
    public interface IShoppingCartStore
    {
        Domains.ShoppingCart Get(int userId);

        void Save(Domains.ShoppingCart shoppingCart);
    }
}
