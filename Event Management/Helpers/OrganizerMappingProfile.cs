using AutoMapper;
using Event_Management.Models.Dtos.OrganizerDtos;
using Event_Management.Models;

namespace Event_Management.Helpers
{
    public class OrganizerMappingProfile : Profile
    {
        public OrganizerMappingProfile()
        {
            CreateMap<Organizer, OrganizerDto>()
                .ForMember(dest => dest.User, opt => opt.MapFrom(src => src.User))
                .ForMember(dest => dest.Locations, opt => opt.MapFrom(src => src.Locations))
                .ReverseMap();

            CreateMap<OrganizerCreateDto, Organizer>();

            CreateMap<OrganizerUpdateDto, Organizer>();
        }
    }
}
