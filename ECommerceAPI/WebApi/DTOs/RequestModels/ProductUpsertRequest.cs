using ECommerceAPI.Application.Models.Enums;

namespace ECommerceAPI.WebApi.DTOs.RequestModels
{
    public class ProductUpsertRequest
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
        public int Stock { get; set; }
        
        public List<ProductMediaRequest> Media { get; set; } = new();
    }
    
    public class ProductMediaRequest
    {
        public string MediaUrl { get; set; } = string.Empty;
        public int OrderIndex { get; set; }
        
        public MediaType Type { get; set; } // ✅ Enum for Image or Video
    }
}