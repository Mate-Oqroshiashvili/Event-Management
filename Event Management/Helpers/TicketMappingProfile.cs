using AutoMapper;
using Event_Management.Models.Dtos.TicketDtos;
using Event_Management.Models;

namespace Event_Management.Helpers
{
    public class TicketMappingProfile : Profile
    {
        public TicketMappingProfile() 
        {
            CreateMap<Ticket, TicketDto>()
                .ForMember(dest => dest.Event, opt => opt.MapFrom(src => src.Event))
                .ForMember(dest => dest.Users, opt => opt.MapFrom(src => src.Users))
                .ForMember(dest => dest.Purchases, opt => opt.MapFrom(src => src.Purchases))
                .ForMember(dest => dest.Participants, opt => opt.MapFrom(src => src.Participants));

            CreateMap<TicketDto, Ticket>();
                //.ForMember(dest => dest.Event, opt => opt.Ignore())
                //.ForMember(dest => dest.User, opt => opt.Ignore())
                //.ForMember(dest => dest.Purchase, opt => opt.Ignore())
                //.ForMember(dest => dest.Participant, opt => opt.Ignore());

            CreateMap<TicketCreateDto, Ticket>();

            CreateMap<TicketUpdateDto, Ticket>()
                .ForMember(dest => dest.Price, opt => opt.MapFrom(src => src.Price))
                .ForMember(dest => dest.Quantity, opt => opt.MapFrom(src => src.Quantity))
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status));
        }
    }
}
