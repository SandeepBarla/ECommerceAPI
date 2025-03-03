using ECommerceAPI.Infrastructure.Context;
using ECommerceAPI.Infrastructure.Entities;
using ECommerceAPI.Infrastructure.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace ECommerceAPI.Infrastructure.Repositories
{
    public class SizeRepository : ISizeRepository
    {
        private readonly AppDbContext _context;

        public SizeRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<SizeEntity>> GetAllAsync()
        {
            return await _context.Sizes.OrderBy(s => s.SortOrder).ToListAsync();
        }

        public async Task<SizeEntity?> GetByIdAsync(int id)
        {
            return await _context.Sizes.FirstOrDefaultAsync(s => s.Id == id);
        }

        public async Task CreateAsync(SizeEntity size)
        {
            await _context.Sizes.AddAsync(size);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(SizeEntity size)
        {
            _context.Sizes.Update(size);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(SizeEntity sizeEntity)
        {
            _context.Sizes.Remove(sizeEntity);
            await _context.SaveChangesAsync();
        }
    }
}