using Xunit;
using Moq;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using FluentAssertions;
using ECommerceAPI.Application.Interfaces;
using ECommerceAPI.Application.Models;
using ECommerceAPI.Application.Services;
using ECommerceAPI.Infrastructure.Entities;
using ECommerceAPI.Infrastructure.Interfaces;
using ECommerceAPI.Tests.Common;

namespace ECommerceAPI.Tests.Application.Services;

public class OrderServiceTests : TestBase
{
    private readonly Mock<IOrderRepository> _orderRepositoryMock;
    private readonly OrderService _orderService;

    public OrderServiceTests()
    {
        _orderRepositoryMock = new Mock<IOrderRepository>();

        _orderService = new OrderService(_orderRepositoryMock.Object, Mapper); // Use real AutoMapper from TestBase
    }

    [Fact]
    public async Task CreateOrder_ShouldCallRepository_WithMappedEntity()
    {
        var order = new Order { UserId = 1, TotalAmount = 100, OrderStatus = "Pending", PaymentStatus = "Pending", TrackingNumber = "TRACK001", OrderDate = DateTime.UtcNow };
        var expectedEntity = Mapper.Map<OrderEntity>(order);

        _orderRepositoryMock.Setup(repo => repo.CreateOrderAsync(It.IsAny<OrderEntity>())).ReturnsAsync(expectedEntity);

        await _orderService.CreateOrderAsync(order);

        _orderRepositoryMock.Verify(repo => repo.CreateOrderAsync(It.IsAny<OrderEntity>()), Times.Once);
    }

    [Fact]
    public async Task GetOrderById_ShouldReturnOrder_WhenOrderExists()
    {
        var orderEntity = new OrderEntity
        {
            Id = 1,
            UserId = 1,
            TotalAmount = 100,
            PaymentStatus = "Pending",
            OrderStatus = "Processing",
            TrackingNumber = "TRACK001",
            OrderDate = DateTime.UtcNow
        };

        _orderRepositoryMock.Setup(repo => repo.GetOrderByIdAsync(1)).ReturnsAsync(orderEntity);

        var result = await _orderService.GetOrderByIdAsync(1);

        result.Should().NotBeNull();
        result.Id.Should().Be(1);
        result.TotalAmount.Should().Be(100);
        result.PaymentStatus.Should().Be("Pending");
        result.OrderStatus.Should().Be("Processing");
    }

    [Fact]
    public async Task GetOrderById_ShouldThrowKeyNotFoundException_WhenOrderDoesNotExist()
    {
        _orderRepositoryMock.Setup(repo => repo.GetOrderByIdAsync(1)).ReturnsAsync((OrderEntity)null);

        await Assert.ThrowsAsync<KeyNotFoundException>(() => _orderService.GetOrderByIdAsync(1));
    }

    [Fact]
    public async Task UpdateOrderStatus_ShouldCallRepositoryUpdate_WhenOrderExists()
    {
        var orderEntity = new OrderEntity { Id = 1, UserId = 1, OrderStatus = "Pending" };

        _orderRepositoryMock.Setup(repo => repo.GetOrderByIdAsync(1)).ReturnsAsync(orderEntity);
        _orderRepositoryMock.Setup(repo => repo.UpdateOrderAsync(orderEntity)).Returns(Task.CompletedTask);

        await _orderService.UpdateOrderStatusAsync(1, "Shipped");

        _orderRepositoryMock.Verify(repo => repo.UpdateOrderAsync(It.IsAny<OrderEntity>()), Times.Once);
    }

    [Fact]
    public async Task UpdateOrderStatus_ShouldThrowKeyNotFoundException_WhenOrderDoesNotExist()
    {
        _orderRepositoryMock.Setup(repo => repo.GetOrderByIdAsync(1)).ReturnsAsync((OrderEntity)null);

        await Assert.ThrowsAsync<KeyNotFoundException>(() => _orderService.UpdateOrderStatusAsync(1, "Shipped"));
    }

