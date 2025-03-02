namespace ECommerceAPI.Infrastructure.Entities
{
    public class ProductEntity
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public string Description { get; set; } = null!;
        public decimal Price { get; set; }
        public int Stock { get; set; }
        
        // ✅ Category Relationship (Many-to-One)
        public int CategoryId { get; set; } = 1;
        public CategoryEntity Category { get; set; } = null!;

        // ✅ Size Relationship (One-to-Many)
        public int SizeId { get; set; } = 1;
        public SizeEntity Size { get; set; } = null!;
        
        // ✅ Favorites Relationship (Many-to-Many)
        public ICollection<FavoriteEntity> Favorites { get; set; } = new List<FavoriteEntity>();

        // ✅ Media Relationship (Existing)
        public List<ProductMediaEntity> Media { get; set; } = new();
    }
}