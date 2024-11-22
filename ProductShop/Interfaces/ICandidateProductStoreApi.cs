using ProductShop.Models;

namespace ProductShop.Interfaces
{
    public interface ICandidateProductStoreApi
    {
        Task<IList<Product>> GetProducts();
        Task<ProductDimensions> GetProductDimensions(int productId);
        Task<IList<Box>> GetBoxes();
        Task<CheckoutSummary> Checkout(int boxId, int[] productIds);
    }
}