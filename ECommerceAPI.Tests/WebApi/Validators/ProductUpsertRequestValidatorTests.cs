using FluentValidation.TestHelper;
using ECommerceAPI.WebApi.DTOs.RequestModels;
using ECommerceAPI.WebApi.Validators;
using ECommerceAPI.Application.Models.Enums;

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
        var request = new ProductUpsertRequest { Name = "", Price = 10, Stock = 5, Media = new List<ProductMediaRequest> { new() { MediaUrl = "https://example.com/image.jpg", Type = MediaType.Image, OrderIndex = 1 } } };
        var result = _validator.TestValidate(request);
        result.ShouldHaveValidationErrorFor(r => r.Name);
    }

    [Fact]
    public void ShouldHaveError_WhenNameExceedsMaxLength()
    {
        var request = new ProductUpsertRequest { Name = new string('A', 101), Price = 10, Stock = 5, Media = new List<ProductMediaRequest> { new() { MediaUrl = "https://example.com/image.jpg", Type = MediaType.Image, OrderIndex = 1 } } };
        var result = _validator.TestValidate(request);
        result.ShouldHaveValidationErrorFor(r => r.Name);
    }

    // ✅ Price Validation Tests
    [Fact]
    public void ShouldHaveError_WhenPriceIsNegative()
    {
        var request = new ProductUpsertRequest { Name = "Product", Price = -1, Stock = 5, Media = new List<ProductMediaRequest> { new() { MediaUrl = "https://example.com/image.jpg", Type = MediaType.Image, OrderIndex = 1 } } };
        var result = _validator.TestValidate(request);
        result.ShouldHaveValidationErrorFor(r => r.Price);
    }

    [Fact]
    public void ShouldHaveError_WhenPriceIsZero()
    {
        var request = new ProductUpsertRequest { Name = "Product", Price = 0, Stock = 5, Media = new List<ProductMediaRequest> { new() { MediaUrl = "https://example.com/image.jpg", Type = MediaType.Image, OrderIndex = 1 } } };
        var result = _validator.TestValidate(request);
        result.ShouldHaveValidationErrorFor(r => r.Price);
    }

    // ✅ Stock Validation Tests
    [Fact]
    public void ShouldHaveError_WhenStockIsNegative()
    {
        var request = new ProductUpsertRequest { Name = "Product", Price = 10, Stock = -5, Media = new List<ProductMediaRequest> { new() { MediaUrl = "https://example.com/image.jpg", Type = MediaType.Image, OrderIndex = 1 } } };
        var result = _validator.TestValidate(request);
        result.ShouldHaveValidationErrorFor(r => r.Stock);
    }

    // ✅ Description Validation Tests
    [Fact]
    public void ShouldHaveError_WhenDescriptionExceedsMaxLength()
    {
        var request = new ProductUpsertRequest
        {
            Name = "Product",
            Price = 10,
            Stock = 5,
            Description = new string('D', 1001),
            Media = new List<ProductMediaRequest> { new() { MediaUrl = "https://example.com/image.jpg", Type = MediaType.Image, OrderIndex = 1 } }
        };
        var result = _validator.TestValidate(request);
        result.ShouldHaveValidationErrorFor(r => r.Description);
    }

    [Fact]
    public void ShouldNotHaveError_WhenDescriptionIsEmpty()
    {
        var request = new ProductUpsertRequest { Name = "Product", Price = 10, Stock = 5, Description = "", Media = new List<ProductMediaRequest> { new() { MediaUrl = "https://example.com/image.jpg", Type = MediaType.Image, OrderIndex = 1 } } };
        var result = _validator.TestValidate(request);
        result.ShouldNotHaveValidationErrorFor(r => r.Description);
    }

    // ✅ Media Validation Tests
    [Fact]
    public void ShouldHaveError_WhenMediaIsEmpty()
    {
        var request = new ProductUpsertRequest { Name = "Product", Price = 10, Stock = 5, Media = new List<ProductMediaRequest>() };
        var result = _validator.TestValidate(request);
        result.ShouldHaveValidationErrorFor(r => r.Media);
    }

    [Fact]
    public void ShouldHaveError_WhenMediaHasInvalidUrl()
    {
        var request = new ProductUpsertRequest
        {
            Name = "Product",
            Price = 10,
            Stock = 5,
            Media = new List<ProductMediaRequest>
            {
                new() { MediaUrl = "invalid-url", Type = MediaType.Image, OrderIndex = 1 }
            }
        };
        var result = _validator.TestValidate(request);
        result.ShouldHaveValidationErrorFor(r => r.Media)
            .WithErrorMessage("One or more media entries are invalid.");
    }

    [Fact]
    public void ShouldHaveError_WhenMediaHasInvalidMediaType()
    {
        var request = new ProductUpsertRequest
        {
            Name = "Product",
            Price = 10,
            Stock = 5,
            Media = new List<ProductMediaRequest>
            {
                new() { MediaUrl = "https://example.com/image.jpg", Type = (MediaType)99, OrderIndex = 1 }
            }
        };
        var result = _validator.TestValidate(request);
        result.ShouldHaveValidationErrorFor(r => r.Media)
            .WithErrorMessage("One or more media entries are invalid.");
    }

    [Fact]
    public void ShouldHaveError_WhenMediaOrderIndexesAreNotSequential()
    {
        var request = new ProductUpsertRequest
        {
            Name = "Product",
            Price = 10,
            Stock = 5,
            Media = new List<ProductMediaRequest>
            {
                new() { MediaUrl = "https://example.com/image1.jpg", Type = MediaType.Image, OrderIndex = 1 },
                new() { MediaUrl = "https://example.com/image2.jpg", Type = MediaType.Image, OrderIndex = 3 } // ❌ Skips index 2
            }
        };
        var result = _validator.TestValidate(request);
        result.ShouldHaveValidationErrorFor(r => r.Media);
    }

    [Fact]
    public void ShouldHaveError_WhenMediaOrderIndexDoesNotStartFromOne()
    {
        var request = new ProductUpsertRequest
        {
            Name = "Product",
            Price = 10,
            Stock = 5,
            Media = new List<ProductMediaRequest>
            {
                new() { MediaUrl = "https://example.com/image1.jpg", Type = MediaType.Image, OrderIndex = 2 }, // ❌ Starts at 2
                new() { MediaUrl = "https://example.com/image2.jpg", Type = MediaType.Image, OrderIndex = 3 }
            }
        };
        var result = _validator.TestValidate(request);
        result.ShouldHaveValidationErrorFor(r => r.Media);
    }

    [Fact]
    public void ShouldNotHaveError_WhenMediaIsValid()
    {
        var request = new ProductUpsertRequest
        {
            Name = "Valid Product",
            Price = 20.99m,
            Stock = 15,
            Description = "A great product",
            Media = new List<ProductMediaRequest>
            {
                new() { MediaUrl = "https://example.com/image1.jpg", Type = MediaType.Image, OrderIndex = 1 },
                new() { MediaUrl = "https://example.com/image2.jpg", Type = MediaType.Image, OrderIndex = 2 },
                new() { MediaUrl = "https://example.com/video.mp4", Type = MediaType.Video, OrderIndex = 3 }
            }
        };
        var result = _validator.TestValidate(request);
        result.ShouldNotHaveAnyValidationErrors();
    }
}