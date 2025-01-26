using System.Collections.Generic;

namespace ECommerceAPI.DTOs.ResponseModels
{
    public class CartResponse
    {
        public int CartId { get; set; }
        public List<CartItemResponse> Items { get; set; } = new List<CartItemResponse>();
        public decimal TotalPrice => Items.Sum(item => item.Price * item.Quantity);
    }
}