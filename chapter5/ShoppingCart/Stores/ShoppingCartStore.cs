using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using Npgsql;
using ShoppingCart.Domains;

namespace ShoppingCart.Stores
{
    public class ShoppingCartStore : IShoppingCartStore
    {
        private readonly string _connectionString;
        
        private const string readItemsSql = @"
SELECT cart.shopping_cart_id AS Id,
       cart.user_id AS UserId
  FROM shopcart.shopping_cart as cart
 WHERE cart.user_id = @UserId;

SELECT item.product_catalog_id AS ProductCatalogId,
       item.product_name AS ProductName,
       item.product_description AS Description,
       item.currency AS Currency,
       item.amount AS Amount
  FROM shopcart.shopping_cart_items AS item
       INNER JOIN shopcart.shopping_cart AS cart ON item.shopping_cart_id = cart.shopping_cart_id
 WHERE cart.user_id = @UserId
";
        
        private const string deleteAllForShoppingCartSql = @"
DELETE FROM shopcart.shopping_cart_items AS item
      USING shopcart.shopping_cart AS cart
      WHERE item.shopping_cart_id = cart.shopping_cart_id
        AND cart.user_id = @UserId
";

        private const string deleteShoppingCartForUserSql = @"
DELETE FROM shopcart.shopping_cart
WHERE user_id = @UserId
";

        private const string addShoppingCartForUserSql = @"
INSERT INTO shopcart.shopping_cart (user_id)
     VALUES (@UserId)
  RETURNING shopping_cart_id
";

        private const string addAllForShoppingCartSql = @"
INSERT INTO shopcart.shopping_cart_items (shopping_cart_id, product_catalog_id, product_name, product_description, currency, amount)
     VALUES (@ShoppingCartId, @ProductCatalogId, @ProductName, @Description, @Currency, @Amount)
";

        public ShoppingCartStore(string connectionString)
        {
            _connectionString = connectionString;
        }

        public async Task<Domains.ShoppingCart> Get(long userId)
        {
            // Opens a connection to the ShoppingCart database
            using (var conn = await GetOpenConnectionAsync().ConfigureAwait(false))
            {
                // Uses a Dapper extension method to execute a SQL query and map the results back to a ShoppingCart object
                using (var multi = await conn.QueryMultipleAsync(readItemsSql, new {UserId = userId})
                    .ConfigureAwait(false))
                {
                    var shoppingCart =
                        await multi.ReadSingleOrDefaultAsync<Domains.ShoppingCart>().ConfigureAwait(false);
                    var items = multi.Read<ShoppingCartItem, Money, ShoppingCartItem>(
                        (item, money) =>
                        {
                            item.Price = money;
                            return item;
                        },
                        splitOn: "Currency"
                    );

                    return new Domains.ShoppingCart((shoppingCart?.Id).GetValueOrDefault(0), userId, items);
                }
            }
        }

        public async Task Save(Domains.ShoppingCart shoppingCart)
        {
            using (var conn = await GetOpenConnectionAsync().ConfigureAwait(false))
            using (var tx = conn.BeginTransaction())
            {
                // Deletes all preexisting shopping cart items
                await conn.ExecuteAsync(deleteAllForShoppingCartSql, new {shoppingCart.UserId}, tx)
                    .ConfigureAwait(false);

                // Deletes all preexisting shopping cart
                await conn.ExecuteAsync(deleteShoppingCartForUserSql, new {shoppingCart.UserId}, tx)
                    .ConfigureAwait(false);

                // Adds the current shopping cart
                shoppingCart.Id = await conn
                    .ExecuteScalarAsync<int>(addShoppingCartForUserSql, new {shoppingCart.UserId}, tx)
                    .ConfigureAwait(false);

                // Adds the current shopping cart items
                await conn.ExecuteAsync(
                        addAllForShoppingCartSql,
                        shoppingCart.Items.Select(
                            i => new
                            {
                                ShoppingCartId = shoppingCart.Id,
                                i.ProductCatalogId,
                                i.ProductName,
                                i.Description,
                                i.Price.Currency,
                                i.Price.Amount
                            }
                        ),
                        tx
                    )
                    .ConfigureAwait(false);

                tx.Commit();
            }
        }

        private async Task<IDbConnection> GetOpenConnectionAsync()
        {
            var connection = new NpgsqlConnection(_connectionString);
            await connection.OpenAsync().ConfigureAwait(false);

            return connection;
        }
    }
}