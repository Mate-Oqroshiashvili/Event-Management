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
                .ForMember(dest => dest.Ticket, opt => opt.MapFrom(src => src.Ticket))
                .ForMember(dest => dest.User, opt => opt.MapFrom(src => src.User));

            CreateMap<ParticipantDto, Participant>();

            CreateMap<ParticipantCreateDto, Participant>();

            CreateMap<ParticipantUpdateDto, Participant>()
                .ForMember(dest => dest.Attendance, opt => opt.MapFrom(src => src.Attendance));
        }
    }
}
