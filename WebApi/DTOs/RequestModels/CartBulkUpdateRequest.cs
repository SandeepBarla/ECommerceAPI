using System.ComponentModel.DataAnnotations;

namespace ECommerceAPI.WebApi.DTOs.RequestModels
{
    public class CartBulkUpdateRequest
    {
        [Required]
        public List<CartUpsertRequest> Items { get; set; } = new List<CartUpsertRequest>();
    }
}