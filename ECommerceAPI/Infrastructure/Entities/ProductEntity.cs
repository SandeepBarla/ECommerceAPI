namespace ECommerceAPI.Infrastructure.Entities
{
    public class ProductEntity
    {
        public int Id { get; set; } // Primary Key
        public string Name { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
        public int Stock { get; set; }
        
        public ICollection<ProductMediaEntity> Media { get; set; } = new List<ProductMediaEntity>();
    }
}