using Xunit;
using FluentValidation.TestHelper;
using ECommerceAPI.WebApi.DTOs.RequestModels;
using ECommerceAPI.WebApi.Validators;

public class ProductUpsertRequestValidatorTests
{
    private readonly ProductUpsertRequestValidator _validator;

    public ProductUpsertRequestValidatorTests()
    {
        _validator = new ProductUpsertRequestValidator();
    }

    [Fact]
    public void ShouldHaveError_WhenNameIsEmpty()
    {
        var request = new ProductUpsertRequest { Name = "", Price = 10, Stock = 5 };
        var result = _validator.TestValidate(request);
        result.ShouldHaveValidationErrorFor(r => r.Name);
    }

    [Fact]
    public void ShouldHaveError_WhenNameExceedsMaxLength()
    {
        var request = new ProductUpsertRequest { Name = new string('A', 101), Price = 10, Stock = 5 };
        var result = _validator.TestValidate(request);
        result.ShouldHaveValidationErrorFor(r => r.Name);
    }

    [Fact]
    public void ShouldHaveError_WhenPriceIsNegative()
    {
        var request = new ProductUpsertRequest { Name = "Product", Price = -1, Stock = 5 };
        var result = _validator.TestValidate(request);
        result.ShouldHaveValidationErrorFor(r => r.Price);
    }

    [Fact]
    public void ShouldHaveError_WhenPriceIsZero()
    {
        var request = new ProductUpsertRequest { Name = "Product", Price = 0, Stock = 5 };
        var result = _validator.TestValidate(request);
        result.ShouldHaveValidationErrorFor(r => r.Price);
    }

    [Fact]
    public void ShouldHaveError_WhenStockIsNegative()
    {
        var request = new ProductUpsertRequest { Name = "Product", Price = 10, Stock = -5 };
        var result = _validator.TestValidate(request);
        result.ShouldHaveValidationErrorFor(r => r.Stock);
    }

    [Fact]
    public void ShouldHaveError_WhenDescriptionExceedsMaxLength()
    {
        var request = new ProductUpsertRequest
        {
            Name = "Product",
            Price = 10,
            Stock = 5,
            Description = new string('D', 501)
        };
        var result = _validator.TestValidate(request);
        result.ShouldHaveValidationErrorFor(r => r.Description);
    }

    [Fact]
    public void ShouldNotHaveError_WhenDescriptionIsEmpty()
    {
        var request = new ProductUpsertRequest { Name = "Product", Price = 10, Stock = 5, Description = "" };
        var result = _validator.TestValidate(request);
        result.ShouldNotHaveValidationErrorFor(r => r.Description);
    }

    [Fact]
    public void ShouldHaveError_WhenImageUrlIsInvalid()
    {
        var request = new ProductUpsertRequest
        {
            Name = "Product",
            Price = 10,
            Stock = 5,
            ImageUrl = "invalid-url"
        };
        var result = _validator.TestValidate(request);
        result.ShouldHaveValidationErrorFor(r => r.ImageUrl);
    }

    [Fact]
    public void ShouldNotHaveError_WhenImageUrlIsValid()
    {
        var request = new ProductUpsertRequest
        {
            Name = "Product",
            Price = 10,
            Stock = 5,
            ImageUrl = "https://example.com/image.png"
        };
        var result = _validator.TestValidate(request);
        result.ShouldNotHaveValidationErrorFor(r => r.ImageUrl);
    }

    [Fact]
    public void ShouldNotHaveError_WhenImageUrlIsEmpty()
    {
        var request = new ProductUpsertRequest { Name = "Product", Price = 10, Stock = 5, ImageUrl = "" };
        var result = _validator.TestValidate(request);
        result.ShouldNotHaveValidationErrorFor(r => r.ImageUrl);
    }

    [Fact]
    public void ShouldNotHaveError_WhenValidRequest()
    {
        var request = new ProductUpsertRequest
        {
            Name = "Valid Product",
            Price = 20.99m,
            Stock = 15,
            Description = "A great product",
            ImageUrl = "https://example.com/product.jpg"
        };
        var result = _validator.TestValidate(request);
        result.ShouldNotHaveAnyValidationErrors();
    }
}