namespace Event_Management.Models
{
    public class Review
    {
        public int Id { get; set; }
        public int StarCount { get; set; }
        public int UserId { get; set; }
        public User User { get; set; }
        public int EventId { get; set; }
        public Event Event { get; set; }
    }
}
