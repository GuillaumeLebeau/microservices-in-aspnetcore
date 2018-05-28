using System.Collections.Generic;
using System.Threading.Tasks;
using ShoppingCart.Domains;

namespace ShoppingCart.Clients
{
    public interface IProductCatalogClient
    {
        Task<IEnumerable<ShoppingCartItem>> GetShoppingCartItems(int[] productCatalogIds);
    }
}
