using ECommerceAPI.Infrastructure.Context;
using ECommerceAPI.Infrastructure.Entities;
using ECommerceAPI.Infrastructure.Repositories;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;

namespace ECommerceAPI.Tests.Infrastructure.Repositories;

public class UserRepositoryTests
{
    private readonly AppDbContext _dbContext;
    private readonly UserRepository _userRepository;

    public UserRepositoryTests()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: "TestDb_UserRepo")
            .Options;

        _dbContext = new AppDbContext(options);
        _userRepository = new UserRepository(_dbContext);

        _dbContext.Database.EnsureDeleted();
        _dbContext.Database.EnsureCreated();

        SeedDatabase();
    }

    private void SeedDatabase()
    {
        var users = new List<UserEntity>
        {
            new UserEntity { Id = 1, FullName = "User One", Email = "user1@example.com", PasswordHash = "hashedPassword1", Role = "Customer" },
            new UserEntity { Id = 2, FullName = "User Two", Email = "user2@example.com", PasswordHash = "hashedPassword2", Role = "Admin" }
        };

        _dbContext.Users.AddRange(users);
        _dbContext.SaveChanges();
    }

    [Fact]
    public async Task GetByIdAsync_ShouldReturnUser_WhenUserExists()
    {
        // Act
        var result = await _userRepository.GetByIdAsync(1);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().Be(1);
        result.Email.Should().Be("user1@example.com");
    }

    [Fact]
    public async Task GetByIdAsync_ShouldReturnNull_WhenUserDoesNotExist()
    {
        // Act
        var result = await _userRepository.GetByIdAsync(99);

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task GetByEmailAsync_ShouldReturnUser_WhenEmailExists()
    {
        // Act
        var result = await _userRepository.GetByEmailAsync("user1@example.com");

        // Assert
        result.Should().NotBeNull();
        result.Email.Should().Be("user1@example.com");
    }

    [Fact]
    public async Task GetByEmailAsync_ShouldReturnNull_WhenEmailDoesNotExist()
    {
        // Act
        var result = await _userRepository.GetByEmailAsync("nonexistent@example.com");

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task GetAllAsync_ShouldReturnAllUsers()
    {
        // Act
        var result = await _userRepository.GetAllAsync();

        // Assert
        result.Should().NotBeEmpty();
        result.Should().HaveCount(2);
    }

    [Fact]
    public async Task CreateAsync_ShouldAddNewUser()
    {
        // Arrange
        var newUser = new UserEntity { Id = 3, FullName = "User Three", Email = "user3@example.com", PasswordHash = "hashedPassword3", Role = "Customer" };

        // Act
        await _userRepository.CreateAsync(newUser);
        var result = await _userRepository.GetByIdAsync(3);

        // Assert
        result.Should().NotBeNull();
        result.Email.Should().Be("user3@example.com");
    }
}