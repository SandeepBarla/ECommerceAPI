using AutoMapper;
using ECommerceAPI.Application.Interfaces;
using ECommerceAPI.Application.Models;
using ECommerceAPI.Application.Models.Enums;
using ECommerceAPI.Application.Services;
using ECommerceAPI.Infrastructure.Entities;
using ECommerceAPI.Infrastructure.Interfaces;
using ECommerceAPI.WebApi.DTOs.ResponseModels;
using FluentAssertions;
using Moq;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace ECommerceAPI.Tests.Application.Services;

public class ProductServiceTests
{
    private readonly Mock<IProductRepository> _repositoryMock;
    private readonly Mock<IMapper> _mapperMock;
    private readonly ProductService _service;

    public ProductServiceTests()
    {
        _repositoryMock = new Mock<IProductRepository>();
        _mapperMock = new Mock<IMapper>();
        _service = new ProductService(_repositoryMock.Object, _mapperMock.Object);
    }

    [Fact]
    public async Task GetProductByIdAsync_ShouldReturnProduct_WhenProductExists()
    {
        var entity = new ProductEntity { Id = 1, Name = "Test Product" };
        var product = new Product { Id = 1, Name = "Test Product" };

        _repositoryMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(entity);
        _mapperMock.Setup(m => m.Map<Product>(entity)).Returns(product);

        var result = await _service.GetProductByIdAsync(1);

        Assert.Equal(product, result);
    }

    [Fact]
    public async Task GetProductByIdAsync_ShouldThrowKeyNotFoundException_WhenProductDoesNotExist()
    {
        _repositoryMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync((ProductEntity)null);

        await Assert.ThrowsAsync<KeyNotFoundException>(() => _service.GetProductByIdAsync(1));
    }

    [Fact]
    public async Task GetProductResponseByIdAsync_ShouldReturnProductResponse_WhenProductExists()
    {
        var entity = new ProductEntity
        {
            Id = 1,
            Name = "Test Product",
            Description = "Test Description",
            OriginalPrice = 129.99m,
            DiscountedPrice = 99.99m,
            IsFeatured = true,
            NewUntil = DateTime.UtcNow.AddDays(30),
            Category = new CategoryEntity { Id = 1, Name = "Electronics" },
            Size = new SizeEntity { Id = 1, Name = "Medium" },
            Media = new List<ProductMediaEntity>
            {
                new() { MediaUrl = "https://example.com/image1.jpg", Type = MediaType.Image, OrderIndex = 1 }
            }
        };

        var expectedResponse = new ProductResponse
        {
            Id = 1,
            Name = "Test Product",
            Description = "Test Description",
            OriginalPrice = 129.99m,
            DiscountedPrice = 99.99m,
            DiscountPercentage = 23, // Calculated: (129.99-99.99)/129.99*100
            IsFeatured = true,
            IsNew = true, // Calculated from NewUntil
            CategoryName = "Electronics",
            SizeName = "Medium",
            Media = new List<ProductMediaResponse>
            {
                new() { MediaUrl = "https://example.com/image1.jpg", Type = "Image", OrderIndex = 1 }
            }
        };

        _repositoryMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(entity);
        _mapperMock.Setup(m => m.Map<ProductResponse>(entity)).Returns(expectedResponse);

        var result = await _service.GetProductResponseByIdAsync(1);

        Assert.Equal(expectedResponse.Id, result.Id);
        Assert.Equal("Electronics", result.CategoryName);
        Assert.Equal("Medium", result.SizeName);
        Assert.Equal(23, result.DiscountPercentage);
        Assert.True(result.IsFeatured);
    }

    [Fact]
    public async Task GetProductResponseByIdAsync_ShouldThrowKeyNotFoundException_WhenProductDoesNotExist()
    {
        _repositoryMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync((ProductEntity)null);

        await Assert.ThrowsAsync<KeyNotFoundException>(() => _service.GetProductResponseByIdAsync(1));
    }

