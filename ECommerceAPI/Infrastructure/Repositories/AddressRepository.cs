using ECommerceAPI.Infrastructure.Context;
using ECommerceAPI.Infrastructure.Entities;
using ECommerceAPI.Infrastructure.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace ECommerceAPI.Infrastructure.Repositories
{
  public class AddressRepository : IAddressRepository
  {
    private readonly AppDbContext _context;

    public AddressRepository(AppDbContext context)
    {
      _context = context;
    }

    public async Task<AddressEntity> CreateAsync(AddressEntity addressEntity)
    {
      _context.Addresses.Add(addressEntity);
      await _context.SaveChangesAsync();
      return addressEntity;
    }

    public async Task<AddressEntity?> GetByIdAsync(int addressId)
    {
      return await _context.Addresses.FindAsync(addressId);
    }

    public async Task<IEnumerable<AddressEntity>> GetByUserIdAsync(int userId)
    {
      return await _context.Addresses
          .Where(a => a.UserId == userId)
          .OrderByDescending(a => a.IsDefault)
          .ThenBy(a => a.Id)
          .ToListAsync();
    }

    public async Task UpdateAsync(AddressEntity addressEntity)
    {
      _context.Addresses.Update(addressEntity);
      await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(int addressId)
    {
      var address = await _context.Addresses.FindAsync(addressId);
      if (address != null)
      {
        _context.Addresses.Remove(address);
        await _context.SaveChangesAsync();
      }
    }

    public async Task<AddressEntity?> GetDefaultAddressByUserIdAsync(int userId)
    {
      return await _context.Addresses
          .FirstOrDefaultAsync(a => a.UserId == userId && a.IsDefault);
    }

    public async Task SetDefaultAddressAsync(int userId, int addressId)
    {
      // First, unset all default addresses for the user
      var userAddresses = await _context.Addresses
          .Where(a => a.UserId == userId)
          .ToListAsync();

      foreach (var address in userAddresses)
      {
        address.IsDefault = address.Id == addressId;
      }

      await _context.SaveChangesAsync();
    }
  }
}