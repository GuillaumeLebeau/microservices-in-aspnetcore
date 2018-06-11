using System.Threading.Tasks;

namespace ShoppingCart.Stores
{
    public interface IShoppingCartStore
    {
        Task<Domains.ShoppingCart> Get(long userId);

        Task Save(Domains.ShoppingCart shoppingCart);
    }
}
