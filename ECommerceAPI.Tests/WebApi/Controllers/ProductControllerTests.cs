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
using System;

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
        var request = new ProductUpsertRequest { Name = "Test Product", Price = 20.0m, Stock = 10 };
        var createdProduct = new Product { Id = 1, Name = "Test Product", Price = 20.0m, Stock = 10 };
        var response = new ProductResponse { Id = 1, Name = "Test Product", Price = 20.0m, Stock = 10 };

        _validatorMock.Setup(v => v.ValidateAsync(request, default))
            .ReturnsAsync(new ValidationResult());

        _mapperMock.Setup(m => m.Map<Product>(request)).Returns(createdProduct);
        _mapperMock.Setup(m => m.Map<ProductResponse>(createdProduct)).Returns(response);

        _productServiceMock.Setup(svc => svc.CreateProductAsync(It.IsAny<Product>()))
            .ReturnsAsync(createdProduct); // ✅ Ensure service returns the created product with ID

        // Act
        var result = await _controller.CreateProduct(request);

        // Assert
        var createdResult = Assert.IsType<CreatedAtActionResult>(result);
        var returnedResponse = Assert.IsType<ProductResponse>(createdResult.Value);
        Assert.Equal(1, returnedResponse.Id); // ✅ Ensure ID is correctly set
    }

    [Fact]
    public async Task CreateProduct_ShouldReturnCreated_WhenValidRequest()
    {
        var request = new ProductUpsertRequest { Name = "New Product", Price = 10.5m, Stock = 5 };
        var product = new Product { Id = 1, Name = "New Product", Price = 10.5m, Stock = 5 };
        var response = new ProductResponse { Id = 1, Name = "New Product", Price = 10.5m, Stock = 5 };

        _validatorMock.Setup(v => v.ValidateAsync(request, default))
            .ReturnsAsync(new ValidationResult());

        _mapperMock.Setup(m => m.Map<Product>(request)).Returns(product);
        _mapperMock.Setup(m => m.Map<ProductResponse>(product)).Returns(response);

        _productServiceMock.Setup(svc => svc.CreateProductAsync(product)).ReturnsAsync(product);

        var result = await _controller.CreateProduct(request);

        var createdResult = Assert.IsType<CreatedAtActionResult>(result);
        var returnedResponse = Assert.IsType<ProductResponse>(createdResult.Value);
        Assert.Equal(response.Id, returnedResponse.Id);
    }

    [Fact]
    public async Task GetProductById_ShouldThrowKeyNotFoundException_WhenProductDoesNotExist()
    {
        _productServiceMock
            .Setup(svc => svc.GetProductByIdAsync(1))
            .ThrowsAsync(new KeyNotFoundException("Product not found"));

        var exception = await Assert.ThrowsAsync<KeyNotFoundException>(
            async () => await _controller.GetProductById(1)
        );

        Assert.Equal("Product not found", exception.Message);
    }

    [Fact]
    public async Task GetAllProducts_ShouldReturnListOfProducts()
    {
        var products = new List<Product> { new Product { Id = 1, Name = "Product 1", Price = 10 } };
        var responseList = new List<ProductResponse> { new ProductResponse { Id = 1, Name = "Product 1", Price = 10 } };

        _productServiceMock.Setup(svc => svc.GetAllProductsAsync()).ReturnsAsync(products);
        _mapperMock.Setup(m => m.Map<IEnumerable<ProductResponse>>(products)).Returns(responseList);

        var result = await _controller.GetAllProducts();
        var okResult = Assert.IsType<OkObjectResult>(result);
        var returnedResponse = Assert.IsType<List<ProductResponse>>(okResult.Value);

        Assert.Single(returnedResponse);
    }

    [Fact]
    public async Task UpdateProduct_ShouldReturnOk_WhenProductExists()
    {
        var request = new ProductUpsertRequest { Name = "Updated Product", Price = 20.5m, Stock = 10 };
        var existingProduct = new Product { Id = 1, Name = "Old Product", Price = 15, Stock = 5 };

        _validatorMock.Setup(v => v.ValidateAsync(request, default))
            .ReturnsAsync(new ValidationResult());

        _productServiceMock.Setup(svc => svc.UpdateProductAsync(It.IsAny<Product>()))
            .Returns(Task.CompletedTask);

        var result = await _controller.UpdateProduct(1, request);

        Assert.IsType<OkObjectResult>(result);
    }

    [Fact]
    public async Task UpdateProduct_ShouldThrowKeyNotFoundException_WhenProductDoesNotExist()
    {
        var request = new ProductUpsertRequest { Name = "Updated Product", Price = 20.5m, Stock = 10 };

        _validatorMock.Setup(v => v.ValidateAsync(request, default))
            .ReturnsAsync(new ValidationResult());

        _productServiceMock
            .Setup(svc => svc.UpdateProductAsync(It.IsAny<Product>()))
            .ThrowsAsync(new KeyNotFoundException("Product not found"));

        var exception = await Assert.ThrowsAsync<KeyNotFoundException>(
            async () => await _controller.UpdateProduct(1, request)
        );

        Assert.Equal("Product not found", exception.Message);
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

        var exception = await Assert.ThrowsAsync<KeyNotFoundException>(
            async () => await _controller.DeleteProduct(1)
        );

        Assert.Equal("Product not found", exception.Message);
    }
}