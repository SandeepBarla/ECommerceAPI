using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ECommerceAPI.WebApi.DTOs.RequestModels
{
    public class OrderCreateRequest
    {
        public List<OrderProductRequest> OrderProducts { get; set; } = new List<OrderProductRequest>();
        public decimal TotalAmount { get; set; }
        public int? AddressId { get; set; } // Foreign Key to Address (nullable for backward compatibility)
        public string? PaymentProofUrl { get; set; } // Cloudinary URL for payment proof image
    }

    public class OrderProductRequest
    {
        public int ProductId { get; set; }
        public int Quantity { get; set; }
    }
}