using Moq;
using Microsoft.AspNetCore.Mvc;
using ECommerceAPI.WebApi.Controllers;
using ECommerceAPI.Application.Interfaces;
using ECommerceAPI.WebApi.DTOs.RequestModels;
using ECommerceAPI.WebApi.DTOs.ResponseModels;
using FluentValidation;
using FluentValidation.Results;
using ECommerceAPI.Application.Models;
using ECommerceAPI.Tests.Common;
namespace ECommerceAPI.Tests.WebApi.Controllers;

public class UserControllerTests : TestBase
{
    private readonly Mock<IUserService> _userServiceMock;
    private readonly Mock<ITokenService> _tokenServiceMock;
    private readonly Mock<IValidator<UserRegisterRequest>> _registerValidatorMock;
    private readonly Mock<IValidator<UserUpdateRequest>> _updateValidatorMock;
    private readonly UserController _controller;

    public UserControllerTests()
    {
        _userServiceMock = new Mock<IUserService>();
        _tokenServiceMock = new Mock<ITokenService>();
        _registerValidatorMock = new Mock<IValidator<UserRegisterRequest>>();
        _updateValidatorMock = new Mock<IValidator<UserUpdateRequest>>();
        _controller = new UserController(_userServiceMock.Object, _tokenServiceMock.Object, Mapper, _registerValidatorMock.Object, _updateValidatorMock.Object);
    }

    [Fact]
    public async Task RegisterUser_ShouldReturnToken_WhenValidRequest()
    {
        // Arrange
        var request = new UserRegisterRequest { FullName = "Test User", Email = "test@example.com", Password = "SecurePass123" };
        var user = new User { Id = 1, FullName = "Test User", Email = request.Email, Role = "Customer" };
        var token = "valid_token";

        _registerValidatorMock.Setup(v => v.ValidateAsync(request, default))
            .ReturnsAsync(new ValidationResult());

        _userServiceMock.Setup(svc => svc.RegisterUserAsync(It.IsAny<User>(), request.Password))
            .ReturnsAsync(user);

        _tokenServiceMock.Setup(ts => ts.GenerateToken(It.IsAny<int>(), It.IsAny<string>(), It.IsAny<string>()))
            .Returns(token);

        // Act
        var result = await _controller.RegisterUser(request);

        // Assert
        var createdResult = Assert.IsType<CreatedAtActionResult>(result);
        var response = Assert.IsType<AuthResponse>(createdResult.Value);
        Assert.Equal("valid_token", response.Token);
        Assert.Equal("Customer", response.Role);
    }

    [Fact]
    public async Task GetUserById_ShouldReturnUser_WhenUserExists()
    {
        // Arrange
        var userId = 1;
        var user = new User { Id = userId, FullName = "Test User", Email = "test@example.com", Role = "Customer" };

        _userServiceMock.Setup(svc => svc.GetUserByIdAsync(userId))
            .ReturnsAsync(user);

        // Act
        var result = await _controller.GetUserById(userId);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var returnedUser = Assert.IsType<UserResponse>(okResult.Value);
        Assert.Equal(userId, returnedUser.Id);
    }

    [Fact]
    public async Task GetUserById_ShouldReturnNotFound_WhenUserDoesNotExist()
    {
        // Arrange
        var userId = 999;

        _userServiceMock.Setup(svc => svc.GetUserByIdAsync(userId))
            .ThrowsAsync(new KeyNotFoundException("User not found."));

        // Act & Assert
        var exception = await Assert.ThrowsAsync<KeyNotFoundException>(
            async () => await _controller.GetUserById(userId)
        );

        Assert.Equal("User not found.", exception.Message);
    }

    [Fact]
    public async Task GetAllUsers_ShouldReturnListOfUsers()
    {
        // Arrange
        var users = new List<User>
        {
            new User { Id = 1, FullName = "User One", Email = "user1@example.com", Role = "Customer" },
            new User { Id = 2, FullName = "User Two", Email = "user2@example.com", Role = "Admin" }
        };

        _userServiceMock.Setup(svc => svc.GetAllUsersAsync()).ReturnsAsync(users);

        // Act
        var result = await _controller.GetAllUsers();

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var returnedUsers = Assert.IsType<List<UserResponse>>(okResult.Value);

        Assert.Equal(2, returnedUsers.Count);
    }
}