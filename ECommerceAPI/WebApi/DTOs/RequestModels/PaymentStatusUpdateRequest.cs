using System.ComponentModel.DataAnnotations;

namespace ECommerceAPI.WebApi.DTOs.RequestModels
{
  public class PaymentStatusUpdateRequest
  {
    [Required]
    public string Status { get; set; } // "Pending", "Approved", "Rejected"

    public string? Remarks { get; set; } // Optional remarks for rejection
  }
}