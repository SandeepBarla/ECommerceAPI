using Moq;
using ECommerceAPI.Application.Interfaces;
using ECommerceAPI.Application.Models;
using ECommerceAPI.Infrastructure.Interfaces;
using ECommerceAPI.Application.Services;
using ECommerceAPI.Infrastructure.Entities;
using ECommerceAPI.Tests.Common;

public class CartServiceTests : TestBase
{
    private readonly Mock<ICartRepository> _cartRepositoryMock;
    private readonly Mock<IProductService> _productServiceMock;
    private readonly Mock<IUserService> _userServiceMock;
    private readonly CartService _cartService;

    public CartServiceTests()
    {
        _cartRepositoryMock = new Mock<ICartRepository>();
        _productServiceMock = new Mock<IProductService>();
        _userServiceMock = new Mock<IUserService>();

        _cartService = new CartService(
            _cartRepositoryMock.Object,
            _userServiceMock.Object,
            _productServiceMock.Object, 
            Mapper
        );
    }

    [Fact]
    public async Task GetCartByUserIdAsync_ShouldReturnCart_WhenCartExists()
    {
        // Arrange
        int userId = 1;
        var cartEntity = new CartEntity { UserId = userId, CartItems = new List<CartItemEntity> { new CartItemEntity { ProductId = 1, Quantity = 2 } } };
        
        _cartRepositoryMock.Setup(repo => repo.GetCartByUserIdAsync(userId)).ReturnsAsync(cartEntity);
        _userServiceMock.Setup(svc => svc.GetUserByIdAsync(userId)).ReturnsAsync(new User { Id = userId });

        // Act
        var result = await _cartService.GetCartByUserIdAsync(userId);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(userId, result.UserId);
        Assert.Single(result.CartItems);
    }

    [Fact]
    public async Task GetCartByUserIdAsync_ShouldThrowException_WhenUserDoesNotExist()
    {
        // Arrange
        int userId = 99;
        _userServiceMock.Setup(svc => svc.GetUserByIdAsync(userId)).ThrowsAsync(new KeyNotFoundException("User not found"));

        // Act & Assert
        await Assert.ThrowsAsync<KeyNotFoundException>(() => _cartService.GetCartByUserIdAsync(userId));
    }

    [Fact]
    public async Task AddOrUpdateCartItemAsync_ShouldThrowException_WhenUserDoesNotExist()
    {
        // Arrange
        int userId = 99;
        var cartItem = new CartItem { ProductId = 1, Quantity = 2 };
        
        _userServiceMock.Setup(svc => svc.GetUserByIdAsync(userId)).ThrowsAsync(new KeyNotFoundException("User not found"));

        // Act & Assert
        await Assert.ThrowsAsync<KeyNotFoundException>(() => _cartService.AddOrUpdateCartItemAsync(userId, cartItem));
    }

    [Fact]
    public async Task AddOrUpdateCartItemAsync_ShouldThrowException_WhenProductDoesNotExist()
    {
        // Arrange
        int userId = 1;
        var cartItem = new CartItem { ProductId = 999, Quantity = 2 };

        _userServiceMock.Setup(svc => svc.GetUserByIdAsync(userId)).ReturnsAsync(new User { Id = userId });
        _productServiceMock.Setup(svc => svc.GetProductByIdAsync(cartItem.ProductId))
            .ThrowsAsync(new KeyNotFoundException("Product not found"));

        // Act & Assert
        await Assert.ThrowsAsync<KeyNotFoundException>(() => _cartService.AddOrUpdateCartItemAsync(userId, cartItem));
    }

    [Fact]
    public async Task AddOrUpdateCartItemAsync_ShouldAddNewItem_WhenValidUserAndProduct()
    {
        // Arrange
        int userId = 1;
        var cartItem = new CartItem { ProductId = 1, Quantity = 2 };
        var cart = new CartEntity { UserId = userId, CartItems = new List<CartItemEntity>() };

        _userServiceMock.Setup(svc => svc.GetUserByIdAsync(userId)).ReturnsAsync(new User { Id = userId });
        _productServiceMock.Setup(svc => svc.GetProductByIdAsync(cartItem.ProductId)).ReturnsAsync(new Product { Id = 1 });

        _cartRepositoryMock.Setup(repo => repo.GetCartByUserIdAsync(userId)).ReturnsAsync(cart);

        // Act
        await _cartService.AddOrUpdateCartItemAsync(userId, cartItem);

        // Assert
        _cartRepositoryMock.Verify(repo => repo.SaveCartAsync(It.Is<CartEntity>(c => c.CartItems.Count == 1)), Times.Once);
    }

    [Fact]
    public async Task AddOrUpdateCartItemAsync_ShouldRemoveItem_WhenQuantityIsZero()
    {
        // Arrange
        int userId = 1;
        var existingItem = new CartItemEntity { ProductId = 1, Quantity = 3 };
        var cartEntity = new CartEntity { UserId = userId, CartItems = new List<CartItemEntity> { existingItem } };
        var removeItem = new CartItem { ProductId = 1, Quantity = 0 };

        _userServiceMock.Setup(svc => svc.GetUserByIdAsync(userId)).ReturnsAsync(new User { Id = userId });
        _productServiceMock.Setup(svc => svc.GetProductByIdAsync(removeItem.ProductId)).ReturnsAsync(new Product { Id = 1 });

        _cartRepositoryMock.Setup(repo => repo.GetCartByUserIdAsync(userId)).ReturnsAsync(cartEntity);

        // Act
        await _cartService.AddOrUpdateCartItemAsync(userId, removeItem);

        // Assert
        Assert.Empty(cartEntity.CartItems);
        _cartRepositoryMock.Verify(repo => repo.SaveCartAsync(cartEntity), Times.Once);
    }

    [Fact]
    public async Task ClearCartAsync_ShouldThrowException_WhenCartDoesNotExist()
    {
        // Arrange
        int userId = 1;
        _cartRepositoryMock.Setup(repo => repo.GetCartByUserIdAsync(userId)).ReturnsAsync((CartEntity)null);

        // Act & Assert
        await Assert.ThrowsAsync<KeyNotFoundException>(() => _cartService.ClearCartAsync(userId));
    }

    [Fact]
    public async Task ClearCartAsync_ShouldRemoveCart_WhenCartExists()
    {
        // Arrange
        int userId = 1;
        var cartEntity = new CartEntity { UserId = userId, CartItems = new List<CartItemEntity> { new CartItemEntity { ProductId = 1, Quantity = 2 } } };

        _cartRepositoryMock.Setup(repo => repo.GetCartByUserIdAsync(userId)).ReturnsAsync(cartEntity);

        // Act
        await _cartService.ClearCartAsync(userId);

        // Assert
        _cartRepositoryMock.Verify(repo => repo.RemoveCartAsync(cartEntity), Times.Once);
    }
}