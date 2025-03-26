using AutoMapper;
using Event_Management.Models.Dtos.EventDtos;
using Event_Management.Models;
using Event_Management.Models.Dtos.LocationDtos;
using Event_Management.Models.Dtos.OrganizerDtos;

namespace Event_Management.Helpers
{
    public class EventMappingProfile : Profile
    {
        public EventMappingProfile()
        {
            CreateMap<Event, EventDto>()
                .ForMember(dest => dest.Location, opt => opt.MapFrom(src => src.Location))
                .ForMember(dest => dest.Organizer, opt => opt.MapFrom(src => src.Organizer))
                .ForMember(dest => dest.Tickets, opt => opt.MapFrom(src => src.Tickets))
                .ForMember(dest => dest.Participants, opt => opt.MapFrom(src => src.Participants))
                .ForMember(dest => dest.SpeakersAndArtists, opt => opt.MapFrom(src => src.SpeakersAndArtists))
                .ForMember(dest => dest.Reviews, opt => opt.MapFrom(src => src.Reviews))
                .ForMember(dest => dest.Comments, opt => opt.MapFrom(src => src.Comments))
                .ForMember(dest => dest.PromoCodes, opt => opt.MapFrom(src => src.PromoCodes))
                .ReverseMap();

            CreateMap<Location, LocationDto>()
                .ForMember(dest => dest.Events, opt => opt.MapFrom(src => src.Events))
                .ForMember(dest => dest.Organizers, opt => opt.MapFrom(src => src.Organizers))
                .ReverseMap();

            CreateMap<Organizer, OrganizerDto>()
                .ForMember(dest => dest.Events, opt => opt.MapFrom(src => src.Events))
                .ForMember(dest => dest.Locations, opt => opt.MapFrom(src => src.Locations))
                .ReverseMap();
        }
    }
}
