using System.ComponentModel.DataAnnotations;

namespace ECommerceAPI.WebApi.DTOs.RequestModels
{
    public class OrderStatusUpdateRequest
    {
        [Required]
        public string Status { get; set; }
    }
}