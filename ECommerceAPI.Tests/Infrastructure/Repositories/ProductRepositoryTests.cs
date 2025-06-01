using ECommerceAPI.Application.Models.Enums;
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
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString()) // ✅ Unique DB per test run
            .Options;

        _dbContext = new AppDbContext(options);
        _productRepository = new ProductRepository(_dbContext);

        // Clear database before running each test
        _dbContext.Products.RemoveRange(_dbContext.Products);
        _dbContext.SaveChanges();

        // Seed categories and sizes first (required for FK relationships)
        _dbContext.Categories.Add(new CategoryEntity { Id = 1, Name = "Test Category" });
        _dbContext.Sizes.Add(new SizeEntity { Id = 1, Name = "Test Size" });
        _dbContext.SaveChanges();

        // Seed initial data with simplified fields
        _dbContext.Products.AddRange(new List<ProductEntity>
        {
            new ProductEntity
            {
                Id = 1,
                Name = "Product A",
                Description = "Product A Description",
                OriginalPrice = 12.99m,
                DiscountedPrice = 10.99m,
                IsFeatured = true,
                NewUntil = DateTime.UtcNow.AddDays(30),
                CategoryId = 1,
                SizeId = 1,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                Media = new List<ProductMediaEntity>
                {
                    new() { Id = 1, ProductId = 1, MediaUrl = "https://example.com/image1.jpg", Type = MediaType.Image, OrderIndex = 1 },
                    new() { Id = 2, ProductId = 1, MediaUrl = "https://example.com/image2.jpg", Type = MediaType.Image, OrderIndex = 2 }
                }
            },
            new ProductEntity
            {
                Id = 2,
                Name = "Product B",
                Description = "Product B Description",
                OriginalPrice = 19.99m,
                IsFeatured = false,
                CategoryId = 1,
                SizeId = 1,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                Media = new List<ProductMediaEntity>
                {
                    new() { Id = 3, ProductId = 2, MediaUrl = "https://example.com/video1.mp4", Type = MediaType.Video, OrderIndex = 1 }
                }
            }
        });
        _dbContext.SaveChanges();
    }

    [Fact]
    public async Task GetAllProductsAsync_ShouldReturnAllProducts_WithOnlyPrimaryImage()
    {
        // Act
        var result = await _productRepository.GetAllAsync();

        // Assert
        result.Should().NotBeNullOrEmpty();
        result.Count().Should().Be(2);

        var productA = result.First(p => p.Id == 1);
        productA.Media.Should().HaveCount(1);
        productA.Media.First().MediaUrl.Should().Be("https://example.com/image1.jpg"); // ✅ Only OrderIndex 1

        var productB = result.First(p => p.Id == 2);
        productB.Media.Should().HaveCount(1);
        productB.Media.First().MediaUrl.Should().Be("https://example.com/video1.mp4"); // ✅ Only OrderIndex 1

        // Verify simplified fields are included
        productA.IsFeatured.Should().BeTrue();
    }

    [Fact]
    public async Task GetByIdAsync_ShouldReturnProductWithAllMedia_WhenProductExists()
    {
        // Act
        var result = await _productRepository.GetByIdAsync(1);

        // Assert
        result.Should().NotBeNull();
        result.Name.Should().Be("Product A");
        result.Media.Should().HaveCount(2); // ✅ Fetches all media
        result.Category.Should().NotBeNull();
        result.Category.Name.Should().Be("Test Category");
        result.Size.Should().NotBeNull();
        result.Size.Name.Should().Be("Test Size");
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
            Description = "Product C Description",
            OriginalPrice = 17.99m,
            DiscountedPrice = 15.99m,
            IsFeatured = false,
            CategoryId = 1,
            SizeId = 1,
            Media = new List<ProductMediaEntity>
            {
                new() { Id = 4, ProductId = 3, MediaUrl = "https://example.com/image3.jpg", Type = MediaType.Image, OrderIndex = 1 }
            }
        };

        // Act
        await _productRepository.CreateAsync(newProduct);
        var result = await _productRepository.GetByIdAsync(3);

        // Assert
        result.Should().NotBeNull();
        result.Name.Should().Be("Product C");
        result.Media.Should().HaveCount(1);
        result.CreatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(10));
        result.UpdatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(10));
    }

    [Fact]
    public async Task UpdateAsync_ShouldModifyExistingProduct()
    {
        // Arrange
        var product = await _productRepository.GetByIdAsync(1);
        product.Should().NotBeNull();
        product.Name = "Updated Product A";
        product.OriginalPrice = 12.99m;

        // Act
        await _productRepository.UpdateAsync(product);
        var result = await _productRepository.GetByIdAsync(1);

        // Assert
        result.Should().NotBeNull();
        result.Name.Should().Be("Updated Product A");
        result.OriginalPrice.Should().Be(12.99m);
        result.UpdatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(10));
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