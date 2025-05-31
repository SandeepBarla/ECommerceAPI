using AutoMapper;
using ECommerceAPI.Application.Interfaces;
using ECommerceAPI.Application.Models;
using ECommerceAPI.Infrastructure.Entities;
using ECommerceAPI.Infrastructure.Repositories.Interfaces;

namespace ECommerceAPI.Application.Services
{
  public class AddressService : IAddressService
  {
    private readonly IAddressRepository _addressRepository;
    private readonly IMapper _mapper;

    public AddressService(IAddressRepository addressRepository, IMapper mapper)
    {
      _addressRepository = addressRepository;
      _mapper = mapper;
    }

    public async Task<Address> CreateAddressAsync(Address address)
    {
      // If this is set as default, ensure no other address is default for this user
      if (address.IsDefault)
      {
        await SetDefaultAddressAsync(address.UserId, 0); // Unset all defaults first
      }

      var addressEntity = _mapper.Map<AddressEntity>(address);
      var createdEntity = await _addressRepository.CreateAsync(addressEntity);
      return _mapper.Map<Address>(createdEntity);
    }

    public async Task<Address?> GetAddressByIdAsync(int addressId)
    {
      var addressEntity = await _addressRepository.GetByIdAsync(addressId);
      return addressEntity == null ? null : _mapper.Map<Address>(addressEntity);
    }

    public async Task<IEnumerable<Address>> GetAddressesByUserIdAsync(int userId)
    {
      var addressEntities = await _addressRepository.GetByUserIdAsync(userId);
      return _mapper.Map<IEnumerable<Address>>(addressEntities);
    }

    public async Task UpdateAddressAsync(Address address)
    {
      var existingAddress = await _addressRepository.GetByIdAsync(address.Id);
      if (existingAddress == null)
        throw new KeyNotFoundException("Address not found.");

      // If this address is being set as default, unset others
      if (address.IsDefault && !existingAddress.IsDefault)
      {
        await SetDefaultAddressAsync(address.UserId, address.Id);
        return; // SetDefaultAddressAsync handles the update
      }

      _mapper.Map(address, existingAddress);
      await _addressRepository.UpdateAsync(existingAddress);
    }

    public async Task DeleteAddressAsync(int addressId)
    {
      var address = await _addressRepository.GetByIdAsync(addressId);
      if (address == null)
        throw new KeyNotFoundException("Address not found.");

      await _addressRepository.DeleteAsync(addressId);

      // If we deleted the default address, make another one default if available
      if (address.IsDefault)
      {
        var userAddresses = await _addressRepository.GetByUserIdAsync(address.UserId);
        var firstAddress = userAddresses.FirstOrDefault();
        if (firstAddress != null)
        {
          await SetDefaultAddressAsync(address.UserId, firstAddress.Id);
        }
      }
    }

    public async Task<Address?> GetDefaultAddressByUserIdAsync(int userId)
    {
      var addressEntity = await _addressRepository.GetDefaultAddressByUserIdAsync(userId);
      return addressEntity == null ? null : _mapper.Map<Address>(addressEntity);
    }

    public async Task SetDefaultAddressAsync(int userId, int addressId)
    {
      await _addressRepository.SetDefaultAddressAsync(userId, addressId);
    }
  }
}