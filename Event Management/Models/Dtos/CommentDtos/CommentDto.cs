namespace Event_Management.Models.Dtos.CommentDtos
{
    public class CommentDto
    {
        public int Id { get; set; }
        public string CommentContent { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public int UserId { get; set; }
        public User User { get; set; }
        public int EventId { get; set; }
    }
}
