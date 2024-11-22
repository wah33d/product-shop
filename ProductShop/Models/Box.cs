namespace ProductShop.Models
{
    public class Box
    {
        public int Id { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
        public int Volume => Width * Height;
    }
}