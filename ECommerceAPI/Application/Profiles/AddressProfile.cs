using AutoMapper;
using ECommerceAPI.Application.Models;
using ECommerceAPI.Infrastructure.Entities;
using ECommerceAPI.WebApi.DTOs.RequestModels;
using ECommerceAPI.WebApi.DTOs.ResponseModels;

namespace ECommerceAPI.Application.Profiles
{
  public class AddressProfile : Profile
  {
    public AddressProfile()
    {
      // Entity to Model
      CreateMap<AddressEntity, Address>().ReverseMap();

      // Model to Response
      CreateMap<Address, AddressResponse>();

      // Request to Model
      CreateMap<AddressUpsertRequest, Address>()
          .ForMember(dest => dest.Id, opt => opt.Ignore())
          .ForMember(dest => dest.UserId, opt => opt.Ignore());

      // Update mapping (includes Id from route and UserId)
      CreateMap<(int id, int userId, AddressUpsertRequest request), Address>()
          .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.id))
          .ForMember(dest => dest.UserId, opt => opt.MapFrom(src => src.userId))
          .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.request.Name))
          .ForMember(dest => dest.Street, opt => opt.MapFrom(src => src.request.Street))
          .ForMember(dest => dest.City, opt => opt.MapFrom(src => src.request.City))
          .ForMember(dest => dest.State, opt => opt.MapFrom(src => src.request.State))
          .ForMember(dest => dest.Pincode, opt => opt.MapFrom(src => src.request.Pincode))
          .ForMember(dest => dest.Phone, opt => opt.MapFrom(src => src.request.Phone))
          .ForMember(dest => dest.IsDefault, opt => opt.MapFrom(src => src.request.IsDefault));
    }
  }
}