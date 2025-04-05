using AutoMapper;
using Event_Management.Models.Dtos.PurchaseDtos;
using Event_Management.Models;

namespace Event_Management.Helpers
{
    public class PurchaseMappingProfile : Profile
    {
        public PurchaseMappingProfile() 
        {
            CreateMap<Purchase, PurchaseDto>()
                .ForMember(dest => dest.User, opt => opt.MapFrom(src => src.User))
                .ForMember(dest => dest.PromoCode, opt => opt.MapFrom(src => src.PromoCode))
                .ForMember(dest => dest.Tickets, opt => opt.MapFrom(src => src.Tickets));

            CreateMap<PurchaseDto, Purchase>();

            CreateMap<PurchaseCreateDto, Purchase>();

            CreateMap<PurchaseUpdateDto, Purchase>()
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status));
        }
    }
}
