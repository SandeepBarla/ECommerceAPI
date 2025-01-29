using AutoMapper;
using ECommerceAPI.Application.Interfaces;
using ECommerceAPI.Application.Models;
using ECommerceAPI.Application.Services;
using ECommerceAPI.Infrastructure.Entities;
using ECommerceAPI.Infrastructure.Interfaces;
using FluentAssertions;
using Moq;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace ECommerceAPI.Tests.Application.Services;

public class ProductServiceTests
{
    private readonly Mock<IProductRepository> _productRepositoryMock;
    private readonly Mock<IMapper> _mapperMock;
    private readonly IProductService _productService;

    public ProductServiceTests()
    {
        _productRepositoryMock = new Mock<IProductRepository>();
        _mapperMock = new Mock<IMapper>();
        _productService = new ProductService(_productRepositoryMock.Object, _mapperMock.Object);
    }

    [Fact]
    public async Task GetProductByIdAsync_ShouldReturnProduct_WhenProductExists()
    {
        // Arrange
        var productEntity = new ProductEntity { Id = 1, Name = "Test Product" };
        var productModel = new Product { Id = 1, Name = "Test Product" };

        _productRepositoryMock.Setup(repo => repo.GetByIdAsync(1))
            .ReturnsAsync(productEntity);
        _mapperMock.Setup(mapper => mapper.Map<Product>(productEntity))
            .Returns(productModel);

        // Act
        var result = await _productService.GetProductByIdAsync(1);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().Be(1);
        result.Name.Should().Be("Test Product");
    }

    [Fact]
    public async Task GetProductByIdAsync_ShouldThrowKeyNotFoundException_WhenProductDoesNotExist()
    {
        _productRepositoryMock.Setup(repo => repo.GetByIdAsync(1))
            .ReturnsAsync((ProductEntity)null!);

        var act = async () => await _productService.GetProductByIdAsync(1);

        await act.Should().ThrowAsync<KeyNotFoundException>()
            .WithMessage("Product not found");
    }

    [Fact]
    public async Task CreateProductAsync_ShouldCallRepositoryCreate()
    {
        var product = new Product { Name = "New Product" };
        var productEntity = new ProductEntity { Name = "New Product" };

        _mapperMock.Setup(m => m.Map<ProductEntity>(product)).Returns(productEntity);

        await _productService.CreateProductAsync(product);

        _productRepositoryMock.Verify(repo => repo.CreateAsync(It.IsAny<ProductEntity>()), Times.Once);
    }

    [Fact]
    public async Task UpdateProductAsync_ShouldUpdate_WhenProductExists()
    {
        var product = new Product { Id = 1, Name = "Updated Product" };
        var productEntity = new ProductEntity { Id = 1, Name = "Original Product" };

        _productRepositoryMock.Setup(repo => repo.GetByIdAsync(1))
            .ReturnsAsync(productEntity);

        await _productService.UpdateProductAsync(product);

        _productRepositoryMock.Verify(repo => repo.UpdateAsync(It.IsAny<ProductEntity>()), Times.Once);
    }

    [Fact]
    public async Task DeleteProductAsync_ShouldDelete_WhenProductExists()
    {
        var productEntity = new ProductEntity { Id = 1, Name = "Product to Delete" };

        _productRepositoryMock.Setup(repo => repo.GetByIdAsync(1))
            .ReturnsAsync(productEntity);

        await _productService.DeleteProductAsync(1);

        _productRepositoryMock.Verify(repo => repo.DeleteAsync(1), Times.Once);
    }
}
