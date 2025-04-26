namespace Event_Management.Models
{
    public class Comment
    {
        public int Id { get; set; } // Unique identifier for the comment
        public string CommentContent { get; set; } // Content of the comment
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow; // Date when the comment was created
        public int UserId { get; set; } // User ID of the person who made the comment
        public User User { get; set; } // User associated with the comment
        public int EventId { get; set; } // Event ID to which the comment belongs
        public Event Event { get; set; } // Event associated with the comment

        public Comment() { }
    }
}
