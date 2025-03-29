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
                .ForMember(dest => dest.Tickets, opt => opt.MapFrom(src => src.Tickets))
                .ForMember(dest => dest.Participants, opt => opt.MapFrom(src => src.Participants));

            CreateMap<PurchaseDto, Purchase>();
                //.ForMember(dest => dest.User, opt => opt.Ignore())
                //.ForMember(dest => dest.PromoCode, opt => opt.Ignore())
                //.ForMember(dest => dest.Tickets, opt => opt.Ignore());

            CreateMap<PurchaseCreateDto, Purchase>();

            CreateMap<PurchaseUpdateDto, Purchase>()
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status));
        }
    }
}
