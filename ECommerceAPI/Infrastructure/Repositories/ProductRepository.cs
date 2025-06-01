using ECommerceAPI.Infrastructure.Context;
using ECommerceAPI.Infrastructure.Entities;
using ECommerceAPI.Infrastructure.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace ECommerceAPI.Infrastructure.Repositories
{
    public class ProductRepository : IProductRepository
    {
        private readonly AppDbContext _context;

        public ProductRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<ProductEntity?> GetByIdAsync(int id)
        {
            return await _context.Products
                .Include(p => p.Media.OrderBy(m => m.OrderIndex)) // ✅ Include all media ordered
                .Include(p => p.Category) // ✅ Include category for name
                .Include(p => p.Size) // ✅ Include size for name
                .FirstOrDefaultAsync(p => p.Id == id);
        }

        public async Task<IEnumerable<ProductEntity>> GetAllAsync()
        {
            var products = await _context.Products
                .Include(p => p.Category) // ✅ Include category for name
                .Include(p => p.Size) // ✅ Include size for name
                .Include(p => p.Media) // ✅ Include all media first
                .ToListAsync();

            // ✅ Filter to only primary image (OrderIndex = 1) after loading
            foreach (var product in products)
            {
                product.Media = product.Media.Where(m => m.OrderIndex == 1).ToList();
            }

            return products;
        }

        public async Task CreateAsync(ProductEntity product)
        {
            // ✅ Set audit fields
            product.CreatedAt = DateTime.UtcNow;
            product.UpdatedAt = DateTime.UtcNow;

            _context.Products.Add(product);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(ProductEntity product)
        {
            // ✅ Update audit field
            product.UpdatedAt = DateTime.UtcNow;

            _context.Products.Update(product);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var product = await _context.Products.FindAsync(id);
            if (product != null)
            {
                _context.Products.Remove(product);
                await _context.SaveChangesAsync();
            }
        }
    }
}