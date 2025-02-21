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
using ECommerceAPI.Application.Models.Enums;
namespace ECommerceAPI.Tests.WebApi.Controllers;
public class ProductControllerTests
{
    private readonly Mock<IProductService> _productServiceMock;
    private readonly Mock<IMapper> _mapperMock;
    private readonly Mock<IValidator<ProductUpsertRequest>> _validatorMock;
    private readonly ProductController _controller;

    public ProductControllerTests()
    {
        _productServiceMock = new Mock<IProductService>();
        _mapperMock = new Mock<IMapper>();
        _validatorMock = new Mock<IValidator<ProductUpsertRequest>>();

        _controller = new ProductController(_mapperMock.Object, _productServiceMock.Object, _validatorMock.Object);
    }

    [Fact]
    public async Task CreateProduct_ShouldReturnProductWithGeneratedId()
    {
        // Arrange
        var request = new ProductUpsertRequest
        {
            Name = "Test Product",
            Price = 20.0m,
            Stock = 10,
            Media = new List<ProductMediaRequest>
            {
                new() { MediaUrl = "https://example.com/image1.jpg", Type = MediaType.Image, OrderIndex = 1 }
            }
        };

        var createdProduct = new Product
        {
            Id = 1,
            Name = "Test Product",
            Price = 20.0m,
            Stock = 10,
            Media = new List<ProductMedia>
            {
                new() { MediaUrl = "https://example.com/image1.jpg", Type = MediaType.Image, OrderIndex = 1 }
            }
        };

        var response = new ProductResponse
        {
            Id = 1,
            Name = "Test Product",
            Price = 20.0m,
            Stock = 10,
            Media = new List<ProductMediaResponse>
            {
                new() { MediaUrl = "https://example.com/image1.jpg", Type = MediaType.Image.ToString(), OrderIndex = 1 }
            }
        };

        _validatorMock.Setup(v => v.ValidateAsync(request, default))
            .ReturnsAsync(new ValidationResult());

        _mapperMock.Setup(m => m.Map<Product>(request)).Returns(createdProduct);
        _mapperMock.Setup(m => m.Map<ProductResponse>(createdProduct)).Returns(response);

        _productServiceMock.Setup(svc => svc.CreateProductAsync(It.IsAny<Product>()))
            .ReturnsAsync(createdProduct);

        // Act
        var result = await _controller.CreateProduct(request);

        // Assert
        var createdResult = Assert.IsType<CreatedAtActionResult>(result);
        var returnedResponse = Assert.IsType<ProductResponse>(createdResult.Value);
        Assert.Equal(1, returnedResponse.Id);
        Assert.Single(returnedResponse.Media);
    }

    [Fact]
    public async Task GetProductById_ShouldReturnProduct_WhenExists()
    {
        var product = new Product
        {
            Id = 1,
            Name = "Test Product",
            Price = 20.0m,
            Stock = 10,
            Media = new List<ProductMedia>
            {
                new() { MediaUrl = "https://example.com/image1.jpg", Type = MediaType.Image, OrderIndex = 1 }
            }
        };

        var response = new ProductResponse
        {
            Id = 1,
            Name = "Test Product",
            Price = 20.0m,
            Stock = 10,
            Media = new List<ProductMediaResponse>
            {
                new() { MediaUrl = "https://example.com/image1.jpg", Type = MediaType.Image.ToString(), OrderIndex = 1 }
            }
        };

        _productServiceMock.Setup(svc => svc.GetProductByIdAsync(1)).ReturnsAsync(product);
        _mapperMock.Setup(m => m.Map<ProductResponse>(product)).Returns(response);

        var result = await _controller.GetProductById(1);

        var okResult = Assert.IsType<OkObjectResult>(result);
        var returnedResponse = Assert.IsType<ProductResponse>(okResult.Value);

        Assert.Equal(1, returnedResponse.Id);
        Assert.Single(returnedResponse.Media);
    }

    [Fact]
    public async Task GetProductById_ShouldReturnNotFound_WhenProductDoesNotExist()
    {
        _productServiceMock
            .Setup(svc => svc.GetProductByIdAsync(1))
            .ThrowsAsync(new KeyNotFoundException("Product not found"));
        
        // Act & Assert
        var exception = await Assert.ThrowsAsync<KeyNotFoundException>(
            async () => await _controller.GetProductById(1)
        );
        
        Assert.Equal("Product not found", exception.Message);
    }

    [Fact]
    public async Task GetAllProducts_ShouldReturnListOfProducts()
    {
        var products = new List<Product>
        {
            new()
            {
                Id = 1,
                Name = "Product 1",
                Price = 10,
                Stock = 5
            }
        };

        var responseList = new List<ProductListResponse>
        {
            new()
            {
                Id = 1,
                Name = "Product 1",
                Price = 10,
                Stock = 5,
                PrimaryImageUrl = "https://example.com/image1.jpg"
            }
        };

        _productServiceMock.Setup(svc => svc.GetAllProductsAsync()).ReturnsAsync(products);
        _mapperMock.Setup(m => m.Map<IEnumerable<ProductListResponse>>(products)).Returns(responseList);

        var result = await _controller.GetAllProducts();
        var okResult = Assert.IsType<OkObjectResult>(result);
        var returnedResponse = Assert.IsType<List<ProductListResponse>>(okResult.Value);

        Assert.Single(returnedResponse);
    }

    [Fact]
    public async Task UpdateProduct_ShouldReturnOk_WhenProductExists()
    {
        var request = new ProductUpsertRequest
        {
            Name = "Updated Product",
            Price = 20.5m,
            Stock = 10,
            Media = new List<ProductMediaRequest>
            {
                new() { MediaUrl = "https://example.com/image1.jpg", Type = MediaType.Image, OrderIndex = 1 }
            }
        };

        _validatorMock.Setup(v => v.ValidateAsync(request, default))
            .ReturnsAsync(new ValidationResult());

        _productServiceMock.Setup(svc => svc.UpdateProductAsync(It.IsAny<Product>()))
            .Returns(Task.CompletedTask);

        var result = await _controller.UpdateProduct(1, request);

        Assert.IsType<OkObjectResult>(result);
    }

    [Fact]
    public async Task DeleteProduct_ShouldReturnOk_WhenProductExists()
    {
        _productServiceMock.Setup(svc => svc.DeleteProductAsync(1)).Returns(Task.CompletedTask);

        var result = await _controller.DeleteProduct(1);

        Assert.IsType<OkObjectResult>(result);
    }

    [Fact]
    public async Task DeleteProduct_ShouldThrowKeyNotFoundException_WhenProductDoesNotExist()
    {
        _productServiceMock
            .Setup(svc => svc.DeleteProductAsync(1))
            .ThrowsAsync(new KeyNotFoundException("Product not found"));
        
        // Act & Assert
        var exception = await Assert.ThrowsAsync<KeyNotFoundException>(
            async () => await _controller.DeleteProduct(1)
        );
        
        Assert.Equal("Product not found", exception.Message);
    }
}