using Moq;
using Microsoft.AspNetCore.Mvc;
using ECommerceAPI.WebApi.Controllers;
using ECommerceAPI.Application.Interfaces;
using ECommerceAPI.WebApi.DTOs.RequestModels;
using ECommerceAPI.WebApi.DTOs.ResponseModels;
using FluentValidation;
using FluentValidation.Results;
using ECommerceAPI.Application.Models;

public class AuthControllerTests
{
    private readonly Mock<IUserService> _userServiceMock;
    private readonly Mock<ITokenService> _tokenServiceMock;
    private readonly Mock<IValidator<UserLoginRequest>> _loginValidatorMock;
    private readonly AuthController _controller;

    public AuthControllerTests()
    {
        _userServiceMock = new Mock<IUserService>();
        _tokenServiceMock = new Mock<ITokenService>();
        _loginValidatorMock = new Mock<IValidator<UserLoginRequest>>();

        _controller = new AuthController(_userServiceMock.Object, _tokenServiceMock.Object, _loginValidatorMock.Object);
    }

    [Fact]
    public async Task Login_ShouldReturnToken_WhenCredentialsAreValid()
    {
        // Arrange
        var request = new UserLoginRequest { Email = "test@example.com", Password = "Password123" };
        var user = new User { Id = 1, FullName = "Test User", Email = request.Email, Role = "Customer" };
        var token = "valid_token";

        _loginValidatorMock.Setup(v => v.ValidateAsync(request, default))
            .ReturnsAsync(new ValidationResult());

        _userServiceMock.Setup(svc => svc.ValidateUserCredentialsAsync(request.Email, request.Password))
            .ReturnsAsync(user);

        _tokenServiceMock.Setup(ts => ts.GenerateToken(user.Id, user.Email, user.Role))
            .Returns(token);

        // Act
        var result = await _controller.Login(request);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var response = Assert.IsType<AuthResponse>(okResult.Value);
        Assert.Equal("valid_token", response.Token);
        Assert.Equal("Customer", response.Role);
    }

    [Fact]
    public async Task Login_ShouldReturnUnauthorized_WhenInvalidCredentials()
    {
        // Arrange
        var request = new UserLoginRequest { Email = "wrong@example.com", Password = "WrongPassword" };

        _loginValidatorMock.Setup(v => v.ValidateAsync(request, default))
            .ReturnsAsync(new ValidationResult());

        _userServiceMock.Setup(svc => svc.ValidateUserCredentialsAsync(request.Email, request.Password))
            .ThrowsAsync(new UnauthorizedAccessException("Invalid credentials."));

        // Act
        var exception = await Assert.ThrowsAsync<UnauthorizedAccessException>(
            async () => await _controller.Login(request)
        );

        // Assert
        Assert.Equal("Invalid credentials.", exception.Message);
    }
}