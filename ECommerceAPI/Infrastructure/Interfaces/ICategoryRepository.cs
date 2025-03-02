using ECommerceAPI.Infrastructure.Entities;

namespace ECommerceAPI.Infrastructure.Interfaces
{
    public interface ICategoryRepository
    {
        Task<IEnumerable<CategoryEntity>> GetAllAsync();
        Task<CategoryEntity?> GetByIdAsync(int id);
        Task CreateAsync(CategoryEntity category);
        Task UpdateAsync(CategoryEntity category);
        Task DeleteAsync(int id);
    }
}