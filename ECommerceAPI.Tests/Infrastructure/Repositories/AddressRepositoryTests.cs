using ECommerceAPI.Infrastructure.Context;
using ECommerceAPI.Infrastructure.Entities;
using ECommerceAPI.Infrastructure.Repositories;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;

namespace ECommerceAPI.Tests.Infrastructure.Repositories
{
  public class AddressRepositoryTests : IDisposable
  {
    private readonly AppDbContext _context;
    private readonly AddressRepository _repository;

    public AddressRepositoryTests()
    {
      var options = new DbContextOptionsBuilder<AppDbContext>()
          .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
          .Options;

      _context = new AppDbContext(options);
      _repository = new AddressRepository(_context);

      // Seed test data
      SeedTestData();
    }

    private void SeedTestData()
    {
      var user = new UserEntity
      {
        Id = 1,
        FullName = "Test User",
        Email = "test@example.com",
        Role = "User"
      };

      var addresses = new List<AddressEntity>
            {
                new AddressEntity
                {
                    Id = 1,
                    UserId = 1,
                    Name = "Home",
                    Street = "123 Main St",
                    City = "New York",
                    State = "NY",
                    Pincode = "123456",
                    Phone = "1234567890",
                    IsDefault = true
                },
                new AddressEntity
                {
                    Id = 2,
                    UserId = 1,
                    Name = "Office",
                    Street = "456 Business Ave",
                    City = "New York",
                    State = "NY",
                    Pincode = "654321",
                    Phone = "0987654321",
                    IsDefault = false
                }
            };

      _context.Users.Add(user);
      _context.Addresses.AddRange(addresses);
      _context.SaveChanges();
    }

    [Fact]
    public async Task CreateAsync_ShouldCreateAddress_WhenValidAddress()
    {
      // Arrange
      var newAddress = new AddressEntity
      {
        UserId = 1,
        Name = "New Address",
        Street = "789 New St",
        City = "Boston",
        State = "MA",
        Pincode = "111111",
        Phone = "1111111111",
        IsDefault = false
      };

      // Act
      var result = await _repository.CreateAsync(newAddress);

      // Assert
      result.Should().NotBeNull();
      result.Id.Should().BeGreaterThan(0);
      result.Name.Should().Be("New Address");

      var addressInDb = await _context.Addresses.FindAsync(result.Id);
      addressInDb.Should().NotBeNull();
      addressInDb.Name.Should().Be("New Address");
    }

    [Fact]
    public async Task GetByIdAsync_ShouldReturnAddress_WhenAddressExists()
    {
      // Act
      var result = await _repository.GetByIdAsync(1);

      // Assert
      result.Should().NotBeNull();
      result.Id.Should().Be(1);
      result.Name.Should().Be("Home");
      result.IsDefault.Should().BeTrue();
    }

    [Fact]
    public async Task GetByIdAsync_ShouldReturnNull_WhenAddressDoesNotExist()
    {
      // Act
      var result = await _repository.GetByIdAsync(999);

      // Assert
      result.Should().BeNull();
    }

    [Fact]
    public async Task GetByUserIdAsync_ShouldReturnUserAddresses_OrderedByDefault()
    {
      // Act
      var result = await _repository.GetByUserIdAsync(1);

      // Assert
      result.Should().NotBeNull();
      result.Should().HaveCount(2);

      var addressList = result.ToList();
      addressList[0].IsDefault.Should().BeTrue(); // Default address should be first
      addressList[0].Name.Should().Be("Home");
      addressList[1].IsDefault.Should().BeFalse();
      addressList[1].Name.Should().Be("Office");
    }

    [Fact]
    public async Task GetByUserIdAsync_ShouldReturnEmpty_WhenUserHasNoAddresses()
    {
      // Act
      var result = await _repository.GetByUserIdAsync(999);

      // Assert
      result.Should().NotBeNull();
      result.Should().BeEmpty();
    }

    [Fact]
    public async Task UpdateAsync_ShouldUpdateAddress_WhenAddressExists()
    {
      // Arrange
      var address = await _context.Addresses.FindAsync(1);
      address.Name = "Updated Home";
      address.Street = "Updated Street";

      // Act
      await _repository.UpdateAsync(address);

      // Assert
      var updatedAddress = await _context.Addresses.FindAsync(1);
      updatedAddress.Name.Should().Be("Updated Home");
      updatedAddress.Street.Should().Be("Updated Street");
    }

    [Fact]
    public async Task DeleteAsync_ShouldDeleteAddress_WhenAddressExists()
    {
      // Act
      await _repository.DeleteAsync(2);

      // Assert
      var deletedAddress = await _context.Addresses.FindAsync(2);
      deletedAddress.Should().BeNull();

      var remainingAddresses = await _context.Addresses.Where(a => a.UserId == 1).ToListAsync();
      remainingAddresses.Should().HaveCount(1);
    }

    [Fact]
    public async Task DeleteAsync_ShouldDoNothing_WhenAddressDoesNotExist()
    {
      // Arrange
      var initialCount = await _context.Addresses.CountAsync();

      // Act
      await _repository.DeleteAsync(999);

      // Assert
      var finalCount = await _context.Addresses.CountAsync();
      finalCount.Should().Be(initialCount);
    }

    [Fact]
    public async Task GetDefaultAddressByUserIdAsync_ShouldReturnDefaultAddress()
    {
      // Act
      var result = await _repository.GetDefaultAddressByUserIdAsync(1);

      // Assert
      result.Should().NotBeNull();
      result.IsDefault.Should().BeTrue();
      result.Name.Should().Be("Home");
    }

    [Fact]
    public async Task GetDefaultAddressByUserIdAsync_ShouldReturnNull_WhenNoDefaultAddress()
    {
      // Arrange - Remove default flag from all addresses
      var addresses = await _context.Addresses.Where(a => a.UserId == 1).ToListAsync();
      foreach (var addr in addresses)
      {
        addr.IsDefault = false;
      }
      await _context.SaveChangesAsync();

      // Act
      var result = await _repository.GetDefaultAddressByUserIdAsync(1);

      // Assert
      result.Should().BeNull();
    }

    [Fact]
    public async Task SetDefaultAddressAsync_ShouldSetCorrectAddressAsDefault()
    {
      // Act
      await _repository.SetDefaultAddressAsync(1, 2);

      // Assert
      var addresses = await _context.Addresses.Where(a => a.UserId == 1).ToListAsync();

      var address1 = addresses.First(a => a.Id == 1);
      var address2 = addresses.First(a => a.Id == 2);

      address1.IsDefault.Should().BeFalse();
      address2.IsDefault.Should().BeTrue();
    }

    [Fact]
    public async Task SetDefaultAddressAsync_ShouldUnsetAllOthers_WhenSettingNewDefault()
    {
      // Arrange - Add another address
      var newAddress = new AddressEntity
      {
        UserId = 1,
        Name = "Third Address",
        Street = "789 Third St",
        City = "Boston",
        State = "MA",
        Pincode = "333333",
        Phone = "3333333333",
        IsDefault = false
      };
      _context.Addresses.Add(newAddress);
      await _context.SaveChangesAsync();

      // Act
      await _repository.SetDefaultAddressAsync(1, newAddress.Id);

      // Assert
      var allAddresses = await _context.Addresses.Where(a => a.UserId == 1).ToListAsync();
      var defaultAddresses = allAddresses.Where(a => a.IsDefault).ToList();

      defaultAddresses.Should().HaveCount(1);
      defaultAddresses.First().Id.Should().Be(newAddress.Id);
    }

    public void Dispose()
    {
      _context.Dispose();
    }
  }
}