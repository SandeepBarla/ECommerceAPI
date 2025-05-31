using Moq;
using ECommerceAPI.Application.Interfaces;
using ECommerceAPI.Application.Models;
using ECommerceAPI.WebApi.Controllers;
using ECommerceAPI.WebApi.DTOs.RequestModels;
using ECommerceAPI.WebApi.DTOs.ResponseModels;
using ECommerceAPI.Tests.Common;
using FluentAssertions;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;

namespace ECommerceAPI.Tests.WebApi.Controllers
{
  public class AddressControllerTests : TestBase
  {
    private readonly AddressController _controller;
    private readonly Mock<IAddressService> _addressServiceMock;
    private readonly Mock<IValidator<AddressUpsertRequest>> _validatorMock;

    public AddressControllerTests()
    {
      _addressServiceMock = new Mock<IAddressService>();
      _validatorMock = new Mock<IValidator<AddressUpsertRequest>>();
      _controller = new AddressController(_addressServiceMock.Object, Mapper, _validatorMock.Object);
    }

    [Fact]
    public async Task GetUserAddresses_ShouldReturnOk_WithAddresses()
    {
      // Arrange
      var addresses = new List<Address>
            {
                new Address { Id = 1, UserId = 1, Name = "Home", IsDefault = true },
                new Address { Id = 2, UserId = 1, Name = "Office", IsDefault = false }
            };

      _addressServiceMock.Setup(service => service.GetAddressesByUserIdAsync(1))
          .ReturnsAsync(addresses);

      // Act
      var result = await _controller.GetUserAddresses(1);

      // Assert
      var okResult = result.Should().BeOfType<OkObjectResult>().Subject;
      var response = okResult.Value.Should().BeAssignableTo<IEnumerable<AddressResponse>>().Subject;
      response.Should().HaveCount(2);
      response.First().Name.Should().Be("Home");
    }

    [Fact]
    public async Task GetAddressById_ShouldReturnOk_WhenAddressExists()
    {
      // Arrange
      var address = new Address
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
      };

      _addressServiceMock.Setup(service => service.GetAddressByIdAsync(1))
          .ReturnsAsync(address);

      // Act
      var result = await _controller.GetAddressById(1, 1);

      // Assert
      var okResult = result.Should().BeOfType<OkObjectResult>().Subject;
      var response = okResult.Value.Should().BeOfType<AddressResponse>().Subject;
      response.Name.Should().Be("Home");
      response.UserId.Should().Be(1);
    }

    [Fact]
    public async Task GetAddressById_ShouldReturnNotFound_WhenAddressDoesNotExist()
    {
      // Arrange
      _addressServiceMock.Setup(service => service.GetAddressByIdAsync(999))
          .ReturnsAsync((Address)null);

      // Act
      var result = await _controller.GetAddressById(1, 999);

      // Assert
      var notFoundResult = result.Should().BeOfType<NotFoundObjectResult>().Subject;
      notFoundResult.Value.Should().Be("Address not found.");
    }

    [Fact]
    public async Task GetAddressById_ShouldReturnNotFound_WhenAddressBelongsToDifferentUser()
    {
      // Arrange
      var address = new Address { Id = 1, UserId = 2, Name = "Home" }; // Different user

      _addressServiceMock.Setup(service => service.GetAddressByIdAsync(1))
          .ReturnsAsync(address);

      // Act
      var result = await _controller.GetAddressById(1, 1); // User 1 trying to access User 2's address

      // Assert
      var notFoundResult = result.Should().BeOfType<NotFoundObjectResult>().Subject;
      notFoundResult.Value.Should().Be("Address not found.");
    }

    [Fact]
    public async Task CreateAddress_ShouldReturnCreated_WhenValidRequest()
    {
      // Arrange
      var request = new AddressUpsertRequest
      {
        Name = "Home",
        Street = "123 Main St",
        City = "New York",
        State = "NY",
        Pincode = "123456",
        Phone = "1234567890",
        IsDefault = true
      };

      var createdAddress = new Address
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
      };

      _validatorMock.Setup(v => v.ValidateAsync(request, default))
          .ReturnsAsync(new FluentValidation.Results.ValidationResult());
      _addressServiceMock.Setup(service => service.CreateAddressAsync(It.IsAny<Address>()))
          .ReturnsAsync(createdAddress);

      // Act
      var result = await _controller.CreateAddress(1, request);

      // Assert
      var createdResult = result.Should().BeOfType<CreatedAtActionResult>().Subject;
      var response = createdResult.Value.Should().BeOfType<AddressResponse>().Subject;
      response.Name.Should().Be("Home");
      response.Id.Should().Be(1);

