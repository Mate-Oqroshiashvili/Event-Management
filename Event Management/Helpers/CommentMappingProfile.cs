using AutoMapper;
using Event_Management.Models;
using Event_Management.Models.Dtos.CommentDtos;

namespace Event_Management.Helpers
{
    public class CommentMappingProfile : Profile
    {
        public CommentMappingProfile()
        {
            CreateMap<CommentDto, Comment>()
                .ForMember(dest => dest.User, opt => opt.MapFrom(src => src.User))
                .ReverseMap();

            CreateMap<CommentCreateDto, Comment>().ReverseMap();

            // Mapping for Updating a Comment (Only CommentContent should be updated)
            CreateMap<CommentUpdateDto, Comment>().ReverseMap();
        }
    }
}
