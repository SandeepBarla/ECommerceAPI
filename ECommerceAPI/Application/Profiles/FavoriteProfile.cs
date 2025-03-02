using AutoMapper;
using ECommerceAPI.Application.Models;
using ECommerceAPI.Infrastructure.Entities;
using ECommerceAPI.WebApi.DTOs.ResponseModels;

namespace ECommerceAPI.Application.Profiles
{
    public class FavoriteProfile : Profile
    {
        public FavoriteProfile()
        {
            // ✅ Entity to Model
            CreateMap<FavoriteEntity, Favorite>().ReverseMap();

            // ✅ Model to Response
            CreateMap<Favorite, FavoriteResponse>();
        }
    }
}