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
            
            CreateMap<(int id, SizeUpsertRequest request), Size>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.id))
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.request.Name))
                .ForMember(dest => dest.SortOrder, opt => opt.MapFrom(src => src.request.SortOrder));
        }
    }
}