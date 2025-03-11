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
                .ForMember(dest => dest.Events, opt => opt.MapFrom(src => src.Events))
                .ForMember(dest => dest.Locations, opt => opt.MapFrom(src => src.Locations))
                .ForMember(dest => dest.User, opt => opt.MapFrom(src => src.User));

            CreateMap<OrganizerDto, Organizer>()
                .ForMember(dest => dest.Events, opt => opt.Ignore())
                .ForMember(dest => dest.Locations, opt => opt.Ignore())
                .ForMember(dest => dest.User, opt => opt.Ignore());

            CreateMap<OrganizerCreateDto, Organizer>();

            CreateMap<OrganizerUpdateDto, Organizer>();
        }
    }
}
