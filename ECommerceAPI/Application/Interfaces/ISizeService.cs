using ECommerceAPI.Application.Models;

namespace ECommerceAPI.Application.Interfaces
{
    public interface ISizeService
    {
        Task<Size> CreateSizeAsync(Size size);
        Task<IEnumerable<Size>> GetAllSizesAsync();
        Task<Size> GetSizeByIdAsync(int id);
        Task UpdateSizeAsync(Size size);
        Task DeleteSizeAsync(int id);
    }
}