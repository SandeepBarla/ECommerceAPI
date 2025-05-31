using Moq;
using ECommerceAPI.Application.Interfaces;
using ECommerceAPI.Application.Models;
using ECommerceAPI.Application.Services;
using ECommerceAPI.Infrastructure.Entities;
using ECommerceAPI.Infrastructure.Repositories.Interfaces;
using ECommerceAPI.Tests.Common;
using FluentAssertions;

namespace ECommerceAPI.Tests.Application.Services
{
  public class AddressServiceTests : TestBase
  {
    private readonly AddressService _addressService;
    private readonly Mock<IAddressRepository> _addressRepositoryMock;

    public AddressServiceTests()
    {
      _addressRepositoryMock = new Mock<IAddressRepository>();
      _addressService = new AddressService(_addressRepositoryMock.Object, Mapper);
    }

    [Fact]
    public async Task CreateAddressAsync_ShouldCreateAddress_WhenValidRequest()
    {
      // Arrange
      var address = new Address
      {
        UserId = 1,
        Name = "John Doe",
        Street = "123 Main St",
        City = "New York",
        State = "NY",
        Pincode = "123456",
        Phone = "1234567890",
        IsDefault = false
      };

      var addressEntity = new AddressEntity
      {
        Id = 1,
        UserId = 1,
        Name = "John Doe",
        Street = "123 Main St",
        City = "New York",
        State = "NY",
        Pincode = "123456",
        Phone = "1234567890",
        IsDefault = false
      };

      _addressRepositoryMock.Setup(repo => repo.CreateAsync(It.IsAny<AddressEntity>()))
          .ReturnsAsync(addressEntity);

      // Act
      var result = await _addressService.CreateAddressAsync(address);

      // Assert
      result.Should().NotBeNull();
      result.Name.Should().Be("John Doe");
      result.UserId.Should().Be(1);
      _addressRepositoryMock.Verify(repo => repo.CreateAsync(It.IsAny<AddressEntity>()), Times.Once);
    }

    [Fact]
    public async Task CreateAddressAsync_ShouldSetAsDefault_WhenIsDefaultIsTrue()
    {
      // Arrange
      var address = new Address
      {
        UserId = 1,
        Name = "John Doe",
        Street = "123 Main St",
        City = "New York",
        State = "NY",
        Pincode = "123456",
        Phone = "1234567890",
        IsDefault = true
      };

      var addressEntity = new AddressEntity { Id = 1, UserId = 1, IsDefault = true };

      _addressRepositoryMock.Setup(repo => repo.CreateAsync(It.IsAny<AddressEntity>()))
          .ReturnsAsync(addressEntity);
      _addressRepositoryMock.Setup(repo => repo.SetDefaultAddressAsync(1, 0))
          .Returns(Task.CompletedTask);

      // Act
      var result = await _addressService.CreateAddressAsync(address);

      // Assert
      result.Should().NotBeNull();
      _addressRepositoryMock.Verify(repo => repo.SetDefaultAddressAsync(1, 0), Times.Once);
      _addressRepositoryMock.Verify(repo => repo.CreateAsync(It.IsAny<AddressEntity>()), Times.Once);
    }

    [Fact]
    public async Task GetAddressByIdAsync_ShouldReturnAddress_WhenAddressExists()
    {
      // Arrange
      var addressEntity = new AddressEntity
      {
        Id = 1,
        UserId = 1,
        Name = "John Doe",
        Street = "123 Main St",
        City = "New York",
        State = "NY",
        Pincode = "123456",
        Phone = "1234567890",
        IsDefault = false
      };

      _addressRepositoryMock.Setup(repo => repo.GetByIdAsync(1))
          .ReturnsAsync(addressEntity);

      // Act
      var result = await _addressService.GetAddressByIdAsync(1);

      // Assert
      result.Should().NotBeNull();
      result.Id.Should().Be(1);
      result.Name.Should().Be("John Doe");
      _addressRepositoryMock.Verify(repo => repo.GetByIdAsync(1), Times.Once);
    }

    [Fact]
    public async Task GetAddressByIdAsync_ShouldReturnNull_WhenAddressDoesNotExist()
    {
      // Arrange
      _addressRepositoryMock.Setup(repo => repo.GetByIdAsync(999))
          .ReturnsAsync((AddressEntity)null);

      // Act
      var result = await _addressService.GetAddressByIdAsync(999);

      // Assert
      result.Should().BeNull();
      _addressRepositoryMock.Verify(repo => repo.GetByIdAsync(999), Times.Once);
    }

