namespace Event_Management.Models.Dtos.CommentDtos
{
    public class CommentCreateDto
    {
        public string CommentContent { get; set; }
        public int UserId { get; set; }
        public int EventId { get; set; }
    }
}
