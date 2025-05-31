namespace ECommerceAPI.Infrastructure.Entities
{
    public class OrderEntity
    {
        public int Id { get; set; } // Primary Key
        public int UserId { get; set; } // Foreign Key
        public int? AddressId { get; set; } // Foreign Key to AddressEntity (nullable for existing orders)
        public decimal TotalAmount { get; set; }
        public string OrderStatus { get; set; } = "Processing";
        public string PaymentStatus { get; set; }
        public string TrackingNumber { get; set; } = "Not Assigned";
        public DateTime OrderDate { get; set; }
        public string? PaymentProofUrl { get; set; } // Cloudinary URL for payment proof image

        // Navigation Properties
        public UserEntity User { get; set; }
        public AddressEntity? Address { get; set; } // Navigation to shipping address (nullable)
        public ICollection<OrderProductEntity> OrderProducts { get; set; }
    }
}