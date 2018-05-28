namespace ShoppingCart.Domains
{
    public class Money
    {
        public Money(string currency, decimal amount)
        {
            Currency = currency;
            Amount = amount;
        }

        public string Currency { get; }

        public decimal Amount { get; }
    }
}
