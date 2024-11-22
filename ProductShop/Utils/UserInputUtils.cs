using ProductShop.Interfaces;

namespace ProductShop.Utils
{
    public static class UserInputUtils
    {
        public static async Task<int[]> GetSelectedProductIds(ICandidateProductStoreApi productStoreApi)
        {
            var products = await productStoreApi.GetProducts();
            Console.WriteLine("Available products:");
            foreach (var product in products)
            {
                Console.WriteLine($"Id: {product.Id}, Name: {product.Name}, Price: {product.Price}");
            }

            Console.Write("Enter product IDs: ");
            var input = Console.ReadLine();
            var productIds = input?.Split(',').Select(int.Parse).ToArray();

            if (productIds == null || productIds.Length != 2)
            {
                Console.WriteLine("Enter 2 product IDs (comma separated) to calculate box: ");
                throw new InvalidOperationException("Invalid product selection.");
            }

            return productIds;
        }
    }
}