using AutoMapper;
using ECommerceAPI.Application.Models;
using ECommerceAPI.Infrastructure.Entities;

namespace ECommerceAPI.Application.Profiles
{
    public class CartProfile : Profile
    {
        public CartProfile()
        {
            CreateMap<CartEntity, Cart>().ReverseMap();
            CreateMap<CartItemEntity, CartItem>().ReverseMap();
        }
    }
}