namespace ECommerceAPI.Application.Models
{
    public class Product
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }

        // ✅ Simplified Pricing Structure
        public decimal OriginalPrice { get; set; }
        public decimal? DiscountedPrice { get; set; }

        // ✅ UI Enhancement Fields
        public bool IsFeatured { get; set; } = false;
        public DateTime? NewUntil { get; set; }

        // ✅ Relationships
        public int CategoryId { get; set; }
        public int SizeId { get; set; }

        // ✅ Audit Fields
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        public ICollection<ProductMedia> Media { get; set; } = new List<ProductMedia>();
    }
}