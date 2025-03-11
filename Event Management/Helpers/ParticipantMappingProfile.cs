using AutoMapper;
using Event_Management.Models.Dtos.ParticipantDtos;
using Event_Management.Models;

namespace Event_Management.Helpers
{
    public class ParticipantMappingProfile : Profile
    {
        public ParticipantMappingProfile()
        {
            CreateMap<Participant, ParticipantDto>()
                .ForMember(dest => dest.Event, opt => opt.MapFrom(src => src.Event))
                .ForMember(dest => dest.Ticket, opt => opt.MapFrom(src => src.Ticket))
                .ForMember(dest => dest.User, opt => opt.MapFrom(src => src.User));

            CreateMap<ParticipantDto, Participant>()
                .ForMember(dest => dest.Event, opt => opt.Ignore())
                .ForMember(dest => dest.Ticket, opt => opt.Ignore())
                .ForMember(dest => dest.User, opt => opt.Ignore());

            CreateMap<ParticipantCreateDto, Participant>();

            CreateMap<ParticipantUpdateDto, Participant>()
                .ForMember(dest => dest.Attendance, opt => opt.MapFrom(src => src.Attendance));
        }
    }
}
