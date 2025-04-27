using AutoMapper;
using Event_Management.Models;
using Event_Management.Models.Dtos.EventDtos;

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
                .ForMember(dest => dest.SpeakersAndArtists, opt => opt.MapFrom(src => src.SpeakersAndArtists))
                .ForMember(dest => dest.Reviews, opt => opt.MapFrom(src => src.Reviews))
                .ForMember(dest => dest.Comments, opt => opt.MapFrom(src => src.Comments))
                .ReverseMap();

            CreateMap<EventCreateDto, Event>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.Images, opt => opt.Ignore());

            CreateMap<EventUpdateDto, Event>();
        }
    }
}
