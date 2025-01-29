namespace ECommerceAPI.Infrastructure.Entities
{
    public class OrderProductEntity
    {
        public int OrderId { get; set; } // Composite Key (OrderId + ProductId)
        public int ProductId { get; set; } // Composite Key
        public int Quantity { get; set; }

        // Navigation Properties
        public OrderEntity Order { get; set; }
        public ProductEntity Product { get; set; }
    }
}