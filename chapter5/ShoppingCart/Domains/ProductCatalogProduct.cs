namespace ShoppingCart.Domains
{
    public class ProductCatalogProduct
    {
        public ProductCatalogProduct(int productId, string productName, string description, Money price)
        {
            ProductId = productId.ToString();
            ProductName = productName;
            ProductDescription = description;
            Price = price;
        }

        public string ProductId { get; }
        public string ProductName { get; }
        public string ProductDescription { get; }
        public Money Price { get; }
    }
}
