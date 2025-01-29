using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Newtonsoft.Json;

namespace ECommerceAPI.Application.Models
{
    public class Order
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int UserId { get; set; }

        // ✅ Store Products as a List instead of a string
        [Required]
        public List<OrderProduct> OrderProducts { get; set; } = new List<OrderProduct>();

        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal TotalAmount { get; set; }

        [Required]
        public string PaymentStatus { get; set; }

        [Required]
        public string OrderStatus { get; set; }

        [Required]
        public DateTime OrderDate { get; set; }

        [Required]
        public string ShippingAddress { get; set; }

        public string TrackingNumber { get; set; } = "Not Assigned";
    }
}