using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ECommerceAPI.Data;
using ECommerceAPI.Models;
using ECommerceAPI.DTOs.RequestModels;
using ECommerceAPI.DTOs.ResponseModels;
using System.Security.Claims;
using ECommerceAPI.Services;

namespace ECommerceAPI.Controllers
{
    [Route("api/orders")]
    [ApiController]
    public class OrderController : ControllerBase
    {
        private readonly IOrderService _orderService;
        private readonly AppDbContext _context;

        public OrderController(AppDbContext context, IOrderService orderService)
        {
            _context = context;
            _orderService = orderService;
        }

        // ✅ Create Order (Using DTO)
        [Authorize]
        [HttpPost]
        public async Task<IActionResult> CreateOrder([FromBody] OrderCreateRequest orderDto)
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);

            var order = new Order
            {
                UserId = userId,
                Products = orderDto.Products,
                TotalAmount = orderDto.TotalAmount,
                ShippingAddress = orderDto.ShippingAddress,
                PaymentStatus = "Pending",
                OrderStatus = "Processing",
                OrderDate = DateTime.UtcNow
            };

            _context.Orders.Add(order);
            await _context.SaveChangesAsync();

            var response = new OrderResponse
            {
                Id = order.Id,
                Products = order.Products,
                TotalAmount = order.TotalAmount,
                PaymentStatus = order.PaymentStatus,
                OrderStatus = order.OrderStatus,
                OrderDate = order.OrderDate,
                ShippingAddress = order.ShippingAddress,
                TrackingNumber = order.TrackingNumber
            };

            return CreatedAtAction(nameof(GetOrderById), new { id = order.Id }, response);
        }

        // ✅ Get Orders for Logged-in User
        [Authorize]
        [HttpGet]
        public async Task<IActionResult> GetUserOrders()
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
            var orders = await _context.Orders.Where(o => o.UserId == userId).ToListAsync();

            var response = orders.Select(o => new OrderResponse
            {
                Id = o.Id,
                Products = o.Products,
                TotalAmount = o.TotalAmount,
                PaymentStatus = o.PaymentStatus,
                OrderStatus = o.OrderStatus,
                OrderDate = o.OrderDate,
                ShippingAddress = o.ShippingAddress,
                TrackingNumber = o.TrackingNumber
            });

            return Ok(response);
        }

        // ✅ Get Order by ID
        [Authorize]
        [HttpGet("{id}")]
        public async Task<IActionResult> GetOrderById(int id)
        {
            var order = await _context.Orders.FindAsync(id);
            if (order == null)
                return NotFound("Order not found.");

            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
            if (order.UserId != userId)
                return Unauthorized("Access denied.");

            var response = new OrderResponse
            {
                Id = order.Id,
                Products = order.Products,
                TotalAmount = order.TotalAmount,
                PaymentStatus = order.PaymentStatus,
                OrderStatus = order.OrderStatus,
                OrderDate = order.OrderDate,
                ShippingAddress = order.ShippingAddress,
                TrackingNumber = order.TrackingNumber
            };

            return Ok(response);
        }

        // ✅ Update Order Status (Admin Only)
        [Authorize(Roles = "Admin")]
        [HttpPut("{id}/status")]
        public async Task<IActionResult> UpdateOrderStatus(int id, [FromBody] string status)
        {
            var order = await _context.Orders.FindAsync(id);
            if (order == null)
                return NotFound("Order not found.");

            order.OrderStatus = status;
            await _context.SaveChangesAsync();
            return Ok(new { message = "Order status updated", orderId = id, newStatus = status });
        }
        
        [HttpGet("user/{userId}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetOrdersByUserId(int userId)
        {
            var orders = await _orderService.GetOrdersByUserIdAsync(userId);
            if (orders == null || !orders.Any())
                return NotFound("No orders found for this user.");

            var response = orders.Select(o => new OrderResponse
            {
                Id = o.Id,
                Products = o.Products,
                TotalAmount = o.TotalAmount,
                PaymentStatus = o.PaymentStatus,
                OrderStatus = o.OrderStatus,
                OrderDate = o.OrderDate,
                ShippingAddress = o.ShippingAddress,
                TrackingNumber = o.TrackingNumber
            });

            return Ok(response);
        }
        
        [HttpGet("all")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetAllOrders()
        {
            var orders = await _orderService.GetAllOrdersAsync();
            return Ok(orders);
        }
    }
}
