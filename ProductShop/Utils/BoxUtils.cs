using ProductShop.Interfaces;
using ProductShop.Models;

namespace ProductShop.Utils
{
    public static class BoxUtils
    {
        public static (int Width, int Height) GetHorizontalConfiguration(Product product1, Product product2)
        {
            int width = product1.Dimensions.Width + product2.Dimensions.Width;
            int height = Math.Max(product1.Dimensions.Height, product2.Dimensions.Height);
            return (width, height);
        }

        public static (int Width, int Height) GetVerticalConfiguration(Product product1, Product product2)
        {
            int width = Math.Max(product1.Dimensions.Width, product2.Dimensions.Width);
            int height = product1.Dimensions.Height + product2.Dimensions.Height;
            return (width, height);
        }

        public static async Task<Box> GetSuitableBox(ICandidateProductStoreApi productStoreApi, Product product1, Product product2)
        {
            var horizontalConfig = GetHorizontalConfiguration(product1, product2);
            var verticalConfig = GetVerticalConfiguration(product1, product2);

            var boxes = await productStoreApi.GetBoxes();

            var suitableBox = boxes
                .Where(b => (b.Width >= horizontalConfig.Width && b.Height >= horizontalConfig.Height) ||
                            (b.Width >= verticalConfig.Width && b.Height >= verticalConfig.Height))
                .OrderBy(b => b.Volume)
                .FirstOrDefault();

            if (suitableBox == null)
            {
                throw new InvalidOperationException("No suitable box found for the selected products.");
            }

            return suitableBox;
        }
    }
}