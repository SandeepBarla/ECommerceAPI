using FluentValidation.TestHelper;
using ECommerceAPI.WebApi.DTOs.RequestModels;
using ECommerceAPI.WebApi.Validators;
namespace ECommerceAPI.Tests.WebApi.Validators;
public class UserRegisterRequestValidatorTests
{
    private readonly UserRegisterRequestValidator _validator;

    public UserRegisterRequestValidatorTests()
    {
        _validator = new UserRegisterRequestValidator();
    }

    [Fact]
    public void ShouldPass_WhenValidRequest()
    {
        var request = new UserRegisterRequest
        {
            FullName = "John Doe",
            Email = "john.doe@example.com",
            Password = "SecurePassword123!"
        };

        var result = _validator.TestValidate(request);
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void ShouldFail_WhenFullNameIsEmpty()
    {
        var request = new UserRegisterRequest
        {
            FullName = "",
            Email = "john.doe@example.com",
            Password = "SecurePassword123!"
        };

        var result = _validator.TestValidate(request);
        result.ShouldHaveValidationErrorFor(r => r.FullName);
    }

    [Fact]
    public void ShouldFail_WhenEmailIsInvalid()
    {
        var request = new UserRegisterRequest
        {
            FullName = "John Doe",
            Email = "invalid-email",
            Password = "SecurePassword123!"
        };

        var result = _validator.TestValidate(request);
        result.ShouldHaveValidationErrorFor(r => r.Email);
    }

    [Fact]
    public void ShouldFail_WhenPasswordIsEmpty()
    {
        var request = new UserRegisterRequest
        {
            FullName = "John Doe",
            Email = "john.doe@example.com",
            Password = ""
        };

        var result = _validator.TestValidate(request);
        result.ShouldHaveValidationErrorFor(r => r.Password);
    }
}