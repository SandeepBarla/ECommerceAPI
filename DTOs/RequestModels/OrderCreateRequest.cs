using System.ComponentModel.DataAnnotations;

namespace ECommerceAPI.DTOs.RequestModels
{
    public class OrderCreateRequest
    {
        [Required]
        public string Products { get; set; } // JSON string of products

        [Required]
        [Range(0.1, double.MaxValue, ErrorMessage = "Total amount must be greater than 0")]
        public decimal TotalAmount { get; set; }

        [Required]
        public string ShippingAddress { get; set; }
    }
}