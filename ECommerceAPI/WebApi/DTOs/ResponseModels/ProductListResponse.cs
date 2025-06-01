namespace ECommerceAPI.WebApi.DTOs.ResponseModels
{
    public class ProductListResponse
    {
        public int Id { get; set; }
        public string Name { get; set; }

        // ✅ Simplified Pricing Structure
        public decimal OriginalPrice { get; set; }
        public decimal? DiscountedPrice { get; set; }
        public decimal? DiscountPercentage { get; set; } // Calculated field for UI

        // ✅ UI Enhancement Fields
        public bool IsFeatured { get; set; }
        public bool IsNew { get; set; } // Calculated from NewUntil

        // ✅ Essential Attributes for Listing
        public string CategoryName { get; set; } = "";
        public string SizeName { get; set; } = "";

        public string PrimaryImageUrl { get; set; } = ""; // ✅ Primary image for listings
    }
}