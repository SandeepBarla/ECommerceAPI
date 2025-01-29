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
            CreateMap<OrderProduct, OrderProductResponse>();
            CreateMap<Order, OrderResponse>();
        }
    }
}   