using AutoMapper;
using Event_Management.Models;
using Event_Management.Models.Dtos.ReviewDtos;

namespace Event_Management.Helpers
{
    public class ReviewMappingProfile : Profile
    {
        public ReviewMappingProfile()
        {
            CreateMap<ReviewDto, Review>()
                .ForMember(dest => dest.User, opt => opt.MapFrom(src => src.User))
                .ReverseMap();

            CreateMap<ReviewCreateDto, Review>().ReverseMap();

            // Mapping for Updating a Review (Only StarCount should be updated)
            CreateMap<ReviewUpdateDto, Review>().ReverseMap();
        }
    }
}
