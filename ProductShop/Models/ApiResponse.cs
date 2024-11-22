using Newtonsoft.Json;

namespace ProductShop.Models
{
    public class ApiResponse<T>
    {
        [JsonProperty("statusCode")]
        public int StatusCode { get; set; }

        [JsonProperty("products")]
        public T Products { get; set; } = default!;

        [JsonProperty("boxes")]
        public T Boxes { get; set; } = default!;
    }
}