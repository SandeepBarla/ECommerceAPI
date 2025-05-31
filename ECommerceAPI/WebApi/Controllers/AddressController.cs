using AutoMapper;
using ECommerceAPI.Application.Interfaces;
using ECommerceAPI.Application.Models;
using ECommerceAPI.WebApi.DTOs.RequestModels;
using ECommerceAPI.WebApi.DTOs.ResponseModels;
using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ECommerceAPI.WebApi.Controllers
{
  [Authorize]
  [Route("api/users/{userId}/addresses")]
  [ApiController]
  public class AddressController : ControllerBase
  {
    private readonly IAddressService _addressService;
    private readonly IMapper _mapper;
    private readonly IValidator<AddressUpsertRequest> _addressUpsertRequestValidator;

    public AddressController(
        IAddressService addressService,
        IMapper mapper,
        IValidator<AddressUpsertRequest> addressUpsertRequestValidator)
    {
      _addressService = addressService;
      _mapper = mapper;
      _addressUpsertRequestValidator = addressUpsertRequestValidator;
    }

    // Get all addresses for a user
    [HttpGet]
    public async Task<IActionResult> GetUserAddresses(int userId)
    {
      var addresses = await _addressService.GetAddressesByUserIdAsync(userId);
      var response = _mapper.Map<IEnumerable<AddressResponse>>(addresses);
      return Ok(response);
    }

    // Get specific address by ID
    [HttpGet("{addressId}")]
    public async Task<IActionResult> GetAddressById(int userId, int addressId)
    {
      var address = await _addressService.GetAddressByIdAsync(addressId);
      if (address == null || address.UserId != userId)
        return NotFound("Address not found.");

      var response = _mapper.Map<AddressResponse>(address);
      return Ok(response);
    }

    // Create new address
    [HttpPost]
    public async Task<IActionResult> CreateAddress(int userId, [FromBody] AddressUpsertRequest request)
    {
      await _addressUpsertRequestValidator.ValidateAndThrowAsync(request);

      var address = _mapper.Map<Address>(request);
      address.UserId = userId;

      var createdAddress = await _addressService.CreateAddressAsync(address);
      var response = _mapper.Map<AddressResponse>(createdAddress);

      return CreatedAtAction(
          nameof(GetAddressById),
          new { userId, addressId = createdAddress.Id },
          response);
    }

    // Update existing address
    [HttpPut("{addressId}")]
    public async Task<IActionResult> UpdateAddress(int userId, int addressId, [FromBody] AddressUpsertRequest request)
    {
      await _addressUpsertRequestValidator.ValidateAndThrowAsync(request);

      var address = _mapper.Map<(int, int, AddressUpsertRequest), Address>((addressId, userId, request));
      await _addressService.UpdateAddressAsync(address);

      return NoContent();
    }

    // Delete address
    [HttpDelete("{addressId}")]
    public async Task<IActionResult> DeleteAddress(int userId, int addressId)
    {
      var address = await _addressService.GetAddressByIdAsync(addressId);
      if (address == null || address.UserId != userId)
        return NotFound("Address not found.");

      await _addressService.DeleteAddressAsync(addressId);
      return NoContent();
    }

    // Set default address
    [HttpPatch("{addressId}/set-default")]
    public async Task<IActionResult> SetDefaultAddress(int userId, int addressId)
    {
      var address = await _addressService.GetAddressByIdAsync(addressId);
      if (address == null || address.UserId != userId)
        return NotFound("Address not found.");

      await _addressService.SetDefaultAddressAsync(userId, addressId);
      return NoContent();
    }

    // Get default address
    [HttpGet("default")]
    public async Task<IActionResult> GetDefaultAddress(int userId)
    {
      var address = await _addressService.GetDefaultAddressByUserIdAsync(userId);
      if (address == null)
        return NotFound("No default address found.");

      var response = _mapper.Map<AddressResponse>(address);
      return Ok(response);
    }
  }
}