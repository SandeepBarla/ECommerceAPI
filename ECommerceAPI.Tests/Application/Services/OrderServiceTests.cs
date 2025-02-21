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
namespace ECommerceAPI.Tests.Application.Services;
public class OrderServiceTests
{
    private readonly Mock<IOrderRepository> _orderRepositoryMock;
    private readonly Mock<IMapper> _mapperMock;
    private readonly OrderService _orderService;

    public OrderServiceTests()
    {
        _orderRepositoryMock = new Mock<IOrderRepository>();
        _mapperMock = new Mock<IMapper>();

        _orderService = new OrderService(_orderRepositoryMock.Object, _mapperMock.Object);
    }

    [Fact]
    public async Task CreateOrder_ShouldCallRepository_WithMappedEntity()
    {
        var order = new Order { UserId = 1, TotalAmount = 100, OrderStatus = "Pending" };
        var orderEntity = new OrderEntity { UserId = 1, TotalAmount = 100, OrderStatus = "Pending" };

        _mapperMock.Setup(m => m.Map<OrderEntity>(order)).Returns(orderEntity);
        _orderRepositoryMock.Setup(repo => repo.CreateOrderAsync(orderEntity)).ReturnsAsync(orderEntity);

        await _orderService.CreateOrderAsync(order);

        _orderRepositoryMock.Verify(repo => repo.CreateOrderAsync(It.IsAny<OrderEntity>()), Times.Once);
    }

    [Fact]
    public async Task GetOrderById_ShouldReturnOrder_WhenOrderExists()
    {
        var orderEntity = new OrderEntity { Id = 1, UserId = 1, TotalAmount = 100 };
        var order = new Order { Id = 1, UserId = 1, TotalAmount = 100 };

        _orderRepositoryMock.Setup(repo => repo.GetOrderByIdAsync(1)).ReturnsAsync(orderEntity);
        _mapperMock.Setup(m => m.Map<Order>(orderEntity)).Returns(order);

        var result = await _orderService.GetOrderByIdAsync(1);

        result.Should().NotBeNull();
        result.Id.Should().Be(1);
        result.TotalAmount.Should().Be(100);
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
}