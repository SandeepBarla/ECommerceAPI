using AutoMapper;
using ECommerceAPI.Application.Models;
using ECommerceAPI.Infrastructure.Entities;

namespace ECommerceAPI.Application.Profiles
{
    public class OrderProfile : Profile
    {
        public OrderProfile()
        {
            CreateMap<OrderEntity, Order>().ReverseMap();
            CreateMap<OrderProductEntity, OrderProduct>().ReverseMap();
        }
    }
}