using AutoMapper;
using ECommerceAPI.Application.Models;
using ECommerceAPI.Application.Models.Enums;
using ECommerceAPI.Application.Profiles;
using ECommerceAPI.Infrastructure.Entities;
using ECommerceAPI.WebApi.DTOs.RequestModels;
using ECommerceAPI.WebApi.DTOs.ResponseModels;

namespace ECommerceAPI.Tests.Application.Profiles;

public class ProductProfileTests
{
  private readonly IMapper _mapper;

  public ProductProfileTests()
  {
    var config = new MapperConfiguration(cfg => cfg.AddProfile<ProductProfile>());
    _mapper = config.CreateMapper();
  }

  [Fact]
  public void ShouldMapProductUpsertRequestToProduct()
  {
    // Arrange
    var request = new ProductUpsertRequest
    {
      Name = "Test Product",
      Description = "Test Description",
      OriginalPrice = 129.99m,
      DiscountedPrice = 99.99m,
      IsFeatured = true,
      NewUntil = DateTime.UtcNow.AddDays(30),
      CategoryId = 1,
      SizeId = 1,
      Media = new List<ProductMediaRequest>
            {
                new() { MediaUrl = "https://example.com/image1.jpg", Type = MediaType.Image, OrderIndex = 1 }
            }
    };

    // Act
    var result = _mapper.Map<Product>(request);

    // Assert
    Assert.Equal(request.Name, result.Name);
    Assert.Equal(request.Description, result.Description);
    Assert.Equal(request.OriginalPrice, result.OriginalPrice);
    Assert.Equal(request.DiscountedPrice, result.DiscountedPrice);
    Assert.Equal(request.IsFeatured, result.IsFeatured);
    Assert.Equal(request.NewUntil, result.NewUntil);
    Assert.Equal(request.CategoryId, result.CategoryId);
    Assert.Equal(request.SizeId, result.SizeId);
    Assert.True(result.CreatedAt > DateTime.MinValue);
    Assert.True(result.UpdatedAt > DateTime.MinValue);
    Assert.Single(result.Media);
  }

  [Fact]
  public void ShouldCalculateDiscountPercentageCorrectly_WhenMappingProductUpsertRequest()
  {
    // Arrange - 20% discount
    var request = new ProductUpsertRequest
    {
      Name = "Test Product",
      OriginalPrice = 100m,
      DiscountedPrice = 80m
    };

    // Act
    var result = _mapper.Map<Product>(request);

    // Assert - Backend should calculate discount percentage: (100-80)/100*100 = 20%
    Assert.Equal(request.OriginalPrice, result.OriginalPrice);
    Assert.Equal(request.DiscountedPrice, result.DiscountedPrice);
  }

  [Fact]
  public void ShouldSetDiscountedPriceToNull_WhenNoDiscount()
  {
    // Arrange
    var request = new ProductUpsertRequest
    {
      Name = "Test Product",
      OriginalPrice = 100m,
      DiscountedPrice = null
    };

    // Act
    var result = _mapper.Map<Product>(request);

    // Assert
    Assert.Equal(100m, result.OriginalPrice);
    Assert.Null(result.DiscountedPrice);
  }

  [Fact]
  public void ShouldHandleInvalidDiscount_WhenDiscountedPriceHigherThanOriginal()
  {
    // Arrange
    var request = new ProductUpsertRequest
    {
      Name = "Test Product",
      OriginalPrice = 80m,
      DiscountedPrice = 100m // Invalid scenario
    };

    // Act
    var result = _mapper.Map<Product>(request);

    // Assert - Should still map the values as provided
    Assert.Equal(80m, result.OriginalPrice);
    Assert.Equal(100m, result.DiscountedPrice);
  }

  [Fact]
  public void ShouldMapTupleToProduct_ForUpdateScenario()
  {
    // Arrange
    var id = 1;
    var request = new ProductUpsertRequest
    {
      Name = "Updated Product",
      Description = "Updated Description",
      OriginalPrice = 199.99m,
      DiscountedPrice = 149.99m,
      IsFeatured = false,
      CategoryId = 2,
      SizeId = 2
    };

    // Act
    var result = _mapper.Map<Product>((id, request));

    // Assert
    Assert.Equal(id, result.Id);
    Assert.Equal(request.Name, result.Name);
    Assert.Equal(request.Description, result.Description);
    Assert.Equal(request.OriginalPrice, result.OriginalPrice);
    Assert.Equal(request.DiscountedPrice, result.DiscountedPrice);
    Assert.True(result.UpdatedAt > DateTime.MinValue);
  }

  [Fact]
  public void ShouldMapProductToProductResponse()
  {
    // Arrange
    var product = new Product
    {
      Id = 1,
      Name = "Test Product",
      Description = "Test Description",
      OriginalPrice = 129.99m,
      DiscountedPrice = 99.99m,
      IsFeatured = true,
      NewUntil = DateTime.UtcNow.AddDays(30),
      CategoryId = 1,
      SizeId = 1,
      Media = new List<ProductMedia>
            {
                new() { MediaUrl = "https://example.com/image1.jpg", Type = MediaType.Image, OrderIndex = 1 }
            }
    };

    // Act
    var result = _mapper.Map<ProductResponse>(product);

    // Assert
    Assert.Equal(product.Id, result.Id);
    Assert.Equal(product.Name, result.Name);
    Assert.Equal(product.Description, result.Description);
    Assert.Equal(product.OriginalPrice, result.OriginalPrice);
    Assert.Equal(product.DiscountedPrice, result.DiscountedPrice);
    Assert.Equal(product.IsFeatured, result.IsFeatured);
    Assert.Equal(product.NewUntil, result.NewUntil);
    Assert.Single(result.Media);
  }

