using Event_Management.Models.Dtos.EventDtos;
using Event_Management.Models.Dtos.TicketDtos;
using Event_Management.Models.Dtos.UserDtos;

namespace Event_Management.Models.Dtos.ParticipantDtos
{
    public class ParticipantDto
    {
        public int Id { get; set; }
        public DateTime RegistrationDate { get; set; }
        public bool Attendance { get; set; }
        public EventDto Event { get; set; }
        public TicketDto Ticket { get; set; }
        public UserDto User { get; set; }

        public ParticipantDto()
        {
            
        }
    }
}
