using Event_Management.Models.Dtos.LocationDtos;
using Event_Management.Models.Dtos.OrganizerDtos;
using Event_Management.Models.Dtos.ParticipantDtos;
using Event_Management.Models.Dtos.PromoCodeDtos;
using Event_Management.Models.Dtos.TicketDtos;
using Event_Management.Models.Dtos.UserDtos;
using Event_Management.Models.Enums;

namespace Event_Management.Models.Dtos.EventDtos
{
    public class EventDto
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public LocationDto Location { get; set; }
        public int Capacity { get; set; }
        public OrganizerDto Organizer { get; set; }
        public EventStatus Status { get; set; }
        public IEnumerable<string> Images { get; set; }
        public IEnumerable<TicketDto>? Tickets { get; set; }
        public IEnumerable<ParticipantDto>? Participants { get; set; }
        public IEnumerable<UserDto> SpeakersAndArtists { get; set; }
        public IEnumerable<PromoCodeDto> PromoCodes { get; set; }

        public EventDto()
        {
            
        }
    }
}
