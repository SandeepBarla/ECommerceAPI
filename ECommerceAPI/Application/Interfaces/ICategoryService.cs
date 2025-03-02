using ECommerceAPI.Application.Models;

namespace ECommerceAPI.Application.Interfaces
{
    public interface ICategoryService
    {
        Task<Category> CreateCategoryAsync(string name);
        Task<IEnumerable<Category>> GetAllCategoriesAsync();
        Task<Category> GetCategoryByIdAsync(int id);
        Task UpdateCategoryAsync(int id, string name);
        Task DeleteCategoryAsync(int id);
    }
}