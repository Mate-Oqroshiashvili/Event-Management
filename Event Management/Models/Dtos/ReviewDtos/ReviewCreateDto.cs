namespace Event_Management.Models.Dtos.ReviewDtos
{
    public class ReviewCreateDto
    {
        public int StarCount { get; set; }
        public int UserId { get; set; }
        public int EventId { get; set; }
    }
}
