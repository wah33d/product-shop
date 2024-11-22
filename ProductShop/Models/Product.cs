namespace ProductShop.Models
{
    public class Product
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public ProductDimensions Dimensions { get; set; } = new ProductDimensions();
    }

    public class ProductDimensions
    {
        public int Width { get; set; }
        public int Height { get; set; }
    }
}