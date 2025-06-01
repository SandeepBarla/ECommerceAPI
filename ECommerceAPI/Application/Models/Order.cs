namespace ECommerceAPI.Application.Models
{
    public class Order
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int? AddressId { get; set; } // Foreign Key to Address (nullable)
        public List<OrderProduct> OrderProducts { get; set; } = new List<OrderProduct>();
        public decimal TotalAmount { get; set; }
        public string PaymentStatus { get; set; } = "Pending";
        public string? PaymentRemarks { get; set; } // Admin remarks for payment rejection
        public string OrderStatus { get; set; } = "Processing";
        public DateTime OrderDate { get; set; }
        public string TrackingNumber { get; set; } = "Not Assigned";
        public string? PaymentProofUrl { get; set; } // Cloudinary URL for payment proof image
    }
}