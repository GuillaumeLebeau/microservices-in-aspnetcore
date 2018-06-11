using System.Collections.Generic;
using ProductCatalog.Domains;

namespace ProductCatalog.Stores
{
    public interface IProductStore
    {
        IEnumerable<ProductCatalogProduct> GetProductsByIds(IEnumerable<int> productIds);
    }
}
