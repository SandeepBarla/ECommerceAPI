using System;
using System.Collections.Generic;

namespace ECommerceAPI.WebApi.DTOs.ResponseModels
{
    public class OrderResponse
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int? AddressId { get; set; }
        public AddressResponse? Address { get; set; }
        public CustomerInfoResponse Customer { get; set; }
        public List<OrderProductResponse> OrderProducts { get; set; }
        public decimal TotalAmount { get; set; }
        public string PaymentStatus { get; set; }
        public string OrderStatus { get; set; }
        public DateTime OrderDate { get; set; }
        public string TrackingNumber { get; set; }
        public string? PaymentProofUrl { get; set; }
    }

    public class OrderProductResponse
    {
        public int ProductId { get; set; }
        public int Quantity { get; set; }
        public string ProductName { get; set; }
        public decimal Price { get; set; }
    }

    public class CustomerInfoResponse
    {
        public int Id { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public string? Phone { get; set; }
    }
}