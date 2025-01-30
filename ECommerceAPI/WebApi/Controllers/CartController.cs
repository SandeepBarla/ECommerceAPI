using AutoMapper;
using ECommerceAPI.Application.Interfaces;
using ECommerceAPI.Application.Models;
using ECommerceAPI.WebApi.DTOs.RequestModels;
using ECommerceAPI.WebApi.DTOs.ResponseModels;
using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ECommerceAPI.WebApi.Controllers
{
    [Route("api/users/{userId}/cart")]
    [ApiController]
    [Authorize] // Ensure authentication for cart operations
    public class CartController : ControllerBase
    {
        private readonly ICartService _cartService;
        private readonly IMapper _mapper;
        private readonly IValidator<CartAddOrUpdateItemRequest> _cartAddOrUpdateItemValidator;

        public CartController(ICartService cartService, IMapper mapper, IValidator<CartAddOrUpdateItemRequest> cartAddOrUpdateItemValidator)
        {
            _cartService = cartService;
            _mapper = mapper;
            _cartAddOrUpdateItemValidator = cartAddOrUpdateItemValidator;
        }

        // Get cart details for a user
        [HttpGet]
        public async Task<IActionResult> GetCart(int userId)
        {
            var cart = await _cartService.GetCartByUserIdAsync(userId);
            var response = _mapper.Map<CartResponse>(cart);
            return Ok(response);
        }

        // Add or update an item in the cart
        [HttpPost]
        public async Task<IActionResult> AddOrUpdateItem(int userId, [FromBody] CartAddOrUpdateItemRequest request)
        {
            await _cartAddOrUpdateItemValidator.ValidateAndThrowAsync(request);
            var cartItem = _mapper.Map<CartItem>(request);
            await _cartService.AddOrUpdateCartItemAsync(userId, cartItem);
            return Ok(new { Message = "Cart updated successfully" });
        }

        // Delete entire cart
        [HttpDelete]
        public async Task<IActionResult> ClearCart(int userId)
        {
            await _cartService.ClearCartAsync(userId);
            return NoContent();
        }
    }
}