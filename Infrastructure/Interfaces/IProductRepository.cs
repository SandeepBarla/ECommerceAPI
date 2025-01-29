using ECommerceAPI.Infrastructure.Entities;

namespace ECommerceAPI.Infrastructure.Interfaces
{
    public interface IProductRepository
    {
        Task<ProductEntity?> GetByIdAsync(int id);
        Task<IEnumerable<ProductEntity>> GetAllAsync();
        Task CreateAsync(ProductEntity product);
        Task UpdateAsync(ProductEntity product);
        Task DeleteAsync(int id);
    }
}