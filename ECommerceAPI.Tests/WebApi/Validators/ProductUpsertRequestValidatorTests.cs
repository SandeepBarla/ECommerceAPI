using FluentValidation.TestHelper;
using ECommerceAPI.WebApi.DTOs.RequestModels;
using ECommerceAPI.WebApi.Validators;
using ECommerceAPI.Application.Models.Enums;
namespace ECommerceAPI.Tests.WebApi.Validators;

public class ProductUpsertRequestValidatorTests
{
    private readonly ProductUpsertRequestValidator _validator;

    public ProductUpsertRequestValidatorTests()
    {
        _validator = new ProductUpsertRequestValidator();
    }

    // ✅ Name Validation Tests
    [Fact]
    public void ShouldHaveError_WhenNameIsEmpty()
    {
        var request = new ProductUpsertRequest { Name = "" };
        var result = _validator.TestValidate(request);
        result.ShouldHaveValidationErrorFor(x => x.Name);
    }

    [Fact]
    public void ShouldHaveError_WhenNameExceedsMaxLength()
    {
        var request = new ProductUpsertRequest { Name = new string('A', 101) };
        var result = _validator.TestValidate(request);
        result.ShouldHaveValidationErrorFor(x => x.Name);
    }

    // ✅ OriginalPrice Validation Tests
    [Fact]
    public void ShouldHaveError_WhenOriginalPriceIsNegative()
    {
        var request = new ProductUpsertRequest { OriginalPrice = -1 };
        var result = _validator.TestValidate(request);
        result.ShouldHaveValidationErrorFor(x => x.OriginalPrice);
    }

    [Fact]
    public void ShouldHaveError_WhenOriginalPriceIsZero()
    {
        var request = new ProductUpsertRequest { OriginalPrice = 0 };
        var result = _validator.TestValidate(request);
        result.ShouldHaveValidationErrorFor(x => x.OriginalPrice);
    }

    // ✅ DiscountedPrice Validation Tests
    [Fact]
    public void ShouldHaveError_WhenDiscountedPriceIsNegative()
    {
        var request = new ProductUpsertRequest { DiscountedPrice = -1 };
        var result = _validator.TestValidate(request);
        result.ShouldHaveValidationErrorFor(x => x.DiscountedPrice);
    }

    [Fact]
    public void ShouldHaveError_WhenDiscountedPriceIsZero()
    {
        var request = new ProductUpsertRequest { DiscountedPrice = 0 };
        var result = _validator.TestValidate(request);
        result.ShouldHaveValidationErrorFor(x => x.DiscountedPrice);
    }

    [Fact]
    public void ShouldNotHaveError_WhenDiscountedPriceIsNull()
    {
        var request = new ProductUpsertRequest
        {
            OriginalPrice = 100m,
            DiscountedPrice = null
        };
        var result = _validator.TestValidate(request);
        result.ShouldNotHaveValidationErrorFor(x => x.DiscountedPrice);
    }

    // ✅ Description Validation Tests
    [Fact]
    public void ShouldHaveError_WhenDescriptionExceedsMaxLength()
    {
        var request = new ProductUpsertRequest { Description = new string('A', 1001) };
        var result = _validator.TestValidate(request);
        result.ShouldHaveValidationErrorFor(x => x.Description);
    }

    [Fact]
    public void ShouldNotHaveError_WhenDescriptionIsEmpty()
    {
        var request = new ProductUpsertRequest { Description = "" };
        var result = _validator.TestValidate(request);
        result.ShouldNotHaveValidationErrorFor(x => x.Description);
    }

    // ✅ Discount Logic Validation Tests
    [Fact]
    public void ShouldHaveError_WhenDiscountedPriceIsGreaterThanOriginalPrice()
    {
        var request = new ProductUpsertRequest
        {
            OriginalPrice = 80m,
            DiscountedPrice = 100m // Should be less than original price
        };
        var result = _validator.TestValidate(request);
        result.ShouldHaveValidationErrorFor(x => x.DiscountedPrice);
    }

    [Fact]
    public void ShouldNotHaveError_WhenDiscountedPriceIsLessThanOriginalPrice()
    {
        var request = new ProductUpsertRequest
        {
            OriginalPrice = 120m,
            DiscountedPrice = 100m
        };
        var result = _validator.TestValidate(request);
        result.ShouldNotHaveValidationErrorFor(x => x.DiscountedPrice);
    }

    [Fact]
    public void ShouldHaveError_WhenCategoryIdIsZero()
    {
        var request = new ProductUpsertRequest { CategoryId = 0 };
        var result = _validator.TestValidate(request);
        result.ShouldHaveValidationErrorFor(x => x.CategoryId);
    }

    [Fact]
    public void ShouldHaveError_WhenSizeIdIsZero()
    {
        var request = new ProductUpsertRequest { SizeId = 0 };
        var result = _validator.TestValidate(request);
        result.ShouldHaveValidationErrorFor(x => x.SizeId);
    }

