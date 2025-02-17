namespace ECommerceAPI.WebApi.DTOs.ResponseModels
{
    public class ProductResponse
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
        public int Stock { get; set; }
        public List<ProductMediaResponse> Media { get; set; } = new List<ProductMediaResponse>();
    }
    public class ProductMediaResponse
    {
        public string MediaUrl { get; set; }
        public int OrderIndex { get; set; }
        public string Type { get; set; } // "Image" or "Video"
    }
}