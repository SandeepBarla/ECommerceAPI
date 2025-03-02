using ECommerceAPI.Application.Models;

namespace ECommerceAPI.Application.Interfaces
{
    public interface ISizeService
    {
        Task<Size> CreateSizeAsync(string name);
        Task<IEnumerable<Size>> GetAllSizesAsync();
        Task<Size> GetSizeByIdAsync(int id);
        Task UpdateSizeAsync(int id, string name);
        Task DeleteSizeAsync(int id);
    }
}