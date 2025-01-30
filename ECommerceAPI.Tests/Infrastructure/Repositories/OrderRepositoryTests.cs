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
        var orders = new List<OrderEntity>
        {
            new OrderEntity { Id = 1, UserId = 1, TotalAmount = 100, PaymentStatus = "Pending", ShippingAddress = "Test Address" },
            new OrderEntity { Id = 2, UserId = 1, TotalAmount = 200, PaymentStatus = "Pending", ShippingAddress = "Test Address" },
            new OrderEntity { Id = 3, UserId = 2, TotalAmount = 300, PaymentStatus = "Pending", ShippingAddress = "Test Address" }
        };

        _dbContext.Orders.AddRange(orders);
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