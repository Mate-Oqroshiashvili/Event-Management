using AutoMapper;
using Event_Management.Models.Dtos.LocationDtos;
using Event_Management.Models;

namespace Event_Management.Helpers
{
    public class LocationMappingProfile : Profile
    {
        public LocationMappingProfile() 
        {
            CreateMap<Location, LocationDto>()
                //.ForMember(dest => dest.Events, opt => opt.MapFrom(src => src.Events))
                //.ForMember(dest => dest.Organizers, opt => opt.MapFrom(src => src.Organizers))
                .ReverseMap();

            //CreateMap<LocationDto, Location>()
            //    .ForMember(dest => dest.Events, opt => opt.Ignore())
            //    .ForMember(dest => dest.Organizers, opt => opt.Ignore());

            CreateMap<LocationCreateDto, Location>();

            CreateMap<LocationUpdateDto, Location>();
        }
    }
}
