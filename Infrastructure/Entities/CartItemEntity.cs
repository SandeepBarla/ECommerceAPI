namespace ECommerceAPI.Infrastructure.Entities
{
    public class CartItemEntity
    {
        public int Id { get; set; }
        public int CartId { get; set; } // Composite Key (CartId + ProductId)
        public int ProductId { get; set; } // Composite Key
        public int Quantity { get; set; }

        // Navigation Properties
        public ProductEntity Product { get; set; }
    }
}