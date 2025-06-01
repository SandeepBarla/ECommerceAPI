namespace ECommerceAPI.WebApi.DTOs.ResponseModels
{
    public class ProductResponse
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }

        // ✅ Simplified Pricing Structure
        public decimal OriginalPrice { get; set; }
        public decimal? DiscountedPrice { get; set; }
        public decimal? DiscountPercentage { get; set; } // Calculated field for UI

        // ✅ UI Enhancement Fields
        public bool IsFeatured { get; set; }
        public bool IsNew { get; set; } // Calculated from NewUntil
        public DateTime? NewUntil { get; set; }

        // ✅ Relationships
        public int CategoryId { get; set; }
        public int SizeId { get; set; }
        public string CategoryName { get; set; } = "";
        public string SizeName { get; set; } = "";

        // ✅ Audit Fields
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        public List<ProductMediaResponse> Media { get; set; } = new List<ProductMediaResponse>();
    }
    public class ProductMediaResponse
    {
        public string MediaUrl { get; set; }
        public int OrderIndex { get; set; }
        public string Type { get; set; } // "Image" or "Video"
    }
}