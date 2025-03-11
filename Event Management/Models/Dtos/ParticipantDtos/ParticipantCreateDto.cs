namespace Event_Management.Models.Dtos.ParticipantDtos
{
    public class ParticipantCreateDto
    {
        public int EventId { get; set; }
        public int UserId { get; set; }
        public int TicketId { get; set; }

        public ParticipantCreateDto()
        {
            
        }
    }
}
