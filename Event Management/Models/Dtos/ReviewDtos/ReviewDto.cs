namespace Event_Management.Models.Dtos.ReviewDtos
{
    public class ReviewDto
    {
        public int Id { get; set; }
        public int StarCount { get; set; }
        public int UserId { get; set; }
        public User User { get; set; }
        public int EventId { get; set; }
    }
}