    // ✅ NEW TESTS FOR PAYMENT STATUS FUNCTIONALITY
    [Fact]
    public async Task UpdatePaymentStatus_ShouldUpdateStatusAndRemarks_WhenOrderExists()
    {
        // Arrange
        var orderEntity = new OrderEntity
        {
            Id = 1,
            UserId = 1,
            PaymentStatus = "Pending",
            PaymentRemarks = null
        };

        _orderRepositoryMock.Setup(repo => repo.GetOrderByIdAsync(1)).ReturnsAsync(orderEntity);
        _orderRepositoryMock.Setup(repo => repo.UpdateOrderAsync(It.IsAny<OrderEntity>())).Returns(Task.CompletedTask);

        // Act
        await _orderService.UpdatePaymentStatusAsync(1, "Approved", "Payment verified");

        // Assert
        orderEntity.PaymentStatus.Should().Be("Approved");
        orderEntity.PaymentRemarks.Should().Be("Payment verified");
        _orderRepositoryMock.Verify(repo => repo.UpdateOrderAsync(orderEntity), Times.Once);
    }

    [Fact]
    public async Task UpdatePaymentStatus_ShouldUpdateStatusWithoutRemarks_WhenRemarksIsNull()
    {
        // Arrange
        var orderEntity = new OrderEntity { Id = 1, PaymentStatus = "Pending", PaymentRemarks = null };
        _orderRepositoryMock.Setup(repo => repo.GetOrderByIdAsync(1)).ReturnsAsync(orderEntity);
        _orderRepositoryMock.Setup(repo => repo.UpdateOrderAsync(orderEntity)).Returns(Task.CompletedTask);

        // Act
        await _orderService.UpdatePaymentStatusAsync(1, "Approved", null);

        // Assert
        Assert.Equal("Approved", orderEntity.PaymentStatus);
        Assert.Null(orderEntity.PaymentRemarks);
        _orderRepositoryMock.Verify(repo => repo.UpdateOrderAsync(orderEntity), Times.Once);
    }

    [Fact]
    public async Task UpdatePaymentStatus_ShouldThrowKeyNotFoundException_WhenOrderDoesNotExist()
    {
        // Arrange
        _orderRepositoryMock.Setup(repo => repo.GetOrderByIdAsync(999)).ReturnsAsync((OrderEntity)null);

        // Act & Assert
        await Assert.ThrowsAsync<KeyNotFoundException>(() =>
            _orderService.UpdatePaymentStatusAsync(999, "Approved", "Payment verified"));
    }

    [Fact]
    public async Task UpdatePaymentStatus_ShouldHandleRejectionWithRemarks()
    {
        // Arrange
        var orderEntity = new OrderEntity
        {
            Id = 1,
            UserId = 1,
            PaymentStatus = "Pending",
            PaymentRemarks = null
        };

        _orderRepositoryMock.Setup(repo => repo.GetOrderByIdAsync(1)).ReturnsAsync(orderEntity);
        _orderRepositoryMock.Setup(repo => repo.UpdateOrderAsync(It.IsAny<OrderEntity>())).Returns(Task.CompletedTask);

        // Act
        await _orderService.UpdatePaymentStatusAsync(1, "Rejected", "Invalid payment proof - image unclear");

        // Assert
        orderEntity.PaymentStatus.Should().Be("Rejected");
        orderEntity.PaymentRemarks.Should().Be("Invalid payment proof - image unclear");
        _orderRepositoryMock.Verify(repo => repo.UpdateOrderAsync(orderEntity), Times.Once);
    }

