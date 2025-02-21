using Xunit;
using Moq;
using AutoMapper;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.Mvc;
using ECommerceAPI.WebApi.Controllers;
using ECommerceAPI.Application.Interfaces;
using ECommerceAPI.WebApi.DTOs.RequestModels;
using ECommerceAPI.WebApi.DTOs.ResponseModels;
using ECommerceAPI.Application.Models;
using System.Collections.Generic;
using System.Threading.Tasks;
namespace ECommerceAPI.Tests.WebApi.Controllers;
public class OrderControllerTests
{
    private readonly Mock<IOrderService> _orderServiceMock;
    private readonly Mock<IMapper> _mapperMock;
    private readonly Mock<IValidator<OrderCreateRequest>> _createValidatorMock;
    private readonly Mock<IValidator<OrderStatusUpdateRequest>> _updateValidatorMock;
    private readonly OrderController _controller;

    public OrderControllerTests()
    {
        _orderServiceMock = new Mock<IOrderService>();
        _mapperMock = new Mock<IMapper>();
        _createValidatorMock = new Mock<IValidator<OrderCreateRequest>>();
        _updateValidatorMock = new Mock<IValidator<OrderStatusUpdateRequest>>();

        _controller = new OrderController(
            _orderServiceMock.Object,
            _mapperMock.Object,
            _createValidatorMock.Object, 
            _updateValidatorMock.Object
        );
    }

    [Fact]
    public async Task CreateOrder_ShouldReturnCreated_WhenValidRequest()
    {
        var request = new OrderCreateRequest
        {
            OrderProducts = new List<OrderProductRequest> { new OrderProductRequest { ProductId = 1, Quantity = 2 } },
            TotalAmount = 100,
            ShippingAddress = "123 Street, NY"
        };

        var order = new Order { Id = 1, TotalAmount = 100, ShippingAddress = "123 Street, NY" };
        var response = new OrderResponse { Id = 1, TotalAmount = 100, ShippingAddress = "123 Street, NY" };

        _createValidatorMock.Setup(v => v.ValidateAsync(request, default))
            .ReturnsAsync(new ValidationResult());

        _mapperMock.Setup(m => m.Map<Order>(request)).Returns(order);
        _mapperMock.Setup(m => m.Map<OrderResponse>(order)).Returns(response);

        _orderServiceMock.Setup(svc => svc.CreateOrderAsync(_mapperMock.Object.Map<Order>(request)))
            .ReturnsAsync(order);

        var result = await _controller.CreateOrder(1, request);

        var createdResult = Assert.IsType<CreatedAtActionResult>(result);
        var returnedResponse = Assert.IsType<OrderResponse>(createdResult.Value);
        Assert.Equal(response.Id, returnedResponse.Id);
    }

    [Fact]
    public async Task GetOrders_ShouldReturnListOfOrders()
    {
        var orders = new List<Order> { new Order { Id = 1, TotalAmount = 100 } };
        var responseList = new List<OrderResponse> { new OrderResponse { Id = 1, TotalAmount = 100 } };

        _orderServiceMock.Setup(svc => svc.GetOrdersByUserIdAsync(1)).ReturnsAsync(orders);
        _mapperMock.Setup(m => m.Map<IEnumerable<OrderResponse>>(orders)).Returns(responseList);

        var result = await _controller.GetOrders(1);
        var okResult = Assert.IsType<OkObjectResult>(result);
        var returnedResponse = Assert.IsType<List<OrderResponse>>(okResult.Value);

        Assert.Single(returnedResponse);
    }

    [Fact]
    public async Task GetOrders_ShouldThrowKeyNotFoundException_WhenOrdersNotFound()
    {
        _orderServiceMock.Setup(svc => svc.GetOrdersByUserIdAsync(1))
            .ThrowsAsync(new KeyNotFoundException("Orders not found"));

        var exception = await Assert.ThrowsAsync<KeyNotFoundException>(
            async () => await _controller.GetOrders(1)
        );

        Assert.Equal("Orders not found", exception.Message);
    }

    [Fact]
    public async Task UpdateOrderStatus_ShouldReturnNoContent_WhenValidRequest()
    {
        var request = new OrderStatusUpdateRequest { Status = "Shipped" };

        _updateValidatorMock.Setup(v => v.ValidateAsync(request, default))
            .ReturnsAsync(new ValidationResult());

        _orderServiceMock.Setup(svc => svc.UpdateOrderStatusAsync(1, "Shipped"))
            .Returns(Task.CompletedTask);

        var result = await _controller.UpdateOrderStatus(1, 1, request);
        Assert.IsType<NoContentResult>(result);
    }

    [Fact]
    public async Task UpdateOrderStatus_ShouldThrowUnauthorizedException_WhenUnauthorized()
    {
        var request = new OrderStatusUpdateRequest { Status = "Shipped" };

        _updateValidatorMock.Setup(v => v.ValidateAsync(request, default))
            .ReturnsAsync(new ValidationResult());

        _orderServiceMock.Setup(svc => svc.UpdateOrderStatusAsync(1, "Shipped"))
            .ThrowsAsync(new UnauthorizedAccessException("Not authorized"));

        var exception = await Assert.ThrowsAsync<UnauthorizedAccessException>(
            async () => await _controller.UpdateOrderStatus(1, 1, request)
        );

        Assert.Equal("Not authorized", exception.Message);
    }
}