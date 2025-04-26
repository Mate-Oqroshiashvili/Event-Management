using Event_Management.Models.Dtos.CommentDtos;

namespace Event_Management.Repositories.CommentRepositoryFolder
{
    public interface ICommentRepository
    {
        Task<IEnumerable<CommentDto>> GetCommentsAsync(); // Retrieves all comments
        Task<CommentDto> GetCommentByIdAsync(int id); // Retrieves a comment by its ID
        Task<IEnumerable<CommentDto>> GetCommentsByEventIdAsync(int eventId); // Retrieves comments by event ID
        Task<IEnumerable<CommentDto>> GetCommentsByUserIdAsync(int userId); // Retrieves comments by user ID
        Task<CommentDto> AddCommentAsync(CommentCreateDto commentCreateDto); // Adds a new comment
        Task<bool> UpdateCommentAsync(int CommentId, int userId, CommentUpdateDto commentUpdateDto); // Updates an existing comment
        Task<bool> DeleteCommentAsync(int id, int userId); // Deletes a comment by its ID
    }
}
