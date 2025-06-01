using FluentValidation;
using ECommerceAPI.WebApi.DTOs.RequestModels;

namespace ECommerceAPI.WebApi.Validators
{
  public class PaymentStatusUpdateRequestValidator : AbstractValidator<PaymentStatusUpdateRequest>
  {
    public PaymentStatusUpdateRequestValidator()
    {
      RuleFor(p => p.Status)
          .NotEmpty().WithMessage("Payment status is required.")
          .Must(s => new[] { "Pending", "Approved", "Rejected" }.Contains(s))
          .WithMessage("Invalid payment status. Allowed values: Pending, Approved, Rejected.");

      RuleFor(p => p.Remarks)
          .NotEmpty()
          .When(p => p.Status == "Rejected")
          .WithMessage("Remarks are required when rejecting payment.");
    }
  }
}