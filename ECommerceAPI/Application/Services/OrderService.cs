using AutoMapper;
using ECommerceAPI.Application.Interfaces;
using ECommerceAPI.Application.Models;
using ECommerceAPI.Infrastructure.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;
using ECommerceAPI.Infrastructure.Interfaces;

namespace ECommerceAPI.Application.Services
{
    public class OrderService : IOrderService
    {
        private readonly IOrderRepository _orderRepository;
        private readonly IMapper _mapper;

        public OrderService(IOrderRepository orderRepository, IMapper mapper)
        {
            _orderRepository = orderRepository;
            _mapper = mapper;
        }

        public async Task<Order> CreateOrderAsync(Order order)
        {
            var orderEntity = _mapper.Map<OrderEntity>(order);
            var createdOrder = await _orderRepository.CreateOrderAsync(orderEntity);
            return _mapper.Map<Order>(createdOrder);
        }

        public async Task<Order> GetOrderByIdAsync(int id)
        {
            var orderEntity = await _orderRepository.GetOrderByIdAsync(id);
            if (orderEntity == null) throw new KeyNotFoundException("Order not found");
            return _mapper.Map<Order>(orderEntity);
        }

        public async Task<List<Order>> GetOrdersByUserIdAsync(int userId)
        {
            var orders = await _orderRepository.GetOrdersByUserIdAsync(userId);
            return _mapper.Map<List<Order>>(orders);
        }

        public async Task<List<Order>> GetAllOrdersAsync()
        {
            var orders = await _orderRepository.GetAllOrdersAsync();
            return _mapper.Map<List<Order>>(orders);
        }

        public async Task UpdateOrderStatusAsync(int orderId, string status)
        {
            var orderEntity = await _orderRepository.GetOrderByIdAsync(orderId);
            if (orderEntity == null) throw new KeyNotFoundException("Order not found");

            orderEntity.OrderStatus = status;
            await _orderRepository.UpdateOrderAsync(orderEntity);
        }

        public async Task UpdatePaymentStatusAsync(int orderId, string status, string? remarks = null)
        {
            var orderEntity = await _orderRepository.GetOrderByIdAsync(orderId);
            if (orderEntity == null) throw new KeyNotFoundException("Order not found");

            orderEntity.PaymentStatus = status;
            orderEntity.PaymentRemarks = remarks;
            await _orderRepository.UpdateOrderAsync(orderEntity);
        }

        // Admin-specific methods that return full entity data with navigation properties
        public async Task<List<OrderEntity>> GetAllOrderEntitiesAsync()
        {
            return await _orderRepository.GetAllOrdersAsync();
        }

        public async Task<OrderEntity?> GetOrderEntityByIdAsync(int id)
        {
            return await _orderRepository.GetOrderByIdAsync(id);
        }
    }
}