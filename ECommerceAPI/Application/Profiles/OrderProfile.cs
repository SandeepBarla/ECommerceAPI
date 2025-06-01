using AutoMapper;
using ECommerceAPI.Application.Models;
using ECommerceAPI.Infrastructure.Entities;
using ECommerceAPI.WebApi.DTOs.RequestModels;
using ECommerceAPI.WebApi.DTOs.ResponseModels;

namespace ECommerceAPI.Application.Profiles
{
    public class OrderProfile : Profile
    {
        public OrderProfile()
        {
            CreateMap<OrderCreateRequest, Order>()
                //.ForMember(dest => dest.OrderStatus, opt => opt.MapFrom(src => "Pending"))
                .ForMember(dest => dest.OrderDate, opt => opt.MapFrom(src => DateTime.UtcNow));

            CreateMap<OrderProductRequest, OrderProduct>();
            CreateMap<OrderEntity, Order>().ReverseMap();
            CreateMap<OrderProductEntity, OrderProduct>().ReverseMap();

            // Enhanced OrderProduct mapping with product details
            CreateMap<OrderProduct, OrderProductResponse>();
            CreateMap<OrderProductEntity, OrderProductResponse>()
                .ForMember(dest => dest.ProductName, opt => opt.MapFrom(src => src.Product != null ? src.Product.Name : "Unknown Product"))
                .ForMember(dest => dest.Price, opt => opt.MapFrom(src => src.Product != null ? (src.Product.DiscountedPrice ?? src.Product.OriginalPrice) : 0));

            // Enhanced OrderEntity to OrderResponse mapping with address and customer info
            CreateMap<OrderEntity, OrderResponse>()
                .ForMember(dest => dest.Customer, opt => opt.MapFrom(src => src.User))
                .ForMember(dest => dest.Address, opt => opt.MapFrom(src => src.Address));

            // Order domain model to OrderResponse mapping - needed for controller tests
            CreateMap<Order, OrderResponse>();

            // Customer info mapping
            CreateMap<UserEntity, CustomerInfoResponse>();

            // Address mapping - needed for OrderEntity.Address to OrderResponse.Address
            CreateMap<AddressEntity, AddressResponse>();
        }
    }
}