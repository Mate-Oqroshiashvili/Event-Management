using AutoMapper;
using Event_Management.Models.Dtos.EventDtos;
using Event_Management.Models;

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
                .ForMember(dest => dest.PromoCodes, opt => opt.MapFrom(src => src.PromoCodes));

            CreateMap<EventDto, Event>()
                .ForMember(dest => dest.Location, opt => opt.Ignore())
                .ForMember(dest => dest.Organizer, opt => opt.Ignore())
                .ForMember(dest => dest.Tickets, opt => opt.Ignore())
                .ForMember(dest => dest.Participants, opt => opt.Ignore())
                .ForMember(dest => dest.SpeakersAndArtists, opt => opt.Ignore())
                .ForMember(dest => dest.PromoCodes, opt => opt.Ignore());

            CreateMap<EventCreateDto, Event>();

            CreateMap<EventUpdateDto, Event>()
                .ForMember(dest => dest.SpeakersAndArtists, opt => opt.MapFrom(src => src.SpeakersAndArtists))
                .ForMember(dest => dest.PromoCodes, opt => opt.MapFrom(src => src.PromoCodes));
        }
    }
}
