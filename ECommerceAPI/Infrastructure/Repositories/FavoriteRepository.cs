using ECommerceAPI.Infrastructure.Context;
using ECommerceAPI.Infrastructure.Entities;
using ECommerceAPI.Infrastructure.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace ECommerceAPI.Infrastructure.Repositories
{
    public class FavoriteRepository : IFavoriteRepository
    {
        private readonly AppDbContext _context;

        public FavoriteRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task AddFavoriteAsync(FavoriteEntity favoriteEntity)
        {
            await _context.Favorites.AddAsync(favoriteEntity);
            await _context.SaveChangesAsync();
        }

        public async Task RemoveFavoriteAsync(FavoriteEntity favoriteEntity)
        {
            _context.Favorites.Remove(favoriteEntity);
            await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<FavoriteEntity>> GetUserFavoritesAsync(int userId)
        {
            return await _context.Favorites
                .Where(f => f.UserId == userId)
                .Include(f => f.Product)
                .ThenInclude(p => p.Media)
                .ToListAsync();
        }
        
        public async Task<bool> ExistsAsync(int userId, int productId)
        {
            return await _context.Favorites
                .AnyAsync(f => f.UserId == userId && f.ProductId == productId);
        }
    }
}