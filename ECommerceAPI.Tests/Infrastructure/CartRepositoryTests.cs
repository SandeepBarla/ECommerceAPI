using Microsoft.EntityFrameworkCore;
using ECommerceAPI.Infrastructure.Context;
using ECommerceAPI.Infrastructure.Repositories;
using ECommerceAPI.Infrastructure.Entities;
using FluentAssertions;

public class CartRepositoryTests
{
    private readonly AppDbContext _dbContext;
    private readonly CartRepository _cartRepository;

    public CartRepositoryTests()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: "TestDb")
            .Options;

        _dbContext = new AppDbContext(options);
        _cartRepository = new CartRepository(_dbContext);
    }

    [Fact]
    public async Task GetCartByUserIdAsync_ShouldReturnCart_WhenCartExists()
    {
        // Arrange
        var userId = 1;

        // First, add the user entity to the database
        var user = new UserEntity { Id = userId, FullName = "Test User", Email = "test@example.com", PasswordHash = "hashedpassword" };
        _dbContext.Users.Add(user);
        await _dbContext.SaveChangesAsync();

        // Now add the cart with a reference to the existing user
        var cart = new CartEntity
        {
            UserId = userId,
            User = user, // Ensure user is set
            CartItems = new List<CartItemEntity>()
        };

        _dbContext.Carts.Add(cart);
        await _dbContext.SaveChangesAsync();

        // Act
        var result = await _cartRepository.GetCartByUserIdAsync(userId);

        // Assert
        result.Should().NotBeNull();
        result.UserId.Should().Be(userId);
    }

    [Fact]
    public async Task GetCartByUserIdAsync_ShouldReturnNull_WhenCartDoesNotExist()
    {
        // Act
        var result = await _cartRepository.GetCartByUserIdAsync(999); // Non-existing user

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task SaveCartAsync_ShouldAddCart_WhenNewCartIsCreated()
    {
        // Arrange
        var cart = new CartEntity { UserId = 2, CartItems = new List<CartItemEntity>() };

        // Act
        await _cartRepository.SaveCartAsync(cart);
        var result = await _dbContext.Carts.FirstOrDefaultAsync(c => c.UserId == 2);

        // Assert
        result.Should().NotBeNull();
        result.UserId.Should().Be(2);
    }

    [Fact]
    public async Task RemoveCartAsync_ShouldDeleteCart_WhenCartExists()
    {
        // Arrange
        var cart = new CartEntity { UserId = 3 };
        _dbContext.Carts.Add(cart);
        await _dbContext.SaveChangesAsync();

        // Act
        await _cartRepository.RemoveCartAsync(cart);
        var result = await _dbContext.Carts.FirstOrDefaultAsync(c => c.UserId == 3);

        // Assert
        result.Should().BeNull();
    }
}