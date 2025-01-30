using System.Collections.Generic;

namespace ECommerceAPI.WebApi.DTOs.ResponseModels
{
    public class CartResponse
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public List<CartItemResponse> CartItems { get; set; } = new List<CartItemResponse>();
        public decimal TotalPrice => CartItems.Sum(item => item.Product.Price * item.Quantity);
    }
}