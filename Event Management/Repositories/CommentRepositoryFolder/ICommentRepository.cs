using Event_Management.Models;
using Event_Management.Models.Dtos.CommentDtos;

namespace Event_Management.Repositories.CommentRepositoryFolder
{
    public interface ICommentRepository
    {
        Task<IEnumerable<Comment>> GetCommentsAsync();
        Task<Comment> GetCommentByIdAsync(int id);
        Task<IEnumerable<Comment>> GetCommentsByEventIdAsync(int eventId);
        Task<IEnumerable<Comment>> GetCommentsByUserIdAsync(int userId);
        Task<Comment> AddCommentAsync(CommentCreateDto commentCreateDto);
        Task<bool> UpdateCommentAsync(int CommentId, int userId, CommentUpdateDto commentUpdateDto);
        Task<bool> DeleteCommentAsync(int id, int userId);
    }
}
