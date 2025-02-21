using Xunit;
using FluentValidation.TestHelper;
using ECommerceAPI.WebApi.DTOs.RequestModels;
using ECommerceAPI.WebApi.Validators;
namespace ECommerceAPI.Tests.WebApi.Validators;
public class UserLoginRequestValidatorTests
{
    private readonly UserLoginRequestValidator _validator;

    public UserLoginRequestValidatorTests()
    {
        _validator = new UserLoginRequestValidator();
    }

    [Fact]
    public void ShouldPass_WhenValidRequest()
    {
        var request = new UserLoginRequest
        {
            Email = "john.doe@example.com",
            Password = "SecurePassword123!"
        };

        var result = _validator.TestValidate(request);
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void ShouldFail_WhenEmailIsEmpty()
    {
        var request = new UserLoginRequest
        {
            Email = "",
            Password = "SecurePassword123!"
        };

        var result = _validator.TestValidate(request);
        result.ShouldHaveValidationErrorFor(r => r.Email);
    }

    [Fact]
    public void ShouldFail_WhenEmailIsInvalid()
    {
        var request = new UserLoginRequest
        {
            Email = "invalid-email",
            Password = "SecurePassword123!"
        };

        var result = _validator.TestValidate(request);
        result.ShouldHaveValidationErrorFor(r => r.Email);
    }

    [Fact]
    public void ShouldFail_WhenPasswordIsEmpty()
    {
        var request = new UserLoginRequest
        {
            Email = "john.doe@example.com",
            Password = ""
        };

        var result = _validator.TestValidate(request);
        result.ShouldHaveValidationErrorFor(r => r.Password);
    }
}