using AutoMapper;
using ECommerceAPI.Application.Interfaces;
using ECommerceAPI.Application.Models;
using ECommerceAPI.WebApi.DTOs.RequestModels;
using ECommerceAPI.WebApi.DTOs.ResponseModels;
using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ECommerceAPI.WebApi.Controllers
{
    [Route("api")]
    [ApiController]
    public class OrderController : ControllerBase
    {
        private readonly IOrderService _orderService;
        private readonly IMapper _mapper;
        private readonly IValidator<OrderCreateRequest> _orderCreateRequestValidator;
        private readonly IValidator<OrderStatusUpdateRequest> _orderStatusUpdateRequestValidator;

        public OrderController(
            IOrderService orderService,
            IMapper mapper,
            IValidator<OrderCreateRequest> orderCreateRequestValidator,
            IValidator<OrderStatusUpdateRequest> orderStatusUpdateRequestValidator)
        {
            _orderService = orderService;
            _mapper = mapper;
            _orderCreateRequestValidator = orderCreateRequestValidator;
            _orderStatusUpdateRequestValidator = orderStatusUpdateRequestValidator;
        }

        [Authorize]
        [HttpPost("users/{userId}/orders")]
        public async Task<IActionResult> CreateOrder(int userId, [FromBody] OrderCreateRequest request)
        {
            await _orderCreateRequestValidator.ValidateAndThrowAsync(request);
            var order = _mapper.Map<Order>(request);
            order.UserId = userId; 
            var createdOrder = await _orderService.CreateOrderAsync(order);
            return CreatedAtAction(nameof(GetOrderById), new { userId, id = createdOrder.Id }, _mapper.Map<OrderResponse>(createdOrder));
        }

        [Authorize]
        [HttpGet("users/{userId}/orders")]
        public async Task<IActionResult> GetOrders(int userId)
        {
            var orders = await _orderService.GetOrdersByUserIdAsync(userId);
            return Ok(_mapper.Map<List<OrderResponse>>(orders));
        }

        [Authorize]
        [HttpGet("users/{userId}/orders/{id}")]
        public async Task<IActionResult> GetOrderById(int userId, int id)
        {
            var order = await _orderService.GetOrderByIdAsync(id);
            if (order.UserId != userId) return Forbid();
            return Ok(_mapper.Map<OrderResponse>(order));
        }

        [Authorize(Roles = "Admin")]
        [HttpGet("orders")]
        public async Task<IActionResult> GetAllOrders()
        {
            var orders = await _orderService.GetAllOrdersAsync();
            return Ok(_mapper.Map<List<OrderResponse>>(orders));
        }

        [Authorize]
        [HttpPatch("users/{userId}/orders/{id}")]
        public async Task<IActionResult> UpdateOrderStatus(int userId, int id, [FromBody] OrderStatusUpdateRequest request)
        {
            await _orderStatusUpdateRequestValidator.ValidateAndThrowAsync(request);
            await _orderService.UpdateOrderStatusAsync(id, request.Status);
            return NoContent();
        }
    }
}