      createdResult.ActionName.Should().Be(nameof(AddressController.GetAddressById));
      createdResult.RouteValues["userId"].Should().Be(1);
      createdResult.RouteValues["addressId"].Should().Be(1);
    }

    [Fact]
    public async Task UpdateAddress_ShouldReturnNoContent_WhenValidRequest()
    {
      // Arrange
      var request = new AddressUpsertRequest
      {
        Name = "Updated Home",
        Street = "456 Updated St",
        City = "Boston",
        State = "MA",
        Pincode = "654321",
        Phone = "0987654321",
        IsDefault = false
      };

      _validatorMock.Setup(v => v.ValidateAsync(request, default))
          .ReturnsAsync(new FluentValidation.Results.ValidationResult());
      _addressServiceMock.Setup(service => service.UpdateAddressAsync(It.IsAny<Address>()))
          .Returns(Task.CompletedTask);

      // Act
      var result = await _controller.UpdateAddress(1, 1, request);

      // Assert
      result.Should().BeOfType<NoContentResult>();
      _addressServiceMock.Verify(service => service.UpdateAddressAsync(It.Is<Address>(a =>
          a.Id == 1 && a.UserId == 1 && a.Name == "Updated Home")), Times.Once);
    }

    [Fact]
    public async Task DeleteAddress_ShouldReturnNotFound_WhenAddressDoesNotExist()
    {
      // Arrange
      _addressServiceMock.Setup(service => service.GetAddressByIdAsync(999))
          .ReturnsAsync((Address)null);

      // Act
      var result = await _controller.DeleteAddress(1, 999);

      // Assert
      var notFoundResult = result.Should().BeOfType<NotFoundObjectResult>().Subject;
      notFoundResult.Value.Should().Be("Address not found.");
    }

    [Fact]
    public async Task DeleteAddress_ShouldReturnNotFound_WhenAddressBelongsToDifferentUser()
    {
      // Arrange
      var address = new Address { Id = 1, UserId = 2, Name = "Home" }; // Different user

      _addressServiceMock.Setup(service => service.GetAddressByIdAsync(1))
          .ReturnsAsync(address);

      // Act
      var result = await _controller.DeleteAddress(1, 1); // User 1 trying to delete User 2's address

      // Assert
      var notFoundResult = result.Should().BeOfType<NotFoundObjectResult>().Subject;
      notFoundResult.Value.Should().Be("Address not found.");
    }

    [Fact]
    public async Task DeleteAddress_ShouldReturnNoContent_WhenAddressExists()
    {
      // Arrange
      var address = new Address { Id = 1, UserId = 1, Name = "Home" };

      _addressServiceMock.Setup(service => service.GetAddressByIdAsync(1))
          .ReturnsAsync(address);
      _addressServiceMock.Setup(service => service.DeleteAddressAsync(1))
          .Returns(Task.CompletedTask);

      // Act
      var result = await _controller.DeleteAddress(1, 1);

      // Assert
      result.Should().BeOfType<NoContentResult>();
      _addressServiceMock.Verify(service => service.DeleteAddressAsync(1), Times.Once);
    }

    [Fact]
    public async Task SetDefaultAddress_ShouldReturnNotFound_WhenAddressDoesNotExist()
    {
      // Arrange
      _addressServiceMock.Setup(service => service.GetAddressByIdAsync(999))
          .ReturnsAsync((Address)null);

      // Act
      var result = await _controller.SetDefaultAddress(1, 999);

      // Assert
      var notFoundResult = result.Should().BeOfType<NotFoundObjectResult>().Subject;
      notFoundResult.Value.Should().Be("Address not found.");
    }

    [Fact]
    public async Task SetDefaultAddress_ShouldReturnNoContent_WhenAddressExists()
    {
      // Arrange
      var address = new Address { Id = 1, UserId = 1, Name = "Home" };

      _addressServiceMock.Setup(service => service.GetAddressByIdAsync(1))
          .ReturnsAsync(address);
      _addressServiceMock.Setup(service => service.SetDefaultAddressAsync(1, 1))
          .Returns(Task.CompletedTask);

      // Act
      var result = await _controller.SetDefaultAddress(1, 1);

      // Assert
      result.Should().BeOfType<NoContentResult>();
      _addressServiceMock.Verify(service => service.SetDefaultAddressAsync(1, 1), Times.Once);
    }

    [Fact]
    public async Task GetDefaultAddress_ShouldReturnOk_WhenDefaultAddressExists()
    {
      // Arrange
      var address = new Address
      {
        Id = 1,
        UserId = 1,
        Name = "Home",
        IsDefault = true
      };

      _addressServiceMock.Setup(service => service.GetDefaultAddressByUserIdAsync(1))
          .ReturnsAsync(address);

      // Act
      var result = await _controller.GetDefaultAddress(1);

      // Assert
      var okResult = result.Should().BeOfType<OkObjectResult>().Subject;
      var response = okResult.Value.Should().BeOfType<AddressResponse>().Subject;
      response.Name.Should().Be("Home");
      response.IsDefault.Should().BeTrue();
    }

    [Fact]
    public async Task GetDefaultAddress_ShouldReturnNotFound_WhenNoDefaultAddress()
    {
      // Arrange
      _addressServiceMock.Setup(service => service.GetDefaultAddressByUserIdAsync(1))
          .ReturnsAsync((Address)null);

      // Act
      var result = await _controller.GetDefaultAddress(1);

      // Assert
      var notFoundResult = result.Should().BeOfType<NotFoundObjectResult>().Subject;
      notFoundResult.Value.Should().Be("No default address found.");
    }
  }
}