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
            CreateMap<(int userId, int productId), Favorite>()
                .ForMember(dest => dest.UserId, opt => opt.MapFrom(src => src.userId))
                .ForMember(dest => dest.ProductId, opt => opt.MapFrom(src => src.productId));

            // ✅ Entity to Model
            CreateMap<FavoriteEntity, Favorite>().ReverseMap();

            // ✅ Model to Response
            CreateMap<Favorite, FavoriteResponse>()
                .ForMember(dest => dest.Name, opt =>
                    opt.MapFrom(src => src.Product.Name))
                .ForMember(dest => dest.Price, opt =>
                    opt.MapFrom(src => src.Product.DiscountedPrice ?? src.Product.OriginalPrice))
                .ForMember(dest => dest.PrimaryImageUrl, opt =>
                    opt.MapFrom(src => src.Product.Media.FirstOrDefault().MediaUrl));
        }
    }
}