    [Fact]
    public async Task GetProductListResponsesAsync_ShouldReturnProductListResponses()
    {
        var entities = new List<ProductEntity>
        {
            new()
            {
                Id = 1,
                Name = "Product 1",
                OriginalPrice = 129.99m,
                DiscountedPrice = 99.99m,
                IsFeatured = true,
                Category = new CategoryEntity { Id = 1, Name = "Electronics" },
                Size = new SizeEntity { Id = 1, Name = "Medium" },
                Media = new List<ProductMediaEntity>
                {
                    new() { MediaUrl = "https://example.com/image1.jpg", Type = MediaType.Image, OrderIndex = 1 }
                }
            },
            new()
            {
                Id = 2,
                Name = "Product 2",
                OriginalPrice = 199.99m,
                IsFeatured = false,
                Category = new CategoryEntity { Id = 2, Name = "Clothing" },
                Size = new SizeEntity { Id = 2, Name = "Large" },
                Media = new List<ProductMediaEntity>
                {
                    new() { MediaUrl = "https://example.com/image2.jpg", Type = MediaType.Image, OrderIndex = 1 }
                }
            }
        };

        var expectedResponses = new List<ProductListResponse>
        {
            new()
            {
                Id = 1,
                Name = "Product 1",
                OriginalPrice = 129.99m,
                DiscountedPrice = 99.99m,
                DiscountPercentage = 23,
                IsFeatured = true,
                PrimaryImageUrl = "https://example.com/image1.jpg",
                CategoryName = "Electronics",
                SizeName = "Medium"
            },
            new()
            {
                Id = 2,
                Name = "Product 2",
                OriginalPrice = 199.99m,
                IsFeatured = false,
                PrimaryImageUrl = "https://example.com/image2.jpg",
                CategoryName = "Clothing",
                SizeName = "Large"
            }
        };

        _repositoryMock.Setup(r => r.GetAllAsync()).ReturnsAsync(entities);
        _mapperMock.Setup(m => m.Map<IEnumerable<ProductListResponse>>(entities)).Returns(expectedResponses);

        var result = await _service.GetProductListResponsesAsync();

        Assert.Equal(2, result.Count());
        Assert.Equal("Electronics", result.First().CategoryName);
        Assert.Equal("Medium", result.First().SizeName);
        Assert.Equal(23, result.First().DiscountPercentage);
        Assert.True(result.First().IsFeatured);
    }

    [Fact]
    public async Task CreateProductAsync_ShouldCallRepositoryCreate()
    {
        var product = new Product { Name = "Test Product" };
        var entity = new ProductEntity { Name = "Test Product" };

        _mapperMock.Setup(m => m.Map<ProductEntity>(product)).Returns(entity);

        await _service.CreateProductAsync(product);

        _repositoryMock.Verify(r => r.CreateAsync(entity), Times.Once);
    }

    [Fact]
    public async Task UpdateProductAsync_ShouldUpdate_WhenProductExists()
    {
        var product = new Product { Id = 1, Name = "Updated Product" };
        var existingEntity = new ProductEntity { Id = 1, Name = "Existing Product" };

        _repositoryMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(existingEntity);

        await _service.UpdateProductAsync(product);

        _mapperMock.Verify(m => m.Map(product, existingEntity), Times.Once);
        _repositoryMock.Verify(r => r.UpdateAsync(existingEntity), Times.Once);
    }

    [Fact]
    public async Task UpdateProductAsync_ShouldThrowKeyNotFoundException_WhenProductDoesNotExist()
    {
        var product = new Product { Id = 1, Name = "Updated Product" };

        _repositoryMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync((ProductEntity)null);

        await Assert.ThrowsAsync<KeyNotFoundException>(() => _service.UpdateProductAsync(product));
    }

    [Fact]
    public async Task DeleteProductAsync_ShouldDelete_WhenProductExists()
    {
        await _service.DeleteProductAsync(1);

        _repositoryMock.Verify(r => r.DeleteAsync(1), Times.Once);
    }

    [Fact]
    public async Task GetProductResponseByIdAsync_ShouldCalculateDiscountPercentage_WhenOriginalPriceExists()
    {
        var entity = new ProductEntity
        {
            Id = 1,
            Name = "Test Product",
            OriginalPrice = 100m,
            DiscountedPrice = 80m, // 20% discount
            Category = new CategoryEntity { Id = 1, Name = "Electronics" },
            Size = new SizeEntity { Id = 1, Name = "Medium" }
        };

        var expectedResponse = new ProductResponse
        {
            Id = 1,
            Name = "Test Product",
            OriginalPrice = 100m,
            DiscountedPrice = 80m,
            DiscountPercentage = 20 // Auto-calculated
        };

        _repositoryMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(entity);
        _mapperMock.Setup(m => m.Map<ProductResponse>(entity)).Returns(expectedResponse);

        var result = await _service.GetProductResponseByIdAsync(1);

        Assert.Equal(20, result.DiscountPercentage);
    }

    [Fact]
    public async Task GetProductResponseByIdAsync_ShouldHandleNullCategoryAndSize()
    {
        var entity = new ProductEntity
        {
            Id = 1,
            Name = "Test Product",
            Category = null, // Missing category
            Size = null // Missing size
        };

        var expectedResponse = new ProductResponse
        {
            Id = 1,
            Name = "Test Product"
        };

        _repositoryMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(entity);
        _mapperMock.Setup(m => m.Map<ProductResponse>(entity)).Returns(expectedResponse);

        var result = await _service.GetProductResponseByIdAsync(1);

        Assert.Equal("Unknown", result.CategoryName);
        Assert.Equal("Unknown", result.SizeName);
    }
}
