using AutoMapper;
using ECommerceAPI.Application.Models;
using ECommerceAPI.Infrastructure.Entities;
using ECommerceAPI.WebApi.DTOs.RequestModels;
using ECommerceAPI.WebApi.DTOs.ResponseModels;

namespace ECommerceAPI.Application.Profiles
{
    public class SizeProfile : Profile
    {
        public SizeProfile()
        {
            // ✅ Entity to Model
            CreateMap<SizeEntity, Size>().ReverseMap();

            // ✅ Model to Response
            CreateMap<Size, SizeResponse>();

            // ✅ Request to Model
            CreateMap<SizeUpsertRequest, Size>();
        }
    }
}