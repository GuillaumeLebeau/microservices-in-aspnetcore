using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Polly;
using ShoppingCart.Domains;

namespace ShoppingCart.Clients
{
    public class ProductCatalogClient : IProductCatalogClient
    {
        // URL for the fake Product Catalog microservice
        private readonly string _productCatalogBaseUrl; // = @"http://private-05cc8-chapter2productcataloguemicroservice.apiary-mock.com";
        private readonly ICache _cache;

        private const string getProductPathTemplate = "/api/products?ids=[{0}]";

        // Use Polly's fluent API to set up a retry policy with an exponential back-off
        private static readonly Policy exponentialRetryPolicy = Policy.Handle<Exception>()
            .WaitAndRetryAsync(3, attempt => TimeSpan.FromMilliseconds(100 * Math.Pow(2, attempt)));
        
        public ProductCatalogClient(string productCatalogUrl, ICache cache)
        {
            _productCatalogBaseUrl = productCatalogUrl;
            _cache = cache;
        }
        
        public Task<IEnumerable<ShoppingCartItem>> GetShoppingCartItems(int[] productCatalogIds) =>
            // Wraps calls to the Product Catalog microservice in the retry policy
            exponentialRetryPolicy.ExecuteAsync(
                async () => await GetItemsFromCatalogService(productCatalogIds).ConfigureAwait(false)
            );
        
        private async Task<IEnumerable<ShoppingCartItem>> GetItemsFromCatalogService(int[] productCatalogIds)
        {
            var response = await RequestProductFromProductCatalog(productCatalogIds).ConfigureAwait(false);
            return await ConvertToShoppingCartItems(response).ConfigureAwait(false);
        }

        private async Task<HttpResponseMessage> RequestProductFromProductCatalog(int[] productCatalogIds)
        {
            // Adds the product IDs as a query string parameter to the path of the /products endpoint
            var productsResource = string.Format(getProductPathTemplate, string.Join(",", productCatalogIds));
            
            // Tries to retrieve a valid response from the cache
            var response = _cache.Get<HttpResponseMessage>(productsResource);
            
            // Only makes the HTTP request if there’s no response in the cache
            if (response == null)
            {
                // Creates a client for making the HTTP GET request
                using (var httpClient = new HttpClient())
                {
                    httpClient.BaseAddress = new Uri(_productCatalogBaseUrl);

                    // Tells HttpClient to perform HTTP GET asynchronously
                    response = await httpClient.GetAsync(productsResource).ConfigureAwait(false);
                    
                    AddToCache(productsResource, response);
                }
            }

            return response;
        }

        private void AddToCache(string resource, HttpResponseMessage response)
        {
            // Reads the Cache-Control header from response
            var maxAge = response.Headers.CacheControl.MaxAge;
            if (maxAge.HasValue)
                _cache.Add(resource, response, maxAge.Value);
        }

        private static async Task<IEnumerable<ShoppingCartItem>> ConvertToShoppingCartItems(HttpResponseMessage response)
        {
            response.EnsureSuccessStatusCode();

            // Uses Json.NET to deserialize the JSON from the Product Catalog microservice
            var products =
                JsonConvert.DeserializeObject<List<ProductCatalogProduct>>(
                    await response.Content.ReadAsStringAsync()
                        .ConfigureAwait(false)
                );

            // Creates a ShoppingCartItem for each product in the response
            return products.Select(
                p => new ShoppingCartItem(int.Parse(p.ProductId), p.ProductName, p.ProductDescription, p.Price));
        }
    }
}