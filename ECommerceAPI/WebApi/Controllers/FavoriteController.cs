using AutoMapper;
using ECommerceAPI.Application.Interfaces;
using ECommerceAPI.WebApi.DTOs.RequestModels;
using ECommerceAPI.WebApi.DTOs.ResponseModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ECommerceAPI.WebApi.Controllers
{
    [Authorize]
    [Route("api/users/{userId}/favorites")]
    [ApiController]
    public class FavoritesController : ControllerBase
    {
        private readonly IFavoriteService _favoriteService;
        private readonly IMapper _mapper;

        public FavoritesController(IFavoriteService favoriteService, IMapper mapper)
        {
            _favoriteService = favoriteService;
            _mapper = mapper;
        }

        // ✅ Mark as Favorite
        [HttpPost]
        public async Task<IActionResult> MarkAsFavorite(int userId, [FromBody] FavoriteUpsertRequest upsertRequest)
        {
            await _favoriteService.MarkAsFavoriteAsync(userId, upsertRequest.ProductId);
            return StatusCode(201); // Created
        }

        // ✅ Unmark as Favorite
        [HttpDelete("{productId}")]
        public async Task<IActionResult> UnmarkAsFavorite(int userId, int productId)
        {
            await _favoriteService.UnmarkAsFavoriteAsync(userId, productId);
            return NoContent(); // 204 No Content
        }

        // ✅ Get User's Favorites
        [HttpGet]
        public async Task<IActionResult> GetUserFavorites(int userId)
        {
            var favorites = await _favoriteService.GetUserFavoritesAsync(userId);
            var response = _mapper.Map<List<FavoriteResponse>>(favorites);
            return Ok(response);
        }
    }
}