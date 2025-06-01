namespace ECommerceAPI.Infrastructure.Entities
{
    public class ProductEntity
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public string Description { get; set; } = null!;

        // ✅ Simplified Pricing Structure
        public decimal OriginalPrice { get; set; } // Required - base price
        public decimal? DiscountedPrice { get; set; } // Optional - sale price

        // ✅ UI Enhancement Fields
        public bool IsFeatured { get; set; } = false;
        public DateTime? NewUntil { get; set; } // Product is "new" if NewUntil > DateTime.UtcNow

        // ✅ Category Relationship (Many-to-One)
        public int CategoryId { get; set; } = 1;
        public CategoryEntity Category { get; set; } = null!;

        // ✅ Size Relationship (One-to-Many)
        public int SizeId { get; set; } = 1;
        public SizeEntity Size { get; set; } = null!;

        // ✅ Audit Fields
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        // ✅ Favorites Relationship (Many-to-Many)
        public ICollection<FavoriteEntity> Favorites { get; set; } = new List<FavoriteEntity>();

        // ✅ Media Relationship (Existing)
        public List<ProductMediaEntity> Media { get; set; } = new();
    }
}