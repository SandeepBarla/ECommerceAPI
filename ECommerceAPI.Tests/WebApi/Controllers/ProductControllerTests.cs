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
            OriginalPrice = 25.0m,
            DiscountedPrice = 20.0m,
            Media = new List<ProductMediaRequest>
            {
                new() { MediaUrl = "https://example.com/image1.jpg", Type = MediaType.Image, OrderIndex = 1 }
            }
        };

        var createdProduct = new Product
        {
            Id = 1,
            Name = "Test Product",
            OriginalPrice = 25.0m,
            DiscountedPrice = 20.0m,
            Media = new List<ProductMedia>
            {
                new() { MediaUrl = "https://example.com/image1.jpg", Type = MediaType.Image, OrderIndex = 1 }
            }
        };

        var response = new ProductResponse
        {
            Id = 1,
            Name = "Test Product",
            Description = "Test Description",
            OriginalPrice = 25.0m,
            DiscountedPrice = 20.0m,
            DiscountPercentage = 20, // Calculated: (25-20)/25*100 = 20%
            CategoryName = "Test Category",
            SizeName = "Test Size",
            Media = new List<ProductMediaResponse>
            {
                new() { MediaUrl = "https://example.com/image1.jpg", Type = MediaType.Image.ToString(), OrderIndex = 1 }
            }
        };

        _validatorMock.Setup(v => v.ValidateAsync(request, default))
            .ReturnsAsync(new ValidationResult());

        _mapperMock.Setup(m => m.Map<Product>(request)).Returns(createdProduct);

        _productServiceMock.Setup(svc => svc.CreateProductAsync(It.IsAny<Product>()))
            .ReturnsAsync(createdProduct);

        _productServiceMock.Setup(svc => svc.GetProductResponseByIdAsync(1))
            .ReturnsAsync(response);

        // Act
        var result = await _controller.CreateProduct(request);

        // Assert
        var createdResult = Assert.IsType<CreatedAtActionResult>(result);
        var returnedResponse = Assert.IsType<ProductResponse>(createdResult.Value);
        Assert.Equal(1, returnedResponse.Id);
        Assert.Single(returnedResponse.Media);
    }

    [Fact]
    public async Task CreateProduct_ShouldAcceptNullDiscountedPrice()
    {
        // Arrange
        var request = new ProductUpsertRequest
        {
            Name = "Test Product Without Discount",
            Description = "Test Description",
            OriginalPrice = 25.0m,
            DiscountedPrice = null, // No discount
            CategoryId = 1,
            SizeId = 1,
            Media = new List<ProductMediaRequest>
            {
                new() { MediaUrl = "https://example.com/image1.jpg", Type = MediaType.Image, OrderIndex = 1 }
            }
        };

        var createdProduct = new Product
        {
            Id = 2,
            Name = "Test Product Without Discount",
            Description = "Test Description",
            OriginalPrice = 25.0m,
            DiscountedPrice = null,
            CategoryId = 1,
            SizeId = 1,
            Media = new List<ProductMedia>
            {
                new() { MediaUrl = "https://example.com/image1.jpg", Type = MediaType.Image, OrderIndex = 1 }
            }
        };

        var response = new ProductResponse
        {
            Id = 2,
            Name = "Test Product Without Discount",
            Description = "Test Description",
            OriginalPrice = 25.0m,
            DiscountedPrice = null,
            DiscountPercentage = 0, // No discount
            CategoryName = "Test Category",
            SizeName = "Test Size",
            Media = new List<ProductMediaResponse>
            {
                new() { MediaUrl = "https://example.com/image1.jpg", Type = MediaType.Image.ToString(), OrderIndex = 1 }
            }
        };

        _validatorMock.Setup(v => v.ValidateAsync(request, default))
            .ReturnsAsync(new ValidationResult());

        _mapperMock.Setup(m => m.Map<Product>(request)).Returns(createdProduct);

        _productServiceMock.Setup(svc => svc.CreateProductAsync(It.IsAny<Product>()))
            .ReturnsAsync(createdProduct);

        _productServiceMock.Setup(svc => svc.GetProductResponseByIdAsync(2))
            .ReturnsAsync(response);

        // Act
        var result = await _controller.CreateProduct(request);

        // Assert
        var createdResult = Assert.IsType<CreatedAtActionResult>(result);
        var returnedResponse = Assert.IsType<ProductResponse>(createdResult.Value);
        Assert.Equal(2, returnedResponse.Id);
        Assert.Equal("Test Product Without Discount", returnedResponse.Name);
        Assert.Null(returnedResponse.DiscountedPrice);
        Assert.Equal(0, returnedResponse.DiscountPercentage);
        Assert.Single(returnedResponse.Media);
    }

    [Fact]
    public async Task GetProductById_ShouldReturnProduct_WhenExists()
    {
        var response = new ProductResponse
        {
            Id = 1,
            Name = "Test Product",
            Description = "Test Description",
            OriginalPrice = 25.0m,
            DiscountedPrice = 20.0m,
            DiscountPercentage = 20, // Calculated: (25-20)/25*100 = 20%
            CategoryName = "Test Category",
            SizeName = "Test Size",
            Media = new List<ProductMediaResponse>
            {
                new() { MediaUrl = "https://example.com/image1.jpg", Type = MediaType.Image.ToString(), OrderIndex = 1 }
            }
        };

        _productServiceMock.Setup(svc => svc.GetProductResponseByIdAsync(1)).ReturnsAsync(response);

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
            .Setup(svc => svc.GetProductResponseByIdAsync(1))
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
        var responseList = new List<ProductListResponse>
        {
            new()
            {
                Id = 1,
                Name = "Product 1",
                OriginalPrice = 15,
                DiscountedPrice = 10,
                DiscountPercentage = 33, // Calculated: (15-10)/15*100 = 33%
                PrimaryImageUrl = "https://example.com/image1.jpg",
                CategoryName = "Test Category",
                SizeName = "Test Size"
            }
        };

        _productServiceMock.Setup(svc => svc.GetProductListResponsesAsync()).ReturnsAsync(responseList);

        var result = await _controller.GetAllProducts();
        var okResult = Assert.IsType<OkObjectResult>(result);
        var returnedResponse = okResult.Value as IEnumerable<ProductListResponse>;

        Assert.Single(returnedResponse);
    }

    [Fact]
    public async Task UpdateProduct_ShouldReturnOk_WhenProductExists()
    {
        var request = new ProductUpsertRequest
        {
            Name = "Updated Product",
            OriginalPrice = 25.0m,
            DiscountedPrice = 20.5m,
            Media = new List<ProductMediaRequest>
            {
                new() { MediaUrl = "https://example.com/image1.jpg", Type = MediaType.Image, OrderIndex = 1 }
            }
        };

        var response = new ProductResponse
        {
            Id = 1,
            Name = "Updated Product",
            Description = "Test Description",
            OriginalPrice = 25.0m,
            DiscountedPrice = 20.5m,
            DiscountPercentage = 18, // Calculated: (25-20.5)/25*100 = 18%
            CategoryName = "Test Category",
            SizeName = "Test Size",
            Media = new List<ProductMediaResponse>
            {
                new() { MediaUrl = "https://example.com/image1.jpg", Type = MediaType.Image.ToString(), OrderIndex = 1 }
            }
        };

        _validatorMock.Setup(v => v.ValidateAsync(request, default))
            .ReturnsAsync(new ValidationResult());

        _productServiceMock.Setup(svc => svc.UpdateProductAsync(It.IsAny<Product>()))
            .Returns(Task.CompletedTask);

        _productServiceMock.Setup(svc => svc.GetProductResponseByIdAsync(1))
            .ReturnsAsync(response);

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