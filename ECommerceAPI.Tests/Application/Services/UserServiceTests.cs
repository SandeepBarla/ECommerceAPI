using Moq;
using ECommerceAPI.Application.Interfaces;
using ECommerceAPI.Application.Models;
using ECommerceAPI.Application.Services;
using ECommerceAPI.Infrastructure.Entities;
using ECommerceAPI.Infrastructure.Repositories.Interfaces;
using ECommerceAPI.Tests.Common;
using FluentAssertions;
namespace ECommerceAPI.Tests.Application.Services;
public class UserServiceTests : TestBase
{
    private readonly UserService _userService;
    private readonly Mock<IUserRepository> _userRepositoryMock;
    private readonly Mock<ITokenService> _tokenServiceMock;

    public UserServiceTests()
    {
        _userRepositoryMock = new Mock<IUserRepository>();
        _tokenServiceMock = new Mock<ITokenService>();

        // Use real AutoMapper instance from TestBase
        _userService = new UserService(
            _userRepositoryMock.Object,
            _tokenServiceMock.Object,
            Mapper
        );
    }

    // Test: Register User Successfully
    [Fact]
    public async Task RegisterUserAsync_ShouldCreateUser_WhenValidRequest()
    {
        // Arrange
        var password = "password123";
        var user = new User
        {
            FullName = "John Doe",
            Email = "john@example.com",
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(password)
        };

        _userRepositoryMock.Setup(repo => repo.GetByEmailAsync(user.Email))
            .ReturnsAsync((UserEntity)null); // Ensure user doesn't already exist
        _userRepositoryMock.Setup(repo => repo.CreateAsync(It.IsAny<UserEntity>()))
            .Returns(Task.CompletedTask); // Ensure creation succeeds

        // Act
        await _userService.RegisterUserAsync(user, password);

        // Assert
        _userRepositoryMock.Verify(repo => repo.CreateAsync(It.IsAny<UserEntity>()), Times.Once);
        _userRepositoryMock.Verify(repo => repo.GetByEmailAsync(user.Email), Times.Once);
    }

    // Test: Login User Successfully
    [Fact]
    public async Task ValidateUserCredentialsAsync_ShouldReturnUser_WhenCredentialsAreValid()
    {
        // Arrange
        var userEntity = new UserEntity { Id = 1, Email = "john@example.com", PasswordHash = BCrypt.Net.BCrypt.HashPassword("password123") };

        _userRepositoryMock.Setup(repo => repo.GetByEmailAsync(userEntity.Email)).ReturnsAsync(userEntity);

        // Act
        var result = await _userService.ValidateUserCredentialsAsync(userEntity.Email, "password123");

        // Assert
        result.Should().NotBeNull();
        result.Email.Should().Be(userEntity.Email);
        _userRepositoryMock.Verify(repo => repo.GetByEmailAsync(userEntity.Email), Times.Once);
    }

    // Test: Login Should Fail for Invalid Credentials
    [Fact]
    public async Task ValidateUserCredentialsAsync_ShouldThrowUnauthorizedAccessException_WhenCredentialsAreInvalid()
    {
        // Arrange
        var userEntity = new UserEntity { Email = "john@example.com", PasswordHash = BCrypt.Net.BCrypt.HashPassword("password123") };
        _userRepositoryMock.Setup(repo => repo.GetByEmailAsync(userEntity.Email)).ReturnsAsync(userEntity);

        // Act & Assert
        await Assert.ThrowsAsync<UnauthorizedAccessException>(
            async () => await _userService.ValidateUserCredentialsAsync(userEntity.Email, "wrongpassword")
        );
    }

    // Test: Get All Users
    [Fact]
    public async Task GetAllUsersAsync_ShouldReturnListOfUsers()
    {
        // Arrange
        var userEntities = new List<UserEntity>
        {
            new UserEntity { Id = 1, FullName = "John Doe", Email = "john@example.com" },
            new UserEntity { Id = 2, FullName = "Jane Doe", Email = "jane@example.com" }
        };
        
        _userRepositoryMock.Setup(repo => repo.GetAllAsync()).ReturnsAsync(userEntities);

        // Act
        var result = await _userService.GetAllUsersAsync();

        // Assert
        result.Should().NotBeNull();
        result.Should().HaveCount(2);
        _userRepositoryMock.Verify(repo => repo.GetAllAsync(), Times.Once);
    }
    
    // Test: Get User by Id Should Throw KeyNotFoundException if Not Found
    [Fact]
    public async Task GetUserByIdAsync_ShouldThrowKeyNotFoundException_WhenUserDoesNotExist()
    {
        // Arrange
        _userRepositoryMock.Setup(repo => repo.GetByIdAsync(It.IsAny<int>())).ReturnsAsync((UserEntity)null);

        // Act
        var act = async () => await _userService.GetUserByIdAsync(10);

        // Assert

        await act.Should().ThrowAsync<KeyNotFoundException>()
            .WithMessage("User not found.");
    }
}