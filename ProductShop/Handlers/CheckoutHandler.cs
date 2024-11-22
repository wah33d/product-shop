using ProductShop.Interfaces;

namespace ProductShop.Handlers
{
    public class CheckoutHandler
    {
        public async Task ProcessCheckout(ICandidateProductStoreApi productStoreApi, int boxId, int[] productIds)
        {
            var checkoutSummary = await productStoreApi.Checkout(boxId, productIds);
            Console.WriteLine($"Checkout Result: {checkoutSummary.Result}");
        }
    }
}