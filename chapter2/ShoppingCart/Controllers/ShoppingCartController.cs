using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using ShoppingCart.Clients;
using ShoppingCart.Store;

namespace ShoppingCart.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ShoppingCartController : ControllerBase
    {
        private readonly IEventStore _eventStore;
        private readonly IProductCatalogClient _productCatalog;
        private readonly IShoppingCartStore _shoppingCartStore;

        public ShoppingCartController(
            IShoppingCartStore shoppingCartStore,
            IProductCatalogClient productCatalog,
            IEventStore eventStore)
        {
            _shoppingCartStore = shoppingCartStore;
            _productCatalog = productCatalog;
            _eventStore = eventStore;
        }

        [HttpGet("{userId:int}")]
        public IActionResult Get(int userId)
        {
            var shoppingCart = _shoppingCartStore.Get(userId);
            return Ok(shoppingCart);
        }

        [HttpPost("{userId:int}/items")]
        public async Task<IActionResult> Post(int userId, [FromBody] int[] productCatalogIds)
        {
            var shoppingCart = _shoppingCartStore.Get(userId);
            var shoppingCartItems = await _productCatalog.GetShoppingCartItems(productCatalogIds).ConfigureAwait(false);
            shoppingCart.AddItems(shoppingCartItems, _eventStore);
            _shoppingCartStore.Save(shoppingCart);

            return Ok(shoppingCart);
        }

        [HttpDelete("{userId:int}/items")]
        public IActionResult Delete(int userId, [FromBody] int[] productCatalogIds)
        {
            var shoppingCart = _shoppingCartStore.Get(userId);
            shoppingCart.RemoveItems(productCatalogIds, _eventStore);
            _shoppingCartStore.Save(shoppingCart);

            return Ok(shoppingCart);
        }
    }
}
