using AutoMapper;
using ECommerceAPI.Application.Models;
using ECommerceAPI.Infrastructure.Entities;
using ECommerceAPI.WebApi.DTOs.RequestModels;
using ECommerceAPI.WebApi.DTOs.ResponseModels;

namespace ECommerceAPI.Application.Profiles
{
    public class ProductProfile : Profile
    {
        public ProductProfile()
        {
            CreateMap<ProductUpsertRequest, Product>();
            CreateMap<(int Id, ProductUpsertRequest Request), Product>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Request.Name))
                .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.Request.Description))
                .ForMember(dest => dest.Price, opt => opt.MapFrom(src => src.Request.Price))
                .ForMember(dest => dest.Stock, opt => opt.MapFrom(src => src.Request.Stock))
                .ForMember(dest => dest.Media, opt => opt.MapFrom(src => src.Request.Media));
            CreateMap<ProductEntity, Product>().ReverseMap();
            CreateMap<Product, ProductResponse>();
            CreateMap<Product, ProductListResponse>()
                .ForMember(dest => dest.PrimaryImageUrl, opt =>
                    opt.MapFrom(src => src.Media.FirstOrDefault().MediaUrl));
            CreateMap<ProductMediaRequest, ProductMedia>();
            CreateMap<ProductMedia, ProductMediaResponse>();
            CreateMap<ProductMediaEntity, ProductMedia>().ReverseMap();
        }
    }
}