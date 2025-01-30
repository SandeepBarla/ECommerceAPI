using ECommerceAPI.Infrastructure.Context;
using ECommerceAPI.Infrastructure.Entities;
using ECommerceAPI.Infrastructure.Repositories;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;

namespace ECommerceAPI.Tests.Infrastructure.Repositories;

public class ProductRepositoryTests
{
    private readonly AppDbContext _dbContext;
    private readonly ProductRepository _productRepository;

    public ProductRepositoryTests()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: "TestDatabase")
            .Options;

        _dbContext = new AppDbContext(options);
        _productRepository = new ProductRepository(_dbContext);
        
        // Clear database before running each test
        _dbContext.Products.RemoveRange(_dbContext.Products);
        _dbContext.SaveChanges();

        // Seed initial data
        _dbContext.Products.AddRange(new List<ProductEntity>
        {
            new ProductEntity { Id = 1, Name = "Product A", Price = 10.99m, Stock = 50, Description = "Product Description", ImageUrl = "example.com" },
            new ProductEntity { Id = 2, Name = "Product B", Price = 19.99m, Stock = 30, Description = "Product Description", ImageUrl = "example.com"  }
        });
        _dbContext.SaveChanges();
    }

    [Fact]
    public async Task GetAllProductsAsync_ShouldReturnAllProducts()
    {
        // Act
        var result = await _productRepository.GetAllAsync();

        // Assert
        result.Should().NotBeNullOrEmpty();
        result.Count().Should().Be(2);
    }

    [Fact]
    public async Task GetByIdAsync_ShouldReturnProduct_WhenProductExists()
    {
        // Act
        var result = await _productRepository.GetByIdAsync(1);

        // Assert
        result.Should().NotBeNull();
        result.Name.Should().Be("Product A");
        result.Price.Should().Be(10.99m);
    }

    [Fact]
    public async Task GetByIdAsync_ShouldReturnNull_WhenProductDoesNotExist()
    {
        // Act
        var result = await _productRepository.GetByIdAsync(99);

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task CreateAsync_ShouldAddProductToDatabase()
    {
        // Arrange
        var newProduct = new ProductEntity
        {
            Id = 3, 
            Name = "Product C", 
            Price = 15.99m, 
            Stock = 20, 
            Description = "Product Description",
            ImageUrl = "example.com"
        };

        // Act
        await _productRepository.CreateAsync(newProduct);
        var result = await _productRepository.GetByIdAsync(3);

        // Assert
        result.Should().NotBeNull();
        result.Name.Should().Be("Product C");
        result.Price.Should().Be(15.99m);
    }

    [Fact]
    public async Task UpdateAsync_ShouldModifyExistingProduct()
    {
        // Arrange
        var product = await _productRepository.GetByIdAsync(1);
        product.Name = "Updated Product A";
        product.Price = 12.99m;

        // Act
        await _productRepository.UpdateAsync(product);
        var result = await _productRepository.GetByIdAsync(1);

        // Assert
        result.Should().NotBeNull();
        result.Name.Should().Be("Updated Product A");
        result.Price.Should().Be(12.99m);
    }

    [Fact]
    public async Task DeleteAsync_ShouldRemoveProductFromDatabase()
    {
        // Arrange & Act
        await _productRepository.DeleteAsync(1);
        var result = await _productRepository.GetByIdAsync(1);

        // Assert
        result.Should().BeNull();
    }
}