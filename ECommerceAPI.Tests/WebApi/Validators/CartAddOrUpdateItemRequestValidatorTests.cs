using Xunit;
using FluentValidation.TestHelper;
using ECommerceAPI.WebApi.DTOs.RequestModels;
using ECommerceAPI.WebApi.Validators;
namespace ECommerceAPI.Tests.WebApi.Validators;
public class CartAddOrUpdateItemRequestValidatorTests
{
    private readonly CartAddOrUpdateItemRequestValidator _validator;

    public CartAddOrUpdateItemRequestValidatorTests()
    {
        _validator = new CartAddOrUpdateItemRequestValidator();
    }

    [Fact]
    public void ShouldHaveError_WhenProductIdIsZero()
    {
        var request = new CartAddOrUpdateItemRequest { ProductId = 0, Quantity = 2 };
        var result = _validator.TestValidate(request);
        result.ShouldHaveValidationErrorFor(r => r.ProductId)
            .WithErrorMessage("Product ID must be greater than zero.");
    }

    [Fact]
    public void ShouldHaveError_WhenQuantityIsNegative()
    {
        var request = new CartAddOrUpdateItemRequest { ProductId = 1, Quantity = -1 };
        var result = _validator.TestValidate(request);
        result.ShouldHaveValidationErrorFor(r => r.Quantity)
            .WithErrorMessage("Quantity cannot be negative.");
    }

    [Fact]
    public void ShouldNotHaveError_WhenValidRequest()
    {
        var request = new CartAddOrUpdateItemRequest { ProductId = 1, Quantity = 2 };
        var result = _validator.TestValidate(request);
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void ShouldAllowQuantityZero_ForItemRemoval()
    {
        var request = new CartAddOrUpdateItemRequest { ProductId = 1, Quantity = 0 };
        var result = _validator.TestValidate(request);
        result.ShouldNotHaveAnyValidationErrors(); // Ensure zero quantity is allowed for removal
    }
}