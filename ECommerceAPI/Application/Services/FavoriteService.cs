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
        private readonly IMapper _mapper;

        public FavoriteService(IFavoriteRepository favoriteRepository, IMapper mapper)
        {
            _favoriteRepository = favoriteRepository;
            _mapper = mapper;
        }

        public async Task MarkAsFavoriteAsync(int userId, int productId)
        {
            await _favoriteRepository.AddFavoriteAsync(userId, productId);
        }

        public async Task UnmarkAsFavoriteAsync(int userId, int productId)
        {
            await _favoriteRepository.RemoveFavoriteAsync(userId, productId);
        }

        public async Task<IEnumerable<Favorite>> GetUserFavoritesAsync(int userId)
        {
            var entities = await _favoriteRepository.GetUserFavoritesAsync(userId);
            return _mapper.Map<IEnumerable<Favorite>>(entities);
        }
    }
}