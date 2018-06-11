using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using ShoppingCart.Clients;
using ShoppingCart.Stores;

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

        [HttpGet("{userId:long}")]
        public async Task<IActionResult> Get(long userId)
        {
            var shoppingCart = await _shoppingCartStore.Get(userId).ConfigureAwait(false);
            return Ok(shoppingCart);
        }

        [HttpPost("{userId:long}/items")]
        public async Task<IActionResult> Post(long userId, [FromBody] int[] productCatalogIds)
        {
            var shoppingCart = await _shoppingCartStore.Get(userId).ConfigureAwait(false);
            var shoppingCartItems = await _productCatalog.GetShoppingCartItems(productCatalogIds).ConfigureAwait(false);
            shoppingCart.AddItems(shoppingCartItems, _eventStore);
            await _shoppingCartStore.Save(shoppingCart).ConfigureAwait(false);

            return Ok(shoppingCart);
        }

        [HttpDelete("{userId:long}/items")]
        public async Task<IActionResult> Delete(long userId, [FromBody] int[] productCatalogIds)
        {
            var shoppingCart = await _shoppingCartStore.Get(userId).ConfigureAwait(false);
            shoppingCart.RemoveItems(productCatalogIds, _eventStore);
            await _shoppingCartStore.Save(shoppingCart).ConfigureAwait(false);

            return Ok(shoppingCart);
        }
    }
}
