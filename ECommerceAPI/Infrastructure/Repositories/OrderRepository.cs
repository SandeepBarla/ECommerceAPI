using ECommerceAPI.Infrastructure.Context;
using ECommerceAPI.Infrastructure.Entities;
using Microsoft.EntityFrameworkCore;
using ECommerceAPI.Infrastructure.Interfaces;

namespace ECommerceAPI.Infrastructure.Repositories
{
    public class OrderRepository : IOrderRepository
    {
        private readonly AppDbContext _context;

        public OrderRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<OrderEntity> CreateOrderAsync(OrderEntity order)
        {
            _context.Orders.Add(order);
            await _context.SaveChangesAsync();
            return order;
        }

        public async Task<OrderEntity?> GetOrderByIdAsync(int orderId)
        {
            return await _context.Orders
                .Include(o => o.OrderProducts)
                    .ThenInclude(op => op.Product)
                .Include(o => o.Address)
                .Include(o => o.User)
                .FirstOrDefaultAsync(o => o.Id == orderId);
        }

        public async Task<List<OrderEntity>> GetOrdersByUserIdAsync(int userId)
        {
            return await _context.Orders
                .Where(o => o.UserId == userId)
                .Include(o => o.OrderProducts)
                    .ThenInclude(op => op.Product)
                .Include(o => o.Address)
                .Include(o => o.User)
                .ToListAsync();
        }

        public async Task<List<OrderEntity>> GetAllOrdersAsync()
        {
            return await _context.Orders
                .Include(o => o.OrderProducts)
                    .ThenInclude(op => op.Product)
                .Include(o => o.Address)
                .Include(o => o.User)
                .ToListAsync();
        }

        public async Task UpdateOrderAsync(OrderEntity order)
        {
            _context.Orders.Update(order);
            await _context.SaveChangesAsync();
        }
    }
}