using AutoMapper;
using Event_Management.Models.Dtos.LocationDtos;
using Event_Management.Models;

namespace Event_Management.Helpers
{
    public class LocationMappingProfile : Profile
    {
        public LocationMappingProfile() 
        {
            CreateMap<Location, LocationDto>().ReverseMap();

            CreateMap<LocationCreateDto, Location>();

            CreateMap<LocationUpdateDto, Location>();
        }
    }
}
