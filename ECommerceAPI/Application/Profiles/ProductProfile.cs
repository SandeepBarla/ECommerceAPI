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
            CreateMap<ProductUpsertRequest, Product>()
                .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => DateTime.UtcNow))
                .ForMember(dest => dest.UpdatedAt, opt => opt.MapFrom(src => DateTime.UtcNow));

            CreateMap<(int Id, ProductUpsertRequest Request), Product>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Request.Name))
                .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.Request.Description))
                .ForMember(dest => dest.OriginalPrice, opt => opt.MapFrom(src => src.Request.OriginalPrice))
                .ForMember(dest => dest.DiscountedPrice, opt => opt.MapFrom(src => src.Request.DiscountedPrice))
                .ForMember(dest => dest.IsFeatured, opt => opt.MapFrom(src => src.Request.IsFeatured))
                .ForMember(dest => dest.NewUntil, opt => opt.MapFrom(src => src.Request.NewUntil))
                .ForMember(dest => dest.CategoryId, opt => opt.MapFrom(src => src.Request.CategoryId))
                .ForMember(dest => dest.SizeId, opt => opt.MapFrom(src => src.Request.SizeId))
                .ForMember(dest => dest.Media, opt => opt.MapFrom(src => src.Request.Media))
                .ForMember(dest => dest.UpdatedAt, opt => opt.MapFrom(src => DateTime.UtcNow));

            CreateMap<ProductEntity, Product>().ReverseMap()
                .ForMember(dest => dest.UpdatedAt, opt => opt.MapFrom(src => DateTime.UtcNow));

            CreateMap<ProductEntity, ProductResponse>()
                .ForMember(dest => dest.CategoryName, opt => opt.MapFrom(src => src.Category.Name))
                .ForMember(dest => dest.SizeName, opt => opt.MapFrom(src => src.Size.Name))
                .ForMember(dest => dest.Media, opt => opt.MapFrom(src => src.Media))
                .ForMember(dest => dest.DiscountPercentage, opt => opt.MapFrom(src =>
                    src.DiscountedPrice.HasValue && src.DiscountedPrice.Value > 0 && src.OriginalPrice > src.DiscountedPrice.Value
                        ? Math.Round(((src.OriginalPrice - src.DiscountedPrice.Value) / src.OriginalPrice) * 100, 2)
                        : (decimal?)null))
                .ForMember(dest => dest.IsNew, opt => opt.MapFrom(src =>
                    src.NewUntil.HasValue && src.NewUntil.Value > DateTime.UtcNow));

            CreateMap<ProductEntity, ProductListResponse>()
                .ForMember(dest => dest.PrimaryImageUrl, opt =>
                    opt.MapFrom(src => src.Media.Any() ? src.Media.OrderBy(m => m.OrderIndex).First().MediaUrl : ""))
                .ForMember(dest => dest.CategoryName, opt => opt.MapFrom(src => src.Category.Name))
                .ForMember(dest => dest.SizeName, opt => opt.MapFrom(src => src.Size.Name))
                .ForMember(dest => dest.DiscountPercentage, opt => opt.MapFrom(src =>
                    src.DiscountedPrice.HasValue && src.DiscountedPrice.Value > 0 && src.OriginalPrice > src.DiscountedPrice.Value
                        ? Math.Round(((src.OriginalPrice - src.DiscountedPrice.Value) / src.OriginalPrice) * 100, 2)
                        : (decimal?)null))
                .ForMember(dest => dest.IsNew, opt => opt.MapFrom(src =>
                    src.NewUntil.HasValue && src.NewUntil.Value > DateTime.UtcNow));

            CreateMap<Product, ProductResponse>()
                .ForMember(dest => dest.CategoryName, opt => opt.Ignore())
                .ForMember(dest => dest.SizeName, opt => opt.Ignore())
                .ForMember(dest => dest.DiscountPercentage, opt => opt.MapFrom(src =>
                    src.DiscountedPrice.HasValue && src.DiscountedPrice.Value > 0 && src.OriginalPrice > src.DiscountedPrice.Value
                        ? Math.Round(((src.OriginalPrice - src.DiscountedPrice.Value) / src.OriginalPrice) * 100, 2)
                        : (decimal?)null))
                .ForMember(dest => dest.IsNew, opt => opt.MapFrom(src =>
                    src.NewUntil.HasValue && src.NewUntil.Value > DateTime.UtcNow));

            CreateMap<Product, ProductListResponse>()
                .ForMember(dest => dest.PrimaryImageUrl, opt =>
                    opt.MapFrom(src => src.Media.Any() ? src.Media.OrderBy(m => m.OrderIndex).First().MediaUrl : ""))
                .ForMember(dest => dest.CategoryName, opt => opt.Ignore())
                .ForMember(dest => dest.SizeName, opt => opt.Ignore())
                .ForMember(dest => dest.DiscountPercentage, opt => opt.MapFrom(src =>
                    src.DiscountedPrice.HasValue && src.DiscountedPrice.Value > 0 && src.OriginalPrice > src.DiscountedPrice.Value
                        ? Math.Round(((src.OriginalPrice - src.DiscountedPrice.Value) / src.OriginalPrice) * 100, 2)
                        : (decimal?)null))
                .ForMember(dest => dest.IsNew, opt => opt.MapFrom(src =>
                    src.NewUntil.HasValue && src.NewUntil.Value > DateTime.UtcNow));

            CreateMap<ProductMediaRequest, ProductMedia>();
            CreateMap<ProductMedia, ProductMediaResponse>();
            CreateMap<ProductMediaEntity, ProductMedia>().ReverseMap();
            CreateMap<ProductMediaEntity, ProductMediaResponse>()
                .ForMember(dest => dest.Type, opt => opt.MapFrom(src => src.Type.ToString()));
        }
    }
}