namespace ECommerceAPI.Infrastructure.Entities
{
    public class SizeEntity
    {
        public int Id { get; set; }

        // ✅ Size Name (e.g., S, M, L, Free Size)
        public string Name { get; set; } = "Free Size";

        // ✅ Sorting Order for Display
        public int SortOrder { get; set; } = 1;

        // ✅ Navigation Property (One-to-Many)
        public ICollection<ProductEntity> Products { get; set; } = new List<ProductEntity>();
    }
}