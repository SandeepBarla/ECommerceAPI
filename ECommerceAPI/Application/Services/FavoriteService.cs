using AutoMapper;
using ECommerceAPI.Application.Interfaces;
using ECommerceAPI.Application.Models;
using ECommerceAPI.Infrastructure.Entities;
using ECommerceAPI.Infrastructure.Interfaces;

namespace ECommerceAPI.Application.Services
{
    public class FavoriteService : IFavoriteService
    {
        private readonly IFavoriteRepository _favoriteRepository;
        private readonly IUserService _userService;
        private readonly IProductService _productService;
        private readonly IMapper _mapper;

        public FavoriteService(IFavoriteRepository favoriteRepository, 
            IUserService userService, IProductService productService, IMapper mapper)
        {
            _favoriteRepository = favoriteRepository;
            _userService = userService;
            _productService = productService;
            _mapper = mapper;
        }

        public async Task MarkAsFavoriteAsync(Favorite favorite)
        {
            await _userService.GetUserByIdAsync(favorite.UserId);
            await _productService.GetProductByIdAsync(favorite.ProductId);
            if (await _favoriteRepository.ExistsAsync(favorite.UserId, favorite.ProductId))
            {
                throw new InvalidOperationException("This product is already in favorites.");
            }
            var favoriteEntity = _mapper.Map<FavoriteEntity>(favorite);
            await _favoriteRepository.AddFavoriteAsync(favoriteEntity);
        }

        public async Task UnmarkAsFavoriteAsync(Favorite favorite)
        {
            await _userService.GetUserByIdAsync(favorite.UserId);
            await _productService.GetProductByIdAsync(favorite.ProductId);
            if (!await _favoriteRepository.ExistsAsync(favorite.UserId, favorite.ProductId))
            {
                throw new InvalidOperationException("This product is not in favorites.");
            }
            var favoriteEntity = _mapper.Map<FavoriteEntity>(favorite);
            await _favoriteRepository.RemoveFavoriteAsync(favoriteEntity);
        }

        public async Task<IEnumerable<Favorite>> GetUserFavoritesAsync(int userId)
        {
            await _userService.GetUserByIdAsync(userId);
            var entities = await _favoriteRepository.GetUserFavoritesAsync(userId);
            return _mapper.Map<IEnumerable<Favorite>>(entities);
        }
    }
}