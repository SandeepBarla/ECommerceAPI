using ECommerceAPI.Infrastructure.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ECommerceAPI.Infrastructure.Interfaces
{
    public interface IOrderRepository
    {
        Task<OrderEntity> CreateOrderAsync(OrderEntity order);
        Task<OrderEntity?> GetOrderByIdAsync(int orderId);
        Task<List<OrderEntity>> GetOrdersByUserIdAsync(int userId);
        Task<List<OrderEntity>> GetAllOrdersAsync();
        Task UpdateOrderAsync(OrderEntity order);
    }
}