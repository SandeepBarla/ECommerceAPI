using ECommerceAPI.Application.Models;

namespace ECommerceAPI.Application.Interfaces
{
    public interface IFavoriteService
    {
        Task MarkAsFavoriteAsync(int userId, int productId);
        Task UnmarkAsFavoriteAsync(int userId, int productId);
        Task<IEnumerable<Favorite>> GetUserFavoritesAsync(int userId);
    }
}