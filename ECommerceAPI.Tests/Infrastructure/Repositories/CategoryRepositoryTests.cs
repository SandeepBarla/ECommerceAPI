using ECommerceAPI.Infrastructure.Context;
using ECommerceAPI.Infrastructure.Entities;
using ECommerceAPI.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using FluentAssertions;
using Xunit;

namespace ECommerceAPI.Tests.Infrastructure.Repositories;

public class CategoryRepositoryTests : IDisposable
{
  private readonly AppDbContext _dbContext;
  private readonly CategoryRepository _repository;

  public CategoryRepositoryTests()
  {
    var options = new DbContextOptionsBuilder<AppDbContext>()
        .UseInMemoryDatabase(databaseName: $"TestDb_CategoryRepo_{Guid.NewGuid()}")
        .Options;

    _dbContext = new AppDbContext(options);
    _repository = new CategoryRepository(_dbContext);

    _dbContext.Database.EnsureDeleted();
    _dbContext.Database.EnsureCreated();
  }

  [Fact]
  public async Task GetAllAsync_ShouldReturnAllCategories()
  {
    // Arrange
    var categories = new List<CategoryEntity>
        {
            new CategoryEntity { Name = "Lehenga" },
            new CategoryEntity { Name = "Saree" },
            new CategoryEntity { Name = "Kurti" }
        };

    await _dbContext.Categories.AddRangeAsync(categories);
    await _dbContext.SaveChangesAsync();

    // Act
    var result = await _repository.GetAllAsync();

    // Assert
    result.Should().HaveCount(3);
    result.Should().Contain(c => c.Name == "Lehenga");
    result.Should().Contain(c => c.Name == "Saree");
    result.Should().Contain(c => c.Name == "Kurti");
  }

  [Fact]
  public async Task GetByIdAsync_WithValidId_ShouldReturnCategory()
  {
    // Arrange
    var category = new CategoryEntity { Name = "Lehenga" };
    await _dbContext.Categories.AddAsync(category);
    await _dbContext.SaveChangesAsync();

    // Act
    var result = await _repository.GetByIdAsync(category.Id);

    // Assert
    result.Should().NotBeNull();
    result!.Name.Should().Be("Lehenga");
  }

  [Fact]
  public async Task GetByIdAsync_WithInvalidId_ShouldReturnNull()
  {
    // Act
    var result = await _repository.GetByIdAsync(999);

    // Assert
    result.Should().BeNull();
  }

  [Fact]
  public async Task CreateAsync_ShouldAddCategoryToDatabase()
  {
    // Arrange
    var category = new CategoryEntity { Name = "Saree" };

    // Act
    await _repository.CreateAsync(category);

    // Assert
    var dbCategory = await _dbContext.Categories.FirstOrDefaultAsync(c => c.Name == "Saree");
    dbCategory.Should().NotBeNull();
    dbCategory!.Name.Should().Be("Saree");
    dbCategory.Id.Should().BeGreaterThan(0);
  }

  [Fact]
  public async Task UpdateAsync_WithValidCategory_ShouldUpdateCategory()
  {
    // Arrange
    var category = new CategoryEntity { Name = "Lehenga" };
    await _dbContext.Categories.AddAsync(category);
    await _dbContext.SaveChangesAsync();

    // Act
    category.Name = "Updated Lehenga";
    await _repository.UpdateAsync(category);

    // Assert
    var dbCategory = await _dbContext.Categories.FindAsync(category.Id);
    dbCategory!.Name.Should().Be("Updated Lehenga");
  }

  [Fact]
  public async Task DeleteAsync_WithValidId_ShouldDeleteCategory()
  {
    // Arrange
    var category = new CategoryEntity { Name = "Saree" };
    await _dbContext.Categories.AddAsync(category);
    await _dbContext.SaveChangesAsync();
    var categoryId = category.Id;

    // Act
    await _repository.DeleteAsync(categoryId);

    // Assert
    var dbCategory = await _dbContext.Categories.FindAsync(categoryId);
    dbCategory.Should().BeNull();
  }

  [Fact]
  public async Task DeleteAsync_WithInvalidId_ShouldNotThrow()
  {
    // Act & Assert - should not throw even if category doesn't exist
    var act = async () => await _repository.DeleteAsync(999);
    await act.Should().NotThrowAsync();
  }

  public void Dispose()
  {
    _dbContext.Dispose();
  }
}