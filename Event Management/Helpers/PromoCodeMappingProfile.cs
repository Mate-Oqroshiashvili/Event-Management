using AutoMapper;
using Event_Management.Models;
using Event_Management.Models.Dtos.PromoCodeDtos;

namespace Event_Management.Helpers
{
    public class PromoCodeMappingProfile : Profile
    {
        public PromoCodeMappingProfile() 
        {
            CreateMap<PromoCode, PromoCodeCreateDto>().ReverseMap();
            CreateMap<PromoCode, PromoCodeDto>()
                .ForMember(dest => dest.Event, opt => opt.MapFrom(src => src.Event))
                .ReverseMap();
            CreateMap<PromoCode, PromoCodeUpdateDto>().ReverseMap();
            CreateMap<UsedPromoCode, UsedPromoCodeDto>().ReverseMap();
        }
    }
}
