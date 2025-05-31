using FluentValidation;
using ECommerceAPI.WebApi.DTOs.RequestModels;

namespace ECommerceAPI.WebApi.Validators
{
    public class OrderCreateRequestValidator : AbstractValidator<OrderCreateRequest>
    {
        public OrderCreateRequestValidator()
        {
            RuleFor(o => o.OrderProducts)
                .NotEmpty().WithMessage("Order must have at least one product.");

            RuleForEach(o => o.OrderProducts).ChildRules(orderProduct =>
            {
                orderProduct.RuleFor(op => op.ProductId)
                    .GreaterThan(0).WithMessage("Product ID must be greater than zero.");

                orderProduct.RuleFor(op => op.Quantity)
                    .GreaterThan(0).WithMessage("Quantity must be at least 1.");
            });

            RuleFor(o => o.TotalAmount)
                .GreaterThan(0).WithMessage("Total amount must be greater than zero.");

            RuleFor(o => o.AddressId)
                .GreaterThan(0).WithMessage("Address ID must be valid.")
                .When(o => o.AddressId.HasValue);
        }
    }
}