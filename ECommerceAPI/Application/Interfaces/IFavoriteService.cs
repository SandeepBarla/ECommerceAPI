using ECommerceAPI.Application.Models;

namespace ECommerceAPI.Application.Interfaces
{
    public interface IFavoriteService
    {
        Task MarkAsFavoriteAsync(Favorite favorite);
        Task UnmarkAsFavoriteAsync(Favorite favorite);
        Task<IEnumerable<Favorite>> GetUserFavoritesAsync(int userId);
    }
}