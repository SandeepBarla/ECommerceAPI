using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ECommerceAPI.WebApi.DTOs.RequestModels
{
    public class OrderCreateRequest
    {
        public List<OrderProductRequest> OrderProducts { get; set; } = new List<OrderProductRequest>();
        public decimal TotalAmount { get; set; }
        public string ShippingAddress { get; set; }
    }

    public class OrderProductRequest
    {
        public int ProductId { get; set; }
        public int Quantity { get; set; }
    }
}