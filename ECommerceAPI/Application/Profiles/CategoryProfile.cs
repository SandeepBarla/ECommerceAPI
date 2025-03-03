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
            // ✅ Create Category (Ignore Id)
            CreateMap<CategoryUpsertRequest, Category>()
                .ForMember(dest => dest.Id, opt => opt.Ignore()); // Id is ignored by default
            
            // ✅ Update Category (Uses Id from the route)
            CreateMap<(int id, CategoryUpsertRequest request), Category>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.id)) // Map Id from route
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.request.Name));
        }
    }
}