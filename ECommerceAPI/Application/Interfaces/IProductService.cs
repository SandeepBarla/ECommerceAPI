using ECommerceAPI.Application.Models;
using ECommerceAPI.WebApi.DTOs.ResponseModels;

namespace ECommerceAPI.Application.Interfaces
{
    public interface IProductService
    {
        Task<Product> GetProductByIdAsync(int id);
        Task<IEnumerable<Product>> GetAllProductsAsync();

        // ✅ Enhanced methods for proper DTO responses
        Task<ProductResponse> GetProductResponseByIdAsync(int id);
        Task<IEnumerable<ProductListResponse>> GetProductListResponsesAsync();

        Task<Product> CreateProductAsync(Product product);
        Task UpdateProductAsync(Product product);
        Task DeleteProductAsync(int id);
    }
}