    [Fact]
    public void ShouldHaveError_WhenNewUntilIsInThePast()
    {
        var request = new ProductUpsertRequest
        {
            NewUntil = DateTime.UtcNow.AddDays(-1) // Past date
        };
        var result = _validator.TestValidate(request);
        result.ShouldHaveValidationErrorFor(x => x.NewUntil);
    }

    [Fact]
    public void ShouldNotHaveError_WhenNewUntilIsInTheFuture()
    {
        var request = new ProductUpsertRequest
        {
            NewUntil = DateTime.UtcNow.AddDays(30) // Future date
        };
        var result = _validator.TestValidate(request);
        result.ShouldNotHaveValidationErrorFor(x => x.NewUntil);
    }

    [Fact]
    public void ShouldNotHaveError_WhenNewUntilIsNull()
    {
        var request = new ProductUpsertRequest
        {
            NewUntil = null
        };
        var result = _validator.TestValidate(request);
        result.ShouldNotHaveValidationErrorFor(x => x.NewUntil);
    }

    // ✅ Media Validation Tests
    [Fact]
    public void ShouldHaveError_WhenMediaIsEmpty()
    {
        var request = new ProductUpsertRequest { Media = new List<ProductMediaRequest>() };
        var result = _validator.TestValidate(request);
        result.ShouldHaveValidationErrorFor(x => x.Media);
    }

    [Fact]
    public void ShouldHaveError_WhenMediaHasInvalidUrl()
    {
        var request = new ProductUpsertRequest
        {
            Media = new List<ProductMediaRequest>
            {
                new() { MediaUrl = "invalid-url", Type = MediaType.Image, OrderIndex = 1 }
            }
        };
        var result = _validator.TestValidate(request);
        result.ShouldHaveValidationErrorFor(x => x.Media);
    }

    [Fact]
    public void ShouldHaveError_WhenMediaHasInvalidMediaType()
    {
        var request = new ProductUpsertRequest
        {
            Media = new List<ProductMediaRequest>
            {
                new() { MediaUrl = "https://example.com/image1.jpg", Type = (MediaType)999, OrderIndex = 1 }
            }
        };
        var result = _validator.TestValidate(request);
        result.ShouldHaveValidationErrorFor(x => x.Media);
    }

    [Fact]
    public void ShouldHaveError_WhenMediaOrderIndexDoesNotStartFromOne()
    {
        var request = new ProductUpsertRequest
        {
            Media = new List<ProductMediaRequest>
            {
                new() { MediaUrl = "https://example.com/image1.jpg", Type = MediaType.Image, OrderIndex = 2 }
            }
        };
        var result = _validator.TestValidate(request);
        result.ShouldHaveValidationErrorFor(x => x.Media);
    }

    [Fact]
    public void ShouldHaveError_WhenMediaOrderIndexesAreNotSequential()
    {
        var request = new ProductUpsertRequest
        {
            Media = new List<ProductMediaRequest>
            {
                new() { MediaUrl = "https://example.com/image1.jpg", Type = MediaType.Image, OrderIndex = 1 },
                new() { MediaUrl = "https://example.com/image2.jpg", Type = MediaType.Image, OrderIndex = 3 } // Should be 2
            }
        };
        var result = _validator.TestValidate(request);
        result.ShouldHaveValidationErrorFor(x => x.Media);
    }

    // ✅ Integration test for simplified product model
    [Fact]
    public void ShouldPassValidation_WhenValidSimplifiedProductRequest()
    {
        var request = new ProductUpsertRequest
        {
            Name = "Test Product",
            Description = "Test Description",
            OriginalPrice = 129.99m,
            DiscountedPrice = 99.99m,
            IsFeatured = true,
            NewUntil = DateTime.UtcNow.AddDays(30),
            CategoryId = 1,
            SizeId = 1,
            Media = new List<ProductMediaRequest>
            {
                new() { MediaUrl = "https://example.com/image1.jpg", Type = MediaType.Image, OrderIndex = 1 },
                new() { MediaUrl = "https://example.com/video1.mp4", Type = MediaType.Video, OrderIndex = 2 }
            }
        };

        var result = _validator.TestValidate(request);
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void ShouldPassValidation_WhenValidProductRequestWithoutDiscount()
    {
        var request = new ProductUpsertRequest
        {
            Name = "Test Product Without Discount",
            Description = "Test Description",
            OriginalPrice = 99.99m,
            DiscountedPrice = null, // No discount
            IsFeatured = false,
            CategoryId = 1,
            SizeId = 1,
            Media = new List<ProductMediaRequest>
            {
                new() { MediaUrl = "https://example.com/image1.jpg", Type = MediaType.Image, OrderIndex = 1 }
            }
        };

        var result = _validator.TestValidate(request);
        result.ShouldNotHaveAnyValidationErrors();
    }
}