using System;
using System.Collections.Generic;

namespace ECommerceAPI.DTOs.ResponseModels
{
    public class OrderResponse
    {
        public int Id { get; set; }
        public List<OrderProductResponse> Products { get; set; }
        public decimal TotalAmount { get; set; }
        public string PaymentStatus { get; set; }
        public string OrderStatus { get; set; }
        public DateTime OrderDate { get; set; }
        public string ShippingAddress { get; set; }
        public string TrackingNumber { get; set; }
    }

    public class OrderProductResponse
    {
        public int ProductId { get; set; }
        public int Quantity { get; set; }
    }
}