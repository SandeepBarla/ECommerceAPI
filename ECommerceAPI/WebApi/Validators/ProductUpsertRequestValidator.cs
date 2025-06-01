using FluentValidation;
using ECommerceAPI.WebApi.DTOs.RequestModels;
using ECommerceAPI.Application.Models.Enums;

namespace ECommerceAPI.WebApi.Validators
{
    public class ProductUpsertRequestValidator : AbstractValidator<ProductUpsertRequest>
    {
        public ProductUpsertRequestValidator()
        {
            RuleFor(p => p.Name)
                .NotEmpty().WithMessage("Product name is required.")
                .MaximumLength(100).WithMessage("Product name must not exceed 100 characters.");

            RuleFor(p => p.OriginalPrice)
                .GreaterThan(0).WithMessage("Original price must be greater than zero.");

            // ✅ Discounted price validation
            RuleFor(p => p.DiscountedPrice)
                .GreaterThan(0)
                .LessThan(p => p.OriginalPrice)
                .When(p => p.DiscountedPrice.HasValue)
                .WithMessage("Discounted price must be greater than zero and less than original price.");

            RuleFor(p => p.Description)
                .MaximumLength(1000).WithMessage("Description must not exceed 1000 characters.")
                .When(p => !string.IsNullOrEmpty(p.Description));

            RuleFor(p => p.CategoryId)
                .GreaterThan(0).WithMessage("CategoryId must be greater than zero.");

            RuleFor(p => p.SizeId)
                .GreaterThan(0).WithMessage("SizeId must be greater than zero.");

            // ✅ NewUntil validation
            RuleFor(p => p.NewUntil)
                .GreaterThan(DateTime.UtcNow)
                .When(p => p.NewUntil.HasValue)
                .WithMessage("NewUntil must be a future date.");

            RuleFor(p => p.Media)
                .NotEmpty().WithMessage("At least one media item is required.")
                .Must(HaveValidOrderIndexes).WithMessage("Media items must have sequential order indexes starting from 1.")
                .Must(m => m.All(IsValidMedia)).WithMessage("One or more media entries are invalid.");
        }

        // ✅ Validate individual media item
        private bool IsValidMedia(ProductMediaRequest media)
        {
            return Uri.TryCreate(media.MediaUrl, UriKind.Absolute, out _) &&
                   Enum.IsDefined(typeof(MediaType), media.Type);
        }

        // ✅ Ensure order indexes are sequential (1,2,3,...)
        private bool HaveValidOrderIndexes(List<ProductMediaRequest> mediaList)
        {
            var orderIndexes = mediaList.Select(m => m.OrderIndex).OrderBy(x => x).ToList();

            // ✅ Ensure order starts from 1 and follows sequentially
            return orderIndexes.Count == mediaList.Count &&
                   orderIndexes.SequenceEqual(Enumerable.Range(1, mediaList.Count));
        }
    }
}