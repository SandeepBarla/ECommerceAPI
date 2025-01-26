using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ECommerceAPI.Models
{
    public class Order
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int UserId { get; set; }

        [ForeignKey("UserId")]
        public User User { get; set; } // Navigation property

        [Required]
        public string Products { get; set; } // JSON-encoded product list

        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal TotalAmount { get; set; }

        [Required]
        public string PaymentStatus { get; set; } // Pending, Completed, Failed

        [Required]
        public DateTime OrderDate { get; set; } = DateTime.UtcNow;

        [Required]
        public string ShippingAddress { get; set; }

        public string TrackingNumber { get; set; } // Optional

        public string OrderStatus { get; set; } = "Processing"; // Processing, Shipped, Delivered, Canceled
    }
}