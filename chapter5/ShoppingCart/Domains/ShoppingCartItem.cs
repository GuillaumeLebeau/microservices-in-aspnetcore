namespace ShoppingCart.Domains
{
    public class ShoppingCartItem
    {
        public ShoppingCartItem()
        {
        }

        public ShoppingCartItem(int productCatalogId, string productName, string description, Money price)
        {
            ProductCatalogId = productCatalogId;
            ProductName = productName;
            Description = description;
            Price = price;
        }

        public int ProductCatalogId { get; set; }
        public string ProductName { get; set; }
        public string Description { get; set; }
        public Money Price { get; set; }
        
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((ShoppingCartItem) obj);
        }
        
        protected bool Equals(ShoppingCartItem other)
        {
            return ProductCatalogId == other.ProductCatalogId;
        }

        public override int GetHashCode()
        {
            return ProductCatalogId.GetHashCode();
        }
    }
}
