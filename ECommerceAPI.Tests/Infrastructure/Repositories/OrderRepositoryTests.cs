using ECommerceAPI.Infrastructure.Context;
using ECommerceAPI.Infrastructure.Entities;
using ECommerceAPI.Infrastructure.Repositories;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;

namespace ECommerceAPI.Tests.Infrastructure.Repositories;

public class OrderRepositoryTests
{
    private readonly AppDbContext _dbContext;
    private readonly OrderRepository _orderRepository;

    public OrderRepositoryTests()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: "TestDb")
            .Options;

        _dbContext = new AppDbContext(options);
        _orderRepository = new OrderRepository(_dbContext);

        _dbContext.Database.EnsureDeleted();
        _dbContext.Database.EnsureCreated();

        SeedDatabase();
    }

    private void SeedDatabase()
    {
        // Add users first
        var users = new List<UserEntity>
        {
            new UserEntity { Id = 1, FullName = "Test User 1", Email = "test1@example.com", PasswordHash = "hash1", Role = "User" },
            new UserEntity { Id = 2, FullName = "Test User 2", Email = "test2@example.com", PasswordHash = "hash2", Role = "User" }
        };
        _dbContext.Users.AddRange(users);

        // Add addresses
        var addresses = new List<AddressEntity>
        {
            new AddressEntity { Id = 1, UserId = 1, Name = "Home", Street = "123 Test St", City = "TestCity", State = "TestState", Pincode = "12345", Phone = "1234567890", IsDefault = true },
            new AddressEntity { Id = 2, UserId = 2, Name = "Home", Street = "456 Test Ave", City = "TestCity", State = "TestState", Pincode = "67890", Phone = "0987654321", IsDefault = true }
        };
        _dbContext.Addresses.AddRange(addresses);

        // Add categories and sizes (required for products)
        var categories = new List<CategoryEntity>
        {
            new CategoryEntity { Id = 1, Name = "Test Category" }
        };
        _dbContext.Categories.AddRange(categories);

        var sizes = new List<SizeEntity>
        {
            new SizeEntity { Id = 1, Name = "Test Size", SortOrder = 1 }
        };
        _dbContext.Sizes.AddRange(sizes);

        // Add products (required for order products)
        var products = new List<ProductEntity>
        {
            new ProductEntity { Id = 1, Name = "Test Product 1", Description = "Test Description", OriginalPrice = 50, CategoryId = 1, SizeId = 1 },
            new ProductEntity { Id = 2, Name = "Test Product 2", Description = "Test Description", OriginalPrice = 75, CategoryId = 1, SizeId = 1 }
        };
        _dbContext.Products.AddRange(products);

        var orders = new List<OrderEntity>
        {
            new OrderEntity { Id = 1, UserId = 1, TotalAmount = 100, PaymentStatus = "Pending", OrderStatus = "Pending", TrackingNumber = "TRACK001", AddressId = 1, OrderDate = DateTime.UtcNow },
            new OrderEntity { Id = 2, UserId = 1, TotalAmount = 200, PaymentStatus = "Pending", OrderStatus = "Processing", TrackingNumber = "TRACK002", AddressId = 1, OrderDate = DateTime.UtcNow },
            new OrderEntity { Id = 3, UserId = 2, TotalAmount = 300, PaymentStatus = "Pending", OrderStatus = "Shipped", TrackingNumber = "TRACK003", AddressId = 2, OrderDate = DateTime.UtcNow }
        };
        _dbContext.Orders.AddRange(orders);

        // Add order products
        var orderProducts = new List<OrderProductEntity>
        {
            new OrderProductEntity { OrderId = 1, ProductId = 1, Quantity = 2 },
            new OrderProductEntity { OrderId = 2, ProductId = 2, Quantity = 1 },
            new OrderProductEntity { OrderId = 3, ProductId = 1, Quantity = 3 }
        };
        _dbContext.OrderProducts.AddRange(orderProducts);

        _dbContext.SaveChanges();
    }

    [Fact]
    public async Task GetOrderByIdAsync_ShouldReturnOrder_WhenOrderExists()
    {
        // Act
        var result = await _orderRepository.GetOrderByIdAsync(1);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().Be(1);
        result.TotalAmount.Should().Be(100);
    }

    [Fact]
    public async Task GetOrderByIdAsync_ShouldReturnNull_WhenOrderDoesNotExist()
    {
        // Act
        var result = await _orderRepository.GetOrderByIdAsync(99);

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task GetOrdersByUserIdAsync_ShouldReturnOrders_WhenUserHasOrders()
    {
        // Act
        var result = await _orderRepository.GetOrdersByUserIdAsync(1);

        // Assert
        result.Should().NotBeEmpty();
        result.Should().HaveCount(2);
    }

    [Fact]
    public async Task GetOrdersByUserIdAsync_ShouldReturnEmptyList_WhenUserHasNoOrders()
    {
        // Act
        var result = await _orderRepository.GetOrdersByUserIdAsync(99);

        // Assert
        result.Should().BeEmpty();
    }

    [Fact]
    public async Task GetAllOrdersAsync_ShouldReturnAllOrders()
    {
        // Act
        var result = await _orderRepository.GetAllOrdersAsync();

        // Assert
        result.Should().NotBeEmpty();
        result.Should().HaveCount(3);
    }

    [Fact]
    public async Task UpdateOrderAsync_ShouldModifyExistingOrder()
    {
        // Arrange
        var order = await _orderRepository.GetOrderByIdAsync(1);
        order.TotalAmount = 500;

        // Act
        await _orderRepository.UpdateOrderAsync(order);
        var updatedOrder = await _orderRepository.GetOrderByIdAsync(1);

        // Assert
        updatedOrder.TotalAmount.Should().Be(500);
    }
}