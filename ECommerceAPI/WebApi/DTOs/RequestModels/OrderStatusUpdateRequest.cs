using System.ComponentModel.DataAnnotations;

namespace ECommerceAPI.WebApi.DTOs.RequestModels
{
    public class OrderStatusUpdateRequest
    {
        public string Status { get; set; }
    }
}