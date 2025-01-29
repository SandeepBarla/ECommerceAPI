using FluentValidation;
using ECommerceAPI.WebApi.DTOs.RequestModels;

namespace ECommerceAPI.WebApi.Validators
{
    public class OrderStatusUpdateRequestValidator : AbstractValidator<OrderStatusUpdateRequest>
    {
        public OrderStatusUpdateRequestValidator()
        {
            RuleFor(o => o.Status)
                .NotEmpty().WithMessage("Status is required.")
                .Must(s => new[] { "Pending", "Shipped", "Delivered", "Cancelled" }.Contains(s))
                .WithMessage("Invalid order status.");
        }
    }
}