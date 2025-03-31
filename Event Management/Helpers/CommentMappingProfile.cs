using AutoMapper;
using Event_Management.Models;
using Event_Management.Models.Dtos.CommentDtos;

namespace Event_Management.Helpers
{
    public class CommentMappingProfile : Profile
    {
        public CommentMappingProfile()
        {
            CreateMap<CommentDto, Comment>().ReverseMap();

            CreateMap<CommentCreateDto, Comment>().ReverseMap();
                //.ForMember(dest => dest.Id, opt => opt.Ignore()) // Ignore Id since it's auto-generated
                //.ForMember(dest => dest.User, opt => opt.Ignore()) // User navigation property is not needed in DTO
                //.ForMember(dest => dest.Event, opt => opt.Ignore()); // Event navigation property is not needed in DTO

            // Mapping for Updating a Comment (Only CommentContent should be updated)
            CreateMap<CommentUpdateDto, Comment>().ReverseMap();
            //    .ForMember(dest => dest.Id, opt => opt.Ignore()) // Ensure Id is not modified
            //    .ForMember(dest => dest.UserId, opt => opt.Ignore()) // UserId should not be updated
            //    .ForMember(dest => dest.EventId, opt => opt.Ignore()) // EventId should not be updated
            //    .ForMember(dest => dest.User, opt => opt.Ignore()) // Ignore navigation property
            //    .ForMember(dest => dest.Event, opt => opt.Ignore());
        }
    }
}
