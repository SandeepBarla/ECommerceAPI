using System.ComponentModel.DataAnnotations;

namespace ECommerceAPI.WebApi.DTOs.RequestModels
{
    public class CartAddOrUpdateItemRequest
    {
        public int ProductId { get; set; }
        
        public int Quantity { get; set; }
    }
}