using System.Collections.Generic;
using System.Linq;
using ProductCatalog.Domains;

namespace ProductCatalog.Stores
{
    public class StaticProductStore : IProductStore
    {
        public IEnumerable<ProductCatalogProduct> GetProductsByIds(IEnumerable<int> productIds)
        {
            return productIds.Select(
                id => new ProductCatalogProduct(id, "foo" + id, "bar", new Money("EUR", 40.3M * id))
            );
        }
    }
}