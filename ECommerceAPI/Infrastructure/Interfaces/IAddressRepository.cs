using ECommerceAPI.Infrastructure.Entities;

namespace ECommerceAPI.Infrastructure.Repositories.Interfaces
{
  public interface IAddressRepository
  {
    Task<AddressEntity> CreateAsync(AddressEntity addressEntity);
    Task<AddressEntity?> GetByIdAsync(int addressId);
    Task<IEnumerable<AddressEntity>> GetByUserIdAsync(int userId);
    Task UpdateAsync(AddressEntity addressEntity);
    Task DeleteAsync(int addressId);
    Task<AddressEntity?> GetDefaultAddressByUserIdAsync(int userId);
    Task SetDefaultAddressAsync(int userId, int addressId);
  }
}