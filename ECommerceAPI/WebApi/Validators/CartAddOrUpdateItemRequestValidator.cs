using ECommerceAPI.WebApi.DTOs.RequestModels;
using FluentValidation;

namespace ECommerceAPI.WebApi.Validators
{
    public class CartAddOrUpdateItemRequestValidator : AbstractValidator<CartAddOrUpdateItemRequest>
    {
        public CartAddOrUpdateItemRequestValidator()
        {
            RuleFor(c => c.ProductId)
                .GreaterThan(0).WithMessage("Product ID must be greater than zero.");

            RuleFor(c => c.Quantity)
                .GreaterThanOrEqualTo(0).WithMessage("Quantity cannot be negative.");
        }
    }
}