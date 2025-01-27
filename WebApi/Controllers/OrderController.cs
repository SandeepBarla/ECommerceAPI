using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ECommerceAPI.Infrastructure.Context;
using ECommerceAPI.Application.Models;
using ECommerceAPI.WebApi.DTOs.RequestModels;
using ECommerceAPI.WebApi.DTOs.ResponseModels;
using System.Security.Claims;
using ECommerceAPI.Application.Services;

namespace ECommerceAPI.WebApi.Controllers
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

        // ✅ Create Order (Fixed DTO Mapping)
        [Authorize]
        [HttpPost]
        public async Task<IActionResult> CreateOrder([FromBody] OrderCreateRequest orderDto)
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);

            var order = new Order
            {
                UserId = userId,
                TotalAmount = orderDto.TotalAmount,
                ShippingAddress = orderDto.ShippingAddress,
                PaymentStatus = "Pending",
                OrderStatus = "Processing",
                OrderDate = DateTime.UtcNow,
                Products = orderDto.Products.Select(p => new OrderProduct
                {
                    ProductId = p.ProductId,
                    Quantity = p.Quantity
                }).ToList()
            };

            _context.Orders.Add(order);
            await _context.SaveChangesAsync();

            var response = new OrderResponse
            {
                Id = order.Id,
                Products = order.Products.Select(p => new OrderProductResponse
                {
                    ProductId = p.ProductId,
                    Quantity = p.Quantity
                }).ToList(),
                TotalAmount = order.TotalAmount,
                PaymentStatus = order.PaymentStatus,
                OrderStatus = order.OrderStatus,
                OrderDate = order.OrderDate,
                ShippingAddress = order.ShippingAddress,
                TrackingNumber = order.TrackingNumber
            };

            return CreatedAtAction(nameof(GetOrderById), new { id = order.Id }, response);
        }

        // ✅ Get Orders for Logged-in User (Fixed Mapping)
        [Authorize]
        [HttpGet]
        public async Task<IActionResult> GetUserOrders()
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
            var orders = await _context.Orders
                .Include(o => o.Products)
                .Where(o => o.UserId == userId)
                .ToListAsync();

            var response = orders.Select(o => new OrderResponse
            {
                Id = o.Id,
                Products = o.Products.Select(p => new OrderProductResponse
                {
                    ProductId = p.ProductId,
                    Quantity = p.Quantity
                }).ToList(),
                TotalAmount = o.TotalAmount,
                PaymentStatus = o.PaymentStatus,
                OrderStatus = o.OrderStatus,
                OrderDate = o.OrderDate,
                ShippingAddress = o.ShippingAddress,
                TrackingNumber = o.TrackingNumber
            });

            return Ok(response);
        }

        // ✅ Get Order by ID (Fixed Mapping)
        [Authorize]
        [HttpGet("{id}")]
        public async Task<IActionResult> GetOrderById(int id)
        {
            var order = await _context.Orders
                .Include(o => o.Products)
                .FirstOrDefaultAsync(o => o.Id == id);

            if (order == null)
                return NotFound("Order not found.");

            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
            if (order.UserId != userId)
                return Unauthorized("Access denied.");

            var response = new OrderResponse
            {
                Id = order.Id,
                Products = order.Products.Select(p => new OrderProductResponse
                {
                    ProductId = p.ProductId,
                    Quantity = p.Quantity
                }).ToList(),
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

        // ✅ Get Orders by User ID (Admin Only) (Fixed Mapping)
        [Authorize(Roles = "Admin")]
        [HttpGet("users/{userId}")]
        public async Task<IActionResult> GetOrdersByUserId(int userId)
        {
            var orders = await _orderService.GetOrdersByUserIdAsync(userId);
            if (orders == null || !orders.Any())
                return NotFound("No orders found for this user.");

            var response = orders.Select(o => new OrderResponse
            {
                Id = o.Id,
                Products = o.Products.Select(p => new OrderProductResponse
                {
                    ProductId = p.ProductId,
                    Quantity = p.Quantity
                }).ToList(),
                TotalAmount = o.TotalAmount,
                PaymentStatus = o.PaymentStatus,
                OrderStatus = o.OrderStatus,
                OrderDate = o.OrderDate,
                ShippingAddress = o.ShippingAddress,
                TrackingNumber = o.TrackingNumber
            });

            return Ok(response);
        }

        // ✅ Get All Orders (Admin Only) (Fixed Mapping)
        [Authorize(Roles = "Admin")]
        [HttpGet("all")]
        public async Task<IActionResult> GetAllOrders()
        {
            var orders = await _orderService.GetAllOrdersAsync();

            var response = orders.Select(o => new OrderResponse
            {
                Id = o.Id,
                Products = o.Products.Select(p => new OrderProductResponse
                {
                    ProductId = p.ProductId,
                    Quantity = p.Quantity
                }).ToList(),
                TotalAmount = o.TotalAmount,
                PaymentStatus = o.PaymentStatus,
                OrderStatus = o.OrderStatus,
                OrderDate = o.OrderDate,
                ShippingAddress = o.ShippingAddress,
                TrackingNumber = o.TrackingNumber
            });

            return Ok(response);
        }
    }
}
