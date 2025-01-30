using AutoMapper;
using ECommerceAPI.Application.Models;
using ECommerceAPI.Infrastructure.Entities;
using ECommerceAPI.WebApi.DTOs.RequestModels;
using ECommerceAPI.WebApi.DTOs.ResponseModels;

namespace ECommerceAPI.Application.Profiles
{
    public class CartProfile : Profile
    {
        public CartProfile()
        {
            CreateMap<CartAddOrUpdateItemRequest, CartItem>();

            CreateMap<CartEntity, Cart>().ReverseMap();
            CreateMap<CartItemEntity, CartItem>().ReverseMap();

            CreateMap<Cart, CartResponse>();
            CreateMap<CartItem, CartItemResponse>();
        }
    }
}