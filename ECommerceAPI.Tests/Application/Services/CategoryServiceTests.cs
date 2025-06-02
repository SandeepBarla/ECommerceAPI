using ECommerceAPI.Application.Interfaces;
using ECommerceAPI.Application.Models;
using ECommerceAPI.Application.Services;
using ECommerceAPI.Infrastructure.Entities;
using ECommerceAPI.Infrastructure.Interfaces;
using ECommerceAPI.Tests.Common;
using ECommerceAPI.WebApi.DTOs.ResponseModels;
using AutoMapper;
using FluentAssertions;
using Moq;
using Xunit;

namespace ECommerceAPI.Tests.Application.Services;

public class CategoryServiceTests : TestBase
{
  private readonly Mock<ICategoryRepository> _mockRepository;
  private readonly CategoryService _service;

  public CategoryServiceTests()
  {
    _mockRepository = new Mock<ICategoryRepository>();
    _service = new CategoryService(_mockRepository.Object, Mapper);
  }

  [Fact]
  public async Task GetAllCategoriesAsync_ShouldReturnMappedCategories()
  {
    // Arrange
    var categoryEntities = new List<CategoryEntity>
        {
            new CategoryEntity { Id = 1, Name = "Lehenga" },
            new CategoryEntity { Id = 2, Name = "Saree" }
        };

    _mockRepository.Setup(r => r.GetAllAsync())
        .ReturnsAsync(categoryEntities);

    // Act
    var result = await _service.GetAllCategoriesAsync();

    // Assert
    result.Should().HaveCount(2);
    result.Should().Contain(c => c.Name == "Lehenga");
    result.Should().Contain(c => c.Name == "Saree");
    _mockRepository.Verify(r => r.GetAllAsync(), Times.Once);
  }

  [Fact]
  public async Task GetCategoryByIdAsync_WithValidId_ShouldReturnMappedCategory()
  {
    // Arrange
    var categoryEntity = new CategoryEntity { Id = 1, Name = "Lehenga" };

    _mockRepository.Setup(r => r.GetByIdAsync(1))
        .ReturnsAsync(categoryEntity);

    // Act
    var result = await _service.GetCategoryByIdAsync(1);

    // Assert
    result.Should().NotBeNull();
    result!.Name.Should().Be("Lehenga");
    _mockRepository.Verify(r => r.GetByIdAsync(1), Times.Once);
  }

  [Fact]
  public async Task GetCategoryByIdAsync_WithInvalidId_ShouldThrowException()
  {
    // Arrange
    _mockRepository.Setup(r => r.GetByIdAsync(999))
        .ReturnsAsync((CategoryEntity?)null);

    // Act & Assert
    var act = async () => await _service.GetCategoryByIdAsync(999);
    await act.Should().ThrowAsync<KeyNotFoundException>()
        .WithMessage("Category not found");
  }

  [Fact]
  public async Task CreateCategoryAsync_WithValidCategory_ShouldCreateCategory()
  {
    // Arrange
    var category = new Category { Name = "Kurti" };

    _mockRepository.Setup(r => r.CreateAsync(It.IsAny<CategoryEntity>()))
        .Returns(Task.CompletedTask);

    // Act
    var result = await _service.CreateCategoryAsync(category);

    // Assert
    result.Should().NotBeNull();
    result.Name.Should().Be("Kurti");
    _mockRepository.Verify(r => r.CreateAsync(It.Is<CategoryEntity>(c => c.Name == "Kurti")), Times.Once);
  }

  [Fact]
  public async Task UpdateCategoryAsync_WithValidCategory_ShouldUpdateCategory()
  {
    // Arrange
    var existingEntity = new CategoryEntity { Id = 1, Name = "Lehenga" };
    var categoryToUpdate = new Category { Id = 1, Name = "Updated Lehenga" };

    _mockRepository.Setup(r => r.GetByIdAsync(1))
        .ReturnsAsync(existingEntity);
    _mockRepository.Setup(r => r.UpdateAsync(It.IsAny<CategoryEntity>()))
        .Returns(Task.CompletedTask);

    // Act
    await _service.UpdateCategoryAsync(categoryToUpdate);

    // Assert
    _mockRepository.Verify(r => r.GetByIdAsync(1), Times.Once);
    _mockRepository.Verify(r => r.UpdateAsync(It.Is<CategoryEntity>(c => c.Name == "Updated Lehenga")), Times.Once);
  }

  [Fact]
  public async Task UpdateCategoryAsync_WithInvalidId_ShouldThrowException()
  {
    // Arrange
    _mockRepository.Setup(r => r.GetByIdAsync(999))
        .ReturnsAsync((CategoryEntity?)null);

    // Act & Assert
    var act = async () => await _service.UpdateCategoryAsync(new Category { Id = 999, Name = "New Name" });
    await act.Should().ThrowAsync<KeyNotFoundException>()
        .WithMessage("Category not found");
  }

  [Fact]
  public async Task DeleteCategoryAsync_WithValidId_ShouldDeleteCategory()
  {
    // Arrange
    var existingEntity = new CategoryEntity { Id = 1, Name = "Lehenga" };

    _mockRepository.Setup(r => r.GetByIdAsync(1))
        .ReturnsAsync(existingEntity);
    _mockRepository.Setup(r => r.DeleteAsync(1))
        .Returns(Task.CompletedTask);

    // Act
    await _service.DeleteCategoryAsync(1);

    // Assert
    _mockRepository.Verify(r => r.GetByIdAsync(1), Times.Once);
    _mockRepository.Verify(r => r.DeleteAsync(1), Times.Once);
  }

  [Fact]
  public async Task DeleteCategoryAsync_WithInvalidId_ShouldThrowException()
  {
    // Arrange
    _mockRepository.Setup(r => r.GetByIdAsync(999))
        .ReturnsAsync((CategoryEntity?)null);

    // Act & Assert
    var act = async () => await _service.DeleteCategoryAsync(999);
    await act.Should().ThrowAsync<KeyNotFoundException>()
        .WithMessage("Category not found");
  }
}