using ECommerceAPI.Infrastructure.Entities;

namespace ECommerceAPI.Infrastructure.Interfaces
{
    public interface ICartRepository
    {
        Task<CartEntity?> GetCartByUserIdAsync(int userId);
        Task SaveCartAsync(CartEntity cartEntity);
        Task RemoveCartAsync(CartEntity cartEntity);
    }
}