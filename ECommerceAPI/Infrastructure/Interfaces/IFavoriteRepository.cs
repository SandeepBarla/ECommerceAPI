using ECommerceAPI.Infrastructure.Entities;

namespace ECommerceAPI.Infrastructure.Interfaces
{
    public interface IFavoriteRepository
    {
        Task AddFavoriteAsync(int userId, int productId);
        Task RemoveFavoriteAsync(int userId, int productId);
        Task<IEnumerable<FavoriteEntity>> GetUserFavoritesAsync(int userId);
    }
}