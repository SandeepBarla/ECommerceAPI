using FluentValidation;
using ECommerceAPI.WebApi.DTOs.RequestModels;

namespace ECommerceAPI.WebApi.Validators
{
    public class ProductUpsertRequestValidator : AbstractValidator<ProductUpsertRequest>
    {
        public ProductUpsertRequestValidator()
        {
            RuleFor(p => p.Name)
                .NotEmpty().WithMessage("Product name is required.")
                .MaximumLength(100).WithMessage("Product name must not exceed 100 characters.");

            RuleFor(p => p.Price)
                .GreaterThan(0).WithMessage("Price must be greater than zero.");

            RuleFor(p => p.Stock)
                .GreaterThanOrEqualTo(0).WithMessage("Stock cannot be negative.");

            RuleFor(p => p.Description)
                .MaximumLength(500).WithMessage("Description must not exceed 500 characters.")
                .When(p => !string.IsNullOrEmpty(p.Description));

            RuleFor(p => p.ImageUrl)
                .Must(BeAValidUrl).WithMessage("Invalid image URL format.")
                .When(p => !string.IsNullOrEmpty(p.ImageUrl));
        }

        private bool BeAValidUrl(string url)
        {
            return Uri.TryCreate(url, UriKind.Absolute, out _);
        }
    }
}