using AutoMapper;
using ECommerceAPI.Infrastructure.Context;
using ECommerceAPI.Application.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ECommerceAPI.Application.Services
{
    public interface IOrderService
    {
        Task<IEnumerable<Order>> GetOrdersByUserIdAsync(int userId);
        Task<IEnumerable<Order>> GetAllOrdersAsync();
    }

    public class OrderService : IOrderService
    {
        private readonly AppDbContext _context;
        private readonly IMapper _mapper;

        public OrderService(AppDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<IEnumerable<Order>> GetOrdersByUserIdAsync(int userId)
        {
            var orderEntities = await _context.Orders
                .Where(o => o.UserId == userId)
                .Include(o => o.OrderProducts) // ✅ Ensure Products are included
                .ThenInclude(op => op.Product) // ✅ Include actual Product details
                .ToListAsync();
            return _mapper.Map<IEnumerable<Order>>(orderEntities);
        }

        public async Task<IEnumerable<Order>> GetAllOrdersAsync()
        {
            var orderEntities = await _context.Orders
                .Include(o => o.OrderProducts) // ✅ Ensure Products are included
                .ThenInclude(op => op.Product) // ✅ Include actual Product details
                .ToListAsync();
            return _mapper.Map<IEnumerable<Order>>(orderEntities);
        }
    }
}