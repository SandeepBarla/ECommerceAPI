using ECommerceAPI.Application.Models;

namespace ECommerceAPI.Application.Interfaces
{
    public interface ICartService
    {
        Task<Cart> GetCartByUserIdAsync(int userId);
        Task AddOrUpdateCartItemAsync(int userId, CartItem cartItem);
        Task ClearCartAsync(int userId);
    }
}