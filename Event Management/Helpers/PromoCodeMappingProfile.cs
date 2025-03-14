using AutoMapper;
using Event_Management.Models.Dtos.PromoCodeDtos;

namespace Event_Management.Helpers
{
    public class PromoCodeMappingProfile : Profile
    {
        public PromoCodeMappingProfile() 
        {
            CreateMap<PromoCodeMappingProfile, PromoCodeCreateDto>().ReverseMap();
            CreateMap<PromoCodeMappingProfile, PromoCodeDto>().ReverseMap();
            CreateMap<PromoCodeMappingProfile, PromoCodeUpdateDto>().ReverseMap();
        }
    }
}
