using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using ECommerceAPI.Application.Services;
using ECommerceAPI.WebApi.DTOs.RequestModels;
using ECommerceAPI.WebApi.DTOs.ResponseModels;

namespace ECommerceAPI.WebApi.Controllers
{
    [Authorize]  // ✅ Requires authentication
    [Route("api/cart")]
    [ApiController]
    public class CartController : ControllerBase
    {
        private readonly CartService _cartService;

        public CartController(CartService cartService)
        {
            _cartService = cartService;
        }

        private int GetUserId()
        {
            return int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
        }

        [HttpGet]
        public async Task<ActionResult<CartResponse>> GetCart()
        {
            var userId = GetUserId();
            var cart = await _cartService.GetCart(userId);
            return Ok(cart);
        }

        [HttpPost]
        public async Task<IActionResult> UpsertCartItem([FromBody] CartUpsertRequest request)
        {
            int userId = GetUserId();
            await _cartService.UpsertCartItem(userId, request);

            return Ok(new 
            { 
                message = request.Quantity == 0 ? "Item removed from cart" : "Cart updated"
            });
        }

        [HttpDelete]
        public async Task<IActionResult> ClearCart()
        {
            int userId = GetUserId();
            await _cartService.ClearCart(userId);
            return Ok(new { message = "Cart cleared" });
        }
        
        [HttpPatch]
        public async Task<IActionResult> BulkUpdateCart([FromBody] CartBulkUpdateRequest request)
        {
            int userId = GetUserId();
            await _cartService.BulkUpdateCart(userId, request);
            return Ok(new { message = "Cart updated successfully" });
        }


    }
}