    // ✅ TESTS FOR MISSING FUNCTIONALITY
    [Fact]
    public async Task GetOrdersByUserIdAsync_ShouldReturnMappedOrders_WhenOrdersExist()
    {
        // Arrange
        var orderEntities = new List<OrderEntity>
        {
            new OrderEntity { Id = 1, UserId = 1, TotalAmount = 100, PaymentStatus = "Pending", OrderStatus = "Processing", TrackingNumber = "TRACK001", OrderDate = DateTime.UtcNow },
            new OrderEntity { Id = 2, UserId = 1, TotalAmount = 200, PaymentStatus = "Pending", OrderStatus = "Processing", TrackingNumber = "TRACK002", OrderDate = DateTime.UtcNow }
        };

        _orderRepositoryMock.Setup(repo => repo.GetOrdersByUserIdAsync(1)).ReturnsAsync(orderEntities);

        // Act
        var result = await _orderService.GetOrdersByUserIdAsync(1);

        // Assert
        result.Should().NotBeNull();
        result.Should().HaveCount(2);
        result[0].Id.Should().Be(1);
        result[1].Id.Should().Be(2);
    }

    [Fact]
    public async Task GetOrdersByUserIdAsync_ShouldReturnEmptyList_WhenNoOrdersExist()
    {
        // Arrange
        var emptyOrderEntities = new List<OrderEntity>();

        _orderRepositoryMock.Setup(repo => repo.GetOrdersByUserIdAsync(999)).ReturnsAsync(emptyOrderEntities);

        // Act
        var result = await _orderService.GetOrdersByUserIdAsync(999);

        // Assert
        result.Should().NotBeNull();
        result.Should().BeEmpty();
    }

    [Fact]
    public async Task GetAllOrdersAsync_ShouldReturnMappedOrders_WhenOrdersExist()
    {
        // Arrange
        var orderEntities = new List<OrderEntity>
        {
            new OrderEntity { Id = 1, UserId = 1, TotalAmount = 100, PaymentStatus = "Pending", OrderStatus = "Processing", TrackingNumber = "TRACK001", OrderDate = DateTime.UtcNow },
            new OrderEntity { Id = 2, UserId = 2, TotalAmount = 200, PaymentStatus = "Pending", OrderStatus = "Processing", TrackingNumber = "TRACK002", OrderDate = DateTime.UtcNow }
        };

        _orderRepositoryMock.Setup(repo => repo.GetAllOrdersAsync()).ReturnsAsync(orderEntities);

        // Act
        var result = await _orderService.GetAllOrdersAsync();

        // Assert
        result.Should().NotBeNull();
        result.Should().HaveCount(2);
    }

    [Fact]
    public async Task CreateOrder_ShouldReturnMappedOrder_AfterSuccessfulCreation()
    {
        // Arrange
        var order = new Order { UserId = 1, TotalAmount = 100, OrderStatus = "Processing", PaymentStatus = "Pending", TrackingNumber = "TRACK001", OrderDate = DateTime.UtcNow };
        var createdOrderEntity = new OrderEntity { Id = 1, UserId = 1, TotalAmount = 100, OrderStatus = "Processing", PaymentStatus = "Pending", TrackingNumber = "TRACK001", OrderDate = DateTime.UtcNow };

        _orderRepositoryMock.Setup(repo => repo.CreateOrderAsync(It.IsAny<OrderEntity>())).ReturnsAsync(createdOrderEntity);

        // Act
        var result = await _orderService.CreateOrderAsync(order);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().Be(1);
        result.UserId.Should().Be(1);
        result.TotalAmount.Should().Be(100);
    }

    [Fact]
    public async Task UpdateOrderStatus_ShouldUpdateCorrectOrderProperty()
    {
        // Arrange
        var orderEntity = new OrderEntity
        {
            Id = 1,
            UserId = 1,
            OrderStatus = "Processing",
            PaymentStatus = "Pending" // Should remain unchanged
        };

        _orderRepositoryMock.Setup(repo => repo.GetOrderByIdAsync(1)).ReturnsAsync(orderEntity);
        _orderRepositoryMock.Setup(repo => repo.UpdateOrderAsync(It.IsAny<OrderEntity>())).Returns(Task.CompletedTask);

        // Act
        await _orderService.UpdateOrderStatusAsync(1, "Shipped");

        // Assert
        orderEntity.OrderStatus.Should().Be("Shipped");
        orderEntity.PaymentStatus.Should().Be("Pending"); // Should remain unchanged
        _orderRepositoryMock.Verify(repo => repo.UpdateOrderAsync(orderEntity), Times.Once);
    }

