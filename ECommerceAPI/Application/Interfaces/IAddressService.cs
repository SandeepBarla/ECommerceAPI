using ECommerceAPI.Application.Models;

namespace ECommerceAPI.Application.Interfaces
{
  public interface IAddressService
  {
    Task<Address> CreateAddressAsync(Address address);
    Task<Address?> GetAddressByIdAsync(int addressId);
    Task<IEnumerable<Address>> GetAddressesByUserIdAsync(int userId);
    Task UpdateAddressAsync(Address address);
    Task DeleteAddressAsync(int addressId);
    Task<Address?> GetDefaultAddressByUserIdAsync(int userId);
    Task SetDefaultAddressAsync(int userId, int addressId);
  }
}