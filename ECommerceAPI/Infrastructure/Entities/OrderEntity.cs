namespace ECommerceAPI.Infrastructure.Entities
{
    public class OrderEntity
    {
        public int Id { get; set; } // Primary Key
        public int UserId { get; set; } // Foreign Key
        public decimal TotalAmount { get; set; }
        public string OrderStatus { get; set; } = "Processing";
        public string PaymentStatus { get; set; }
        public string TrackingNumber { get; set; } = "Not Assigned";
        public DateTime OrderDate { get; set; }
        public string ShippingAddress { get; set; }

        // Navigation Properties
        public UserEntity User { get; set; }
        public ICollection<OrderProductEntity> OrderProducts { get; set; }
    }
}