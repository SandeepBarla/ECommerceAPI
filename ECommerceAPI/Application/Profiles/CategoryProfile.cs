using AutoMapper;
using ECommerceAPI.Application.Models;
using ECommerceAPI.Infrastructure.Entities;
using ECommerceAPI.WebApi.DTOs.RequestModels;
using ECommerceAPI.WebApi.DTOs.ResponseModels;

namespace ECommerceAPI.Application.Profiles
{
    public class CategoryProfile : Profile
    {
        public CategoryProfile()
        {
            // ✅ Entity to Model
            CreateMap<CategoryEntity, Category>().ReverseMap();

            // ✅ Model to Response
            CreateMap<Category, CategoryResponse>();

            // ✅ Request to Model
            CreateMap<CategoryUpsertRequest, Category>();
        }
    }
}