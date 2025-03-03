using ECommerceAPI.Infrastructure.Entities;

namespace ECommerceAPI.Infrastructure.Interfaces
{
    public interface IFavoriteRepository
    {
        Task AddFavoriteAsync(FavoriteEntity favoriteEntity);
        Task RemoveFavoriteAsync(FavoriteEntity favoriteEntity);
        Task<IEnumerable<FavoriteEntity>> GetUserFavoritesAsync(int userId);
        Task<bool> ExistsAsync(int userId, int productId);
    }
}