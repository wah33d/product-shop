using ProductShop.Models;

namespace ProductShop.Interfaces
{
    public interface ICandidateProductStore
    {
        Task<IList<Product>> GetAllProducts();
        Task<IList<Product>> GetAllProductsAbovePrice(decimal price);
        Task<Box> CalculateSmallestBoxForTwoProducts(Product product1, Product product2);
        Task<CheckoutSummary> Checkout(Box box, Product product1, Product product2);
    }
}