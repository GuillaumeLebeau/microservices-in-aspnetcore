namespace ShoppingCart.Domains
{
    public class ShoppingCartItem
    {
        public ShoppingCartItem(int productCatalogId, string productName, string description, Money price)
        {
            ProductCatalogId = productCatalogId;
            ProductName = productName;
            Description = description;
            Price = price;
        }

        public int ProductCatalogId { get; }
        public string ProductName { get; }
        public string Description { get; }
        public Money Price { get; }

        public override bool Equals(object obj)
        {
            if (obj == null || GetType() != obj.GetType())
                return false;

            var that = obj as ShoppingCartItem;
            return ProductCatalogId.Equals(that.ProductCatalogId);
        }

        // override object.GetHashCode
        public override int GetHashCode()
        {
            return ProductCatalogId.GetHashCode();
        }
    }
}
