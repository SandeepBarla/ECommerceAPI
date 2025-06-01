using ECommerceAPI.Application.Models;
using ECommerceAPI.Infrastructure.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ECommerceAPI.Application.Interfaces
{
    public interface IOrderService
    {
        Task<Order> CreateOrderAsync(Order order);
        Task<Order> GetOrderByIdAsync(int id);
        Task<List<Order>> GetOrdersByUserIdAsync(int userId);
        Task<List<Order>> GetAllOrdersAsync();
        Task UpdateOrderStatusAsync(int orderId, string status);
        Task UpdatePaymentStatusAsync(int orderId, string status, string? remarks = null);

        // Admin-specific methods that return full entity data with navigation properties
        Task<List<OrderEntity>> GetAllOrderEntitiesAsync();
        Task<OrderEntity?> GetOrderEntityByIdAsync(int id);
    }
}