    [Fact]
    public async Task GetAddressesByUserIdAsync_ShouldReturnUserAddresses()
    {
      // Arrange
      var addressEntities = new List<AddressEntity>
            {
                new AddressEntity { Id = 1, UserId = 1, Name = "Home", IsDefault = true },
                new AddressEntity { Id = 2, UserId = 1, Name = "Office", IsDefault = false }
            };

      _addressRepositoryMock.Setup(repo => repo.GetByUserIdAsync(1))
          .ReturnsAsync(addressEntities);

      // Act
      var result = await _addressService.GetAddressesByUserIdAsync(1);

      // Assert
      result.Should().NotBeNull();
      result.Should().HaveCount(2);
      result.First().IsDefault.Should().BeTrue();
      _addressRepositoryMock.Verify(repo => repo.GetByUserIdAsync(1), Times.Once);
    }

    [Fact]
    public async Task UpdateAddressAsync_ShouldThrowKeyNotFoundException_WhenAddressDoesNotExist()
    {
      // Arrange
      var address = new Address { Id = 999, UserId = 1, Name = "Test" };
      _addressRepositoryMock.Setup(repo => repo.GetByIdAsync(999))
          .ReturnsAsync((AddressEntity)null);

      // Act
      var act = async () => await _addressService.UpdateAddressAsync(address);

      // Assert
      await act.Should().ThrowAsync<KeyNotFoundException>()
          .WithMessage("Address not found.");
    }

    [Fact]
    public async Task UpdateAddressAsync_ShouldUpdateAddress_WhenAddressExists()
    {
      // Arrange
      var existingAddress = new AddressEntity { Id = 1, UserId = 1, IsDefault = false };
      var updatedAddress = new Address
      {
        Id = 1,
        UserId = 1,
        Name = "Updated Name",
        IsDefault = false
      };

      _addressRepositoryMock.Setup(repo => repo.GetByIdAsync(1))
          .ReturnsAsync(existingAddress);
      _addressRepositoryMock.Setup(repo => repo.UpdateAsync(It.IsAny<AddressEntity>()))
          .Returns(Task.CompletedTask);

      // Act
      await _addressService.UpdateAddressAsync(updatedAddress);

      // Assert
      _addressRepositoryMock.Verify(repo => repo.UpdateAsync(It.IsAny<AddressEntity>()), Times.Once);
    }

    [Fact]
    public async Task DeleteAddressAsync_ShouldThrowKeyNotFoundException_WhenAddressDoesNotExist()
    {
      // Arrange
      _addressRepositoryMock.Setup(repo => repo.GetByIdAsync(999))
          .ReturnsAsync((AddressEntity)null);

      // Act
      var act = async () => await _addressService.DeleteAddressAsync(999);

      // Assert
      await act.Should().ThrowAsync<KeyNotFoundException>()
          .WithMessage("Address not found.");
    }

    [Fact]
    public async Task DeleteAddressAsync_ShouldDeleteAddress_WhenAddressExists()
    {
      // Arrange
      var address = new AddressEntity { Id = 1, UserId = 1, IsDefault = false };
      _addressRepositoryMock.Setup(repo => repo.GetByIdAsync(1))
          .ReturnsAsync(address);
      _addressRepositoryMock.Setup(repo => repo.DeleteAsync(1))
          .Returns(Task.CompletedTask);

      // Act
      await _addressService.DeleteAddressAsync(1);

      // Assert
      _addressRepositoryMock.Verify(repo => repo.DeleteAsync(1), Times.Once);
    }

    [Fact]
    public async Task DeleteAddressAsync_ShouldSetNewDefault_WhenDeletingDefaultAddress()
    {
      // Arrange
      var defaultAddress = new AddressEntity { Id = 1, UserId = 1, IsDefault = true };
      var otherAddresses = new List<AddressEntity>
            {
                new AddressEntity { Id = 2, UserId = 1, IsDefault = false }
            };

      _addressRepositoryMock.Setup(repo => repo.GetByIdAsync(1))
          .ReturnsAsync(defaultAddress);
      _addressRepositoryMock.Setup(repo => repo.DeleteAsync(1))
          .Returns(Task.CompletedTask);
      _addressRepositoryMock.Setup(repo => repo.GetByUserIdAsync(1))
          .ReturnsAsync(otherAddresses);
      _addressRepositoryMock.Setup(repo => repo.SetDefaultAddressAsync(1, 2))
          .Returns(Task.CompletedTask);

      // Act
      await _addressService.DeleteAddressAsync(1);

      // Assert
      _addressRepositoryMock.Verify(repo => repo.SetDefaultAddressAsync(1, 2), Times.Once);
    }

    [Fact]
    public async Task SetDefaultAddressAsync_ShouldCallRepository()
    {
      // Arrange
      _addressRepositoryMock.Setup(repo => repo.SetDefaultAddressAsync(1, 2))
          .Returns(Task.CompletedTask);

      // Act
      await _addressService.SetDefaultAddressAsync(1, 2);

      // Assert
      _addressRepositoryMock.Verify(repo => repo.SetDefaultAddressAsync(1, 2), Times.Once);
    }
  }
}