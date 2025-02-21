using Moq;
using Microsoft.AspNetCore.Mvc;
using ECommerceAPI.Application.Interfaces;
using ECommerceAPI.WebApi.Controllers;
using ECommerceAPI.WebApi.DTOs.RequestModels;
using ECommerceAPI.WebApi.DTOs.ResponseModels;
using ECommerceAPI.Application.Models;
using FluentValidation;
using FluentValidation.Results;
using ECommerceAPI.Tests.Common;
namespace ECommerceAPI.Tests.WebApi.Controllers;
public class CartControllerTests : TestBase
{
    private readonly Mock<ICartService> _cartServiceMock;
    private readonly Mock<IValidator<CartAddOrUpdateItemRequest>> _validatorMock;
    private readonly CartController _controller;

    public CartControllerTests()
    {
        _cartServiceMock = new Mock<ICartService>();
        _validatorMock = new Mock<IValidator<CartAddOrUpdateItemRequest>>();

        _controller = new CartController(_cartServiceMock.Object, Mapper, _validatorMock.Object);
    }

    [Fact]
    public async Task GetCart_ShouldReturnCart_WhenCartExists()
    {
        // Arrange
        int userId = 1;
        var cart = new Cart
        {
            UserId = userId,
            CartItems = new List<CartItem> { new CartItem { ProductId = 1, Quantity = 2 } }
        };

        _cartServiceMock.Setup(svc => svc.GetCartByUserIdAsync(userId)).ReturnsAsync(cart);

        // Act
        var result = await _controller.GetCart(userId);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var returnedCart = Assert.IsType<CartResponse>(okResult.Value);
        Assert.Equal(cart.UserId, returnedCart.UserId);
    }

    [Fact]
    public async Task GetCart_ShouldThrowKeyNotFoundException_WhenCartDoesNotExist()
    {
        // Arrange
        int userId = 1;
        _cartServiceMock.Setup(svc => svc.GetCartByUserIdAsync(userId)).ThrowsAsync(new KeyNotFoundException("Cart not found"));

        // Act & Assert
        var exception = await Assert.ThrowsAsync<KeyNotFoundException>(
            async () => await _controller.GetCart(userId)
        );

        Assert.Equal("Cart not found", exception.Message);
    }

    [Fact]
    public async Task AddOrUpdateItem_ShouldReturnOk_WhenItemIsAdded()
    {
        // Arrange
        int userId = 1;
        var request = new CartAddOrUpdateItemRequest { ProductId = 1, Quantity = 2 };
        var cartItem = new CartItem { ProductId = request.ProductId, Quantity = request.Quantity };

        _validatorMock.Setup(v => v.ValidateAsync(request, default))
            .ReturnsAsync(new ValidationResult());
        
        _cartServiceMock.Setup(svc => svc.AddOrUpdateCartItemAsync(It.IsAny<int>(), It.IsAny<CartItem>()))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _controller.AddOrUpdateItem(userId, request);

        // Assert
        Assert.IsType<OkObjectResult>(result);

        // Verify interactions
        _cartServiceMock.Verify(svc => svc.AddOrUpdateCartItemAsync(It.IsAny<int>(), It.IsAny<CartItem>()), Times.Once);
    }

    [Fact]
    public async Task ClearCart_ShouldReturnNoContent_WhenCartIsCleared()
    {
        // Arrange
        int userId = 1;
        _cartServiceMock.Setup(svc => svc.ClearCartAsync(userId)).Returns(Task.CompletedTask);

        // Act
        var result = await _controller.ClearCart(userId);

        // Assert
        Assert.IsType<NoContentResult>(result);
    }

    [Fact]
    public async Task ClearCart_ShouldThrowKeyNotFoundException_WhenCartDoesNotExist()
    {
        // Arrange
        int userId = 1;
        _cartServiceMock.Setup(svc => svc.ClearCartAsync(userId)).ThrowsAsync(new KeyNotFoundException("Cart is empty"));

        // Act & Assert
        var exception = await Assert.ThrowsAsync<KeyNotFoundException>(
            async () => await _controller.ClearCart(userId)
        );

        Assert.Equal("Cart is empty", exception.Message);
    }
}