using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using ProductCatalog.Stores;

namespace ProductCatalog.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        private readonly IProductStore _productStore;

        public ProductsController(IProductStore productStore)
        {
            _productStore = productStore;
        }
        
        // GET api/products
        // Adds a Cache-Control header with max-age in seconds (86400 seconds = 24 hours)
        [HttpGet]
        [ResponseCache(Duration = 86400)]
        public IActionResult Get([FromQuery]string ids)
        {
            var productIds = ParseProductIdsFromQueryString(ids);
            var products = _productStore.GetProductsByIds(productIds);
            return Ok(products);
        }
        
        private static IEnumerable<int> ParseProductIdsFromQueryString(string productIdsString)
        {
            if (string.IsNullOrEmpty(productIdsString))
                return Enumerable.Empty<int>();
            
            return productIdsString.Split(',').Select(s => s.Replace("[", "").Replace("]", "")).Select(int.Parse);
        }
    }
}
