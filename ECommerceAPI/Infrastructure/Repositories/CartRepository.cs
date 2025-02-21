using ECommerceAPI.Infrastructure.Context;
using ECommerceAPI.Infrastructure.Entities;
using ECommerceAPI.Infrastructure.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace ECommerceAPI.Infrastructure.Repositories
{
    public class CartRepository : ICartRepository
    {
        private readonly AppDbContext _context;

        public CartRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<CartEntity?> GetCartByUserIdAsync(int userId)
        {
            var cartEntity = await _context.Carts
                .Include(c => c.CartItems)
                .ThenInclude(ci => ci.Product)
                .ThenInclude(p => p.Media)
                .Include(c => c.User)
                .FirstOrDefaultAsync(c => c.UserId == userId);
            return cartEntity;
        }

        public async Task SaveCartAsync(CartEntity cartEntity)
        {
            _context.Carts.Update(cartEntity);
            await _context.SaveChangesAsync();
        }

        public async Task RemoveCartAsync(CartEntity cartEntity)
        {
            _context.Carts.Remove(cartEntity);
            await _context.SaveChangesAsync();
        }
    }
}