using ProductShop.Api;
using ProductShop.Utils;
using ProductShop.Handlers;
using ProductShop.Interfaces;

namespace ProductShop
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var client = new HttpClient();
            ICandidateProductStoreApi productStoreApi = new CandidateProductStoreApi(client);
            CheckoutHandler checkoutHandler = new CheckoutHandler();

            var productIds = await UserInputUtils.GetSelectedProductIds(productStoreApi);

            var products = await productStoreApi.GetProducts();
            var selectedProducts = products.Where(p => productIds.Contains(p.Id)).ToList();

            var product1 = selectedProducts[0];
            var product2 = selectedProducts[1];

            Console.WriteLine($"Selected Products:\nProduct 1: Name: {product1.Name}, Size: {product1.Dimensions.Width}x{product1.Dimensions.Height}\nProduct 2: Name: {product2.Name}, Size: {product2.Dimensions.Width}x{product2.Dimensions.Height}\n");

            var suitableBox = await BoxUtils.GetSuitableBox(productStoreApi, product1, product2);

            Console.WriteLine($"Selected Box: Box Id: {suitableBox.Id}, Size: {suitableBox.Width}x{suitableBox.Height}\n");
            await checkoutHandler.ProcessCheckout(productStoreApi, suitableBox.Id, productIds);
        }
    }
}