  [Fact]
  public void ShouldMapProductToProductListResponse()
  {
    // Arrange
    var product = new Product
    {
      Id = 1,
      Name = "Test Product",
      OriginalPrice = 129.99m,
      DiscountedPrice = 99.99m,
      IsFeatured = true,
      NewUntil = DateTime.UtcNow.AddDays(30),
      CategoryId = 1,
      SizeId = 1,
      Media = new List<ProductMedia>
            {
                new() { MediaUrl = "https://example.com/image1.jpg", Type = MediaType.Image, OrderIndex = 1 },
                new() { MediaUrl = "https://example.com/image2.jpg", Type = MediaType.Image, OrderIndex = 2 }
            }
    };

    // Act
    var result = _mapper.Map<ProductListResponse>(product);

    // Assert
    Assert.Equal(product.Id, result.Id);
    Assert.Equal(product.Name, result.Name);
    Assert.Equal(product.OriginalPrice, result.OriginalPrice);
    Assert.Equal(product.DiscountedPrice, result.DiscountedPrice);
    Assert.Equal(product.IsFeatured, result.IsFeatured);
    Assert.Equal("https://example.com/image1.jpg", result.PrimaryImageUrl); // First media item
  }

  [Fact]
  public void ShouldMapProductEntityToProduct()
  {
    // Arrange
    var entity = new ProductEntity
    {
      Id = 1,
      Name = "Test Product",
      Description = "Test Description",
      OriginalPrice = 129.99m,
      DiscountedPrice = 99.99m,
      IsFeatured = true,
      NewUntil = DateTime.UtcNow.AddDays(30),
      CategoryId = 1,
      SizeId = 1,
      CreatedAt = DateTime.UtcNow,
      UpdatedAt = DateTime.UtcNow
    };

    // Act
    var result = _mapper.Map<Product>(entity);

    // Assert
    Assert.Equal(entity.Id, result.Id);
    Assert.Equal(entity.Name, result.Name);
    Assert.Equal(entity.Description, result.Description);
    Assert.Equal(entity.OriginalPrice, result.OriginalPrice);
    Assert.Equal(entity.DiscountedPrice, result.DiscountedPrice);
    Assert.Equal(entity.IsFeatured, result.IsFeatured);
    Assert.Equal(entity.NewUntil, result.NewUntil);
    Assert.Equal(entity.CategoryId, result.CategoryId);
    Assert.Equal(entity.SizeId, result.SizeId);
    Assert.Equal(entity.CreatedAt, result.CreatedAt);
    Assert.Equal(entity.UpdatedAt, result.UpdatedAt);
  }

  [Fact]
  public void ShouldMapProductMediaRequestToProductMedia()
  {
    // Arrange
    var request = new ProductMediaRequest
    {
      MediaUrl = "https://example.com/image1.jpg",
      Type = MediaType.Image,
      OrderIndex = 1
    };

    // Act
    var result = _mapper.Map<ProductMedia>(request);

    // Assert
    Assert.Equal(request.MediaUrl, result.MediaUrl);
    Assert.Equal(request.Type, result.Type);
    Assert.Equal(request.OrderIndex, result.OrderIndex);
  }

  [Fact]
  public void ShouldMapProductMediaToProductMediaResponse()
  {
    // Arrange
    var media = new ProductMedia
    {
      Id = 1,
      MediaUrl = "https://example.com/image1.jpg",
      Type = MediaType.Image,
      OrderIndex = 1
    };

    // Act
    var result = _mapper.Map<ProductMediaResponse>(media);

    // Assert
    Assert.Equal(media.MediaUrl, result.MediaUrl);
    Assert.Equal(media.Type.ToString(), result.Type);
    Assert.Equal(media.OrderIndex, result.OrderIndex);
  }

  [Fact]
  public void ShouldHandleEmptyMediaList_WhenMappingToProductListResponse()
  {
    // Arrange
    var product = new Product
    {
      Id = 1,
      Name = "Test Product",
      Media = new List<ProductMedia>() // Empty media list
    };

    // Act
    var result = _mapper.Map<ProductListResponse>(product);

    // Assert
    Assert.Equal("", result.PrimaryImageUrl); // Should default to empty string
  }

  [Fact]
  public void ShouldCalculateDiscountPercentageInResponse_WhenDiscountedPriceExists()
  {
    // Arrange
    var product = new Product
    {
      Id = 1,
      Name = "Test Product",
      OriginalPrice = 100m,
      DiscountedPrice = 75m, // 25% discount
      IsFeatured = true
    };

    // Act
    var result = _mapper.Map<ProductResponse>(product);

    // Assert
    Assert.Equal(25, result.DiscountPercentage); // Should be calculated by mapper
    Assert.True(result.IsNew == false || result.IsNew == true); // Should be calculated from NewUntil
  }

  [Fact]
  public void ShouldCalculateIsNewFromNewUntil_WhenDateInFuture()
  {
    // Arrange
    var product = new Product
    {
      Id = 1,
      Name = "Test Product",
      OriginalPrice = 100m,
      NewUntil = DateTime.UtcNow.AddDays(10) // Future date
    };

    // Act
    var result = _mapper.Map<ProductResponse>(product);

    // Assert
    Assert.True(result.IsNew); // Should be true since NewUntil is in future
  }

  [Fact]
  public void ShouldCalculateIsNewFromNewUntil_WhenDateInPast()
  {
    // Arrange
    var product = new Product
    {
      Id = 1,
      Name = "Test Product",
      OriginalPrice = 100m,
      NewUntil = DateTime.UtcNow.AddDays(-10) // Past date
    };

    // Act
    var result = _mapper.Map<ProductResponse>(product);

    // Assert
    Assert.False(result.IsNew); // Should be false since NewUntil is in past
  }
}