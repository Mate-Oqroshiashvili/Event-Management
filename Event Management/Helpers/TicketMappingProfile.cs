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
                .ForMember(dest => dest.Purchases, opt => opt.MapFrom(src => src.Purchases));

            CreateMap<TicketDto, Ticket>();

            CreateMap<TicketCreateDto, Ticket>();

            CreateMap<TicketUpdateDto, Ticket>();
        }
    }
}
