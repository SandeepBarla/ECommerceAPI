using ECommerceAPI.Application.Models.Enums;

namespace ECommerceAPI.WebApi.DTOs.RequestModels
{
    public class ProductUpsertRequest
    {
        public string Name { get; set; }
        public string Description { get; set; }

        // ✅ Simplified Pricing Structure
        public decimal OriginalPrice { get; set; }
        public decimal? DiscountedPrice { get; set; }

        // ✅ UI Enhancement Fields
        public bool IsFeatured { get; set; } = false;
        public DateTime? NewUntil { get; set; }

        // ✅ Relationships
        public int CategoryId { get; set; } = 1;
        public int SizeId { get; set; } = 1;

        public List<ProductMediaRequest> Media { get; set; } = new();
    }

    public class ProductMediaRequest
    {
        public string MediaUrl { get; set; } = string.Empty;
        public int OrderIndex { get; set; }

        public MediaType Type { get; set; } // ✅ Enum for Image or Video
    }
}