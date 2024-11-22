using Newtonsoft.Json;
using ProductShop.Interfaces;
using ProductShop.Models;

namespace ProductShop.Api
{
    public class CandidateProductStoreApi : ICandidateProductStoreApi
    {
        private readonly HttpClient _client;

        public CandidateProductStoreApi(HttpClient client)
        {
            _client = client;
            _client.BaseAddress = new Uri("https://api.deverything.se/candidate-product-store/");
            _client.DefaultRequestHeaders.Add("Accept", "application/json");
            _client.DefaultRequestHeaders.Add("USER", "waheed");
            _client.DefaultRequestHeaders.Add("APIKEY", "SUPERSECRETAPIKEY");
        }

        public async Task<IList<Product>> GetProducts()
        {
            var response = await _client.GetAsync("products");
            response.EnsureSuccessStatusCode();
            var responseData = await response.Content.ReadAsStringAsync();
            var productsResponse = JsonConvert.DeserializeObject<ApiResponse<IList<Product>>>(responseData);
            if (productsResponse?.Products == null)
            {
                throw new InvalidOperationException("Failed to retrieve products.");
            }
            var products = productsResponse.Products;

            var tasks = products.Select(async product =>
            {
                product.Dimensions = await GetProductDimensions(product.Id);
            });

            await Task.WhenAll(tasks);

            return products;
        }

        public async Task<ProductDimensions> GetProductDimensions(int productId)
        {
            var response = await _client.GetAsync($"products/{productId}");
            response.EnsureSuccessStatusCode();
            var responseData = await response.Content.ReadAsStringAsync();
            var dimensions = JsonConvert.DeserializeObject<ProductDimensions>(responseData);
            if (dimensions == null)
            {
                throw new InvalidOperationException($"Failed to retrieve dimensions for product {productId}.");
            }
            return dimensions;
        }

        public async Task<IList<Box>> GetBoxes()
        {
            var response = await _client.GetAsync("boxes");
            response.EnsureSuccessStatusCode();
            var responseData = await response.Content.ReadAsStringAsync();
            var boxesResponse = JsonConvert.DeserializeObject<ApiResponse<IList<Box>>>(responseData);
            if (boxesResponse?.Boxes == null)
            {
                throw new InvalidOperationException("Failed to retrieve boxes.");
            }
            return boxesResponse.Boxes;
        }

        public async Task<CheckoutSummary> Checkout(int boxId, int[] productIds)
        {
            var requestBody = new { boxId, productIds };
            var content = new StringContent(JsonConvert.SerializeObject(requestBody), System.Text.Encoding.UTF8, "application/json");
            var response = await _client.PostAsync("checkout", content);
            response.EnsureSuccessStatusCode();
            var responseData = await response.Content.ReadAsStringAsync();
            var checkoutSummary = JsonConvert.DeserializeObject<CheckoutSummary>(responseData);
            if (checkoutSummary == null)
            {
                throw new InvalidOperationException("Failed to retrieve checkout summary.");
            }
            return checkoutSummary;
        }
    }
}