    // ✅ COMPREHENSIVE EDGE CASE AND ERROR HANDLING TESTS
    [Fact]
    public async Task GetAllOrderEntitiesAsync_ShouldReturnEmptyList_WhenNoOrdersExist()
    {
        // Arrange
        var emptyOrderEntities = new List<OrderEntity>();
        _orderRepositoryMock.Setup(repo => repo.GetAllOrdersAsync()).ReturnsAsync(emptyOrderEntities);

        // Act
        var result = await _orderService.GetAllOrderEntitiesAsync();

        // Assert
        result.Should().NotBeNull();
        result.Should().BeEmpty();
    }

    [Fact]
    public async Task GetAllOrderEntitiesAsync_ShouldThrowException_WhenRepositoryThrows()
    {
        // Arrange
        _orderRepositoryMock.Setup(repo => repo.GetAllOrdersAsync())
            .ThrowsAsync(new InvalidOperationException("Database connection failed"));

        // Act & Assert
        var exception = await Assert.ThrowsAsync<InvalidOperationException>(
            async () => await _orderService.GetAllOrderEntitiesAsync()
        );

        Assert.Equal("Database connection failed", exception.Message);
    }

    [Fact]
    public async Task GetOrderEntityByIdAsync_ShouldReturnNull_WhenOrderDoesNotExist()
    {
        // Arrange
        _orderRepositoryMock.Setup(repo => repo.GetOrderByIdAsync(999)).ReturnsAsync((OrderEntity)null);

        // Act
        var result = await _orderService.GetOrderEntityByIdAsync(999);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task GetOrderEntityByIdAsync_ShouldThrowException_WhenRepositoryThrows()
    {
        // Arrange
        _orderRepositoryMock.Setup(repo => repo.GetOrderByIdAsync(1))
            .ThrowsAsync(new InvalidOperationException("Database error"));

        // Act & Assert
        var exception = await Assert.ThrowsAsync<InvalidOperationException>(
            async () => await _orderService.GetOrderEntityByIdAsync(1)
        );

        Assert.Equal("Database error", exception.Message);
    }

    [Fact]
    public async Task CreateOrder_ShouldThrowException_WhenRepositoryThrows()
    {
        // Arrange
        var order = new Order { TotalAmount = 100, UserId = 1, PaymentStatus = "Pending", OrderStatus = "Processing", TrackingNumber = "TRACK001", OrderDate = DateTime.UtcNow };

        _orderRepositoryMock.Setup(repo => repo.CreateOrderAsync(It.IsAny<OrderEntity>()))
            .ThrowsAsync(new InvalidOperationException("Database insert failed"));

        // Act & Assert
        var exception = await Assert.ThrowsAsync<InvalidOperationException>(
            async () => await _orderService.CreateOrderAsync(order)
        );

        Assert.Equal("Database insert failed", exception.Message);
    }

    [Fact]
    public async Task GetAllOrdersAsync_ShouldThrowException_WhenRepositoryThrows()
    {
        // Arrange
        _orderRepositoryMock.Setup(repo => repo.GetAllOrdersAsync())
            .ThrowsAsync(new InvalidOperationException("Database query failed"));

        // Act & Assert
        var exception = await Assert.ThrowsAsync<InvalidOperationException>(
            async () => await _orderService.GetAllOrdersAsync()
        );

        Assert.Equal("Database query failed", exception.Message);
    }

    [Fact]
    public async Task GetOrdersByUserIdAsync_ShouldThrowException_WhenRepositoryThrows()
    {
        // Arrange
        _orderRepositoryMock.Setup(repo => repo.GetOrdersByUserIdAsync(1))
            .ThrowsAsync(new InvalidOperationException("User orders query failed"));

        // Act & Assert
        var exception = await Assert.ThrowsAsync<InvalidOperationException>(
            async () => await _orderService.GetOrdersByUserIdAsync(1)
        );

        Assert.Equal("User orders query failed", exception.Message);
    }

    [Fact]
    public async Task GetOrderByIdAsync_ShouldThrowException_WhenRepositoryThrows()
    {
        // Arrange
        _orderRepositoryMock.Setup(repo => repo.GetOrderByIdAsync(1))
            .ThrowsAsync(new InvalidOperationException("Single order query failed"));

        // Act & Assert
        var exception = await Assert.ThrowsAsync<InvalidOperationException>(
            async () => await _orderService.GetOrderByIdAsync(1)
        );

        Assert.Equal("Single order query failed", exception.Message);
    }

    [Fact]
    public async Task UpdateOrderStatusAsync_ShouldThrowException_WhenRepositoryGetThrows()
    {
        // Arrange
        _orderRepositoryMock.Setup(repo => repo.GetOrderByIdAsync(1))
            .ThrowsAsync(new InvalidOperationException("Get order failed"));

        // Act & Assert
        var exception = await Assert.ThrowsAsync<InvalidOperationException>(
            async () => await _orderService.UpdateOrderStatusAsync(1, "Shipped")
        );

        Assert.Equal("Get order failed", exception.Message);
    }

    [Fact]
    public async Task UpdateOrderStatusAsync_ShouldThrowException_WhenRepositoryUpdateThrows()
    {
        // Arrange
        var orderEntity = new OrderEntity { Id = 1, OrderStatus = "Processing" };
        _orderRepositoryMock.Setup(repo => repo.GetOrderByIdAsync(1)).ReturnsAsync(orderEntity);
        _orderRepositoryMock.Setup(repo => repo.UpdateOrderAsync(orderEntity))
            .ThrowsAsync(new InvalidOperationException("Update order failed"));

        // Act & Assert
        var exception = await Assert.ThrowsAsync<InvalidOperationException>(
            async () => await _orderService.UpdateOrderStatusAsync(1, "Shipped")
        );

        Assert.Equal("Update order failed", exception.Message);
    }

    [Fact]
    public async Task UpdatePaymentStatusAsync_ShouldThrowException_WhenRepositoryGetThrows()
    {
        // Arrange
        _orderRepositoryMock.Setup(repo => repo.GetOrderByIdAsync(1))
            .ThrowsAsync(new InvalidOperationException("Get order for payment update failed"));

        // Act & Assert
        var exception = await Assert.ThrowsAsync<InvalidOperationException>(
            async () => await _orderService.UpdatePaymentStatusAsync(1, "Approved", null)
        );

        Assert.Equal("Get order for payment update failed", exception.Message);
    }

    [Fact]
    public async Task UpdatePaymentStatusAsync_ShouldThrowException_WhenRepositoryUpdateThrows()
    {
        // Arrange
        var orderEntity = new OrderEntity { Id = 1, PaymentStatus = "Pending" };
        _orderRepositoryMock.Setup(repo => repo.GetOrderByIdAsync(1)).ReturnsAsync(orderEntity);
        _orderRepositoryMock.Setup(repo => repo.UpdateOrderAsync(orderEntity))
            .ThrowsAsync(new InvalidOperationException("Update payment status failed"));

        // Act & Assert
        var exception = await Assert.ThrowsAsync<InvalidOperationException>(
            async () => await _orderService.UpdatePaymentStatusAsync(1, "Approved", null)
        );

        Assert.Equal("Update payment status failed", exception.Message);
    }
}