using ECommerceAPI.Infrastructure.Context;
using ECommerceAPI.Infrastructure.Entities;
using Microsoft.EntityFrameworkCore;
using FluentAssertions;
using Xunit;

namespace ECommerceAPI.Tests.Infrastructure;

public class DataSeederTests : IDisposable
{
  private readonly AppDbContext _dbContext;

  public DataSeederTests()
  {
    var options = new DbContextOptionsBuilder<AppDbContext>()
        .UseInMemoryDatabase(databaseName: $"TestDb_DataSeeder_{Guid.NewGuid()}")
        .Options;

    _dbContext = new AppDbContext(options);
    _dbContext.Database.EnsureDeleted();
    _dbContext.Database.EnsureCreated();
  }

  [Fact]
  public async Task SeedDataAsync_WithEmptyDatabase_ShouldCreateBothCategories()
  {
    // Arrange - Empty database

    // Act
    await DataSeeder.SeedDataAsync(_dbContext);

    // Assert
    var categories = await _dbContext.Categories.ToListAsync();
    categories.Should().HaveCount(2);
    categories.Should().Contain(c => c.Name == "Lehenga");
    categories.Should().Contain(c => c.Name == "Saree");
  }

  [Fact]
  public async Task SeedDataAsync_WithExistingLehengaCategory_ShouldAddSareeCategory()
  {
    // Arrange
    var existingCategory = new CategoryEntity { Name = "Lehenga" };
    await _dbContext.Categories.AddAsync(existingCategory);
    await _dbContext.SaveChangesAsync();

    // Act
    await DataSeeder.SeedDataAsync(_dbContext);

    // Assert
    var categories = await _dbContext.Categories.ToListAsync();
    categories.Should().HaveCount(2);
    categories.Should().Contain(c => c.Name == "Lehenga");
    categories.Should().Contain(c => c.Name == "Saree");
  }

  [Fact]
  public async Task SeedDataAsync_WithExistingSareeCategory_ShouldNotDuplicateSaree()
  {
    // Arrange
    var existingCategories = new List<CategoryEntity>
        {
            new CategoryEntity { Name = "Lehenga" },
            new CategoryEntity { Name = "Saree" }
        };
    await _dbContext.Categories.AddRangeAsync(existingCategories);
    await _dbContext.SaveChangesAsync();

    // Act
    await DataSeeder.SeedDataAsync(_dbContext);

    // Assert
    var categories = await _dbContext.Categories.ToListAsync();
    categories.Should().HaveCount(2);
    categories.Count(c => c.Name == "Saree").Should().Be(1);
    categories.Count(c => c.Name == "Lehenga").Should().Be(1);
  }

  [Fact]
  public async Task SeedDataAsync_WithEmptyDatabase_ShouldCreateDefaultSizes()
  {
    // Arrange - Empty database

    // Act
    await DataSeeder.SeedDataAsync(_dbContext);

    // Assert
    var sizes = await _dbContext.Sizes.ToListAsync();
    sizes.Should().HaveCount(6);
    sizes.Should().Contain(s => s.Name == "Free Size");
    sizes.Should().Contain(s => s.Name == "S");
    sizes.Should().Contain(s => s.Name == "M");
    sizes.Should().Contain(s => s.Name == "L");
    sizes.Should().Contain(s => s.Name == "XL");
    sizes.Should().Contain(s => s.Name == "XXL");
  }

  [Fact]
  public async Task SeedDataAsync_WithExistingSizes_ShouldNotDuplicateSizes()
  {
    // Arrange
    var existingSizes = new List<SizeEntity>
        {
            new SizeEntity { Name = "S" },
            new SizeEntity { Name = "M" }
        };
    await _dbContext.Sizes.AddRangeAsync(existingSizes);
    await _dbContext.SaveChangesAsync();

    // Act
    await DataSeeder.SeedDataAsync(_dbContext);

    // Assert
    var sizes = await _dbContext.Sizes.ToListAsync();
    sizes.Should().HaveCount(2); // Should not add more sizes if any exist
    sizes.Should().Contain(s => s.Name == "S");
    sizes.Should().Contain(s => s.Name == "M");
  }

  [Fact]
  public async Task SeedDataAsync_ShouldBeSafeToCallMultipleTimes()
  {
    // Act - Call seeding multiple times
    await DataSeeder.SeedDataAsync(_dbContext);
    await DataSeeder.SeedDataAsync(_dbContext);
    await DataSeeder.SeedDataAsync(_dbContext);

    // Assert
    var categories = await _dbContext.Categories.ToListAsync();
    var sizes = await _dbContext.Sizes.ToListAsync();

    categories.Should().HaveCount(2);
    sizes.Should().HaveCount(6);

    // Verify no duplicates
    categories.Count(c => c.Name == "Lehenga").Should().Be(1);
    categories.Count(c => c.Name == "Saree").Should().Be(1);
  }

  public void Dispose()
  {
    _dbContext.Dispose();
  }
}