using ECommerceAPI.Infrastructure.Entities;

namespace ECommerceAPI.Infrastructure.Interfaces
{
    public interface ISizeRepository
    {
        Task<IEnumerable<SizeEntity>> GetAllAsync();
        Task<SizeEntity?> GetByIdAsync(int id);
        Task CreateAsync(SizeEntity size);
        Task UpdateAsync(SizeEntity size);
        Task DeleteAsync(int id);
    }
}