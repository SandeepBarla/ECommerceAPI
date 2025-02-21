using Xunit;
using FluentValidation.TestHelper;
using ECommerceAPI.WebApi.DTOs.RequestModels;
using ECommerceAPI.WebApi.Validators;
namespace ECommerceAPI.Tests.WebApi.Validators;
public class OrderStatusUpdateRequestValidatorTests
{
    private readonly OrderStatusUpdateRequestValidator _validator;

    public OrderStatusUpdateRequestValidatorTests()
    {
        _validator = new OrderStatusUpdateRequestValidator();
    }

    [Fact]
    public void ShouldHaveError_WhenStatusIsInvalid()
    {
        var request = new OrderStatusUpdateRequest { Status = "InvalidStatus" };
        var result = _validator.TestValidate(request);
        result.ShouldHaveValidationErrorFor(r => r.Status);
    }

    [Fact]
    public void ShouldNotHaveError_WhenStatusIsValid()
    {
        var request = new OrderStatusUpdateRequest { Status = "Shipped" };
        var result = _validator.TestValidate(request);
        result.ShouldNotHaveAnyValidationErrors();
    }
}