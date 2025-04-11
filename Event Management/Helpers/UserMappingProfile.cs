using AutoMapper;
using Event_Management.Models.Dtos.UserDtos;
using Event_Management.Models;

namespace Event_Management.Helpers
{
    public class UserMappingProfile : Profile
    {
        public UserMappingProfile() 
        {
            CreateMap<User, UserDto>()
                .ForMember(dest => dest.Organizer, opt => opt.MapFrom(src => src.Organizer))
                .ForMember(dest => dest.Purchases, opt => opt.MapFrom(src => src.Purchases))
                .ForMember(dest => dest.UsedPromoCodes, opt => opt.MapFrom(src => src.UsedPromoCodes));

            CreateMap<UserDto, User>();

            CreateMap<UserCreateDto, User>()
                .ForMember(dest => dest.PasswordHash, opt => opt.Ignore());

            CreateMap<UserUpdateDto, User>()
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name))
                .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Email))
                .ForMember(dest => dest.PhoneNumber, opt => opt.MapFrom(src => src.PhoneNumber));
        }
    }
}
