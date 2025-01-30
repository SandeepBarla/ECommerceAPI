using AutoMapper;
using ECommerceAPI.Application.Models;
using ECommerceAPI.Infrastructure.Entities;
using ECommerceAPI.WebApi.DTOs.RequestModels;
using ECommerceAPI.WebApi.DTOs.ResponseModels;

namespace ECommerceAPI.Application.Profiles
{
    public class UserProfile : Profile
    {
        public UserProfile()
        {
            CreateMap<UserRegisterRequest, User>();
            CreateMap<UserEntity, User>().ReverseMap();
            CreateMap<User, UserResponse>();
        }
    }
}