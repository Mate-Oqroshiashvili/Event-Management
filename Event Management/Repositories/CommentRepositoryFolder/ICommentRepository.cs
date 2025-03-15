using Event_Management.Models;
using Event_Management.Models.Dtos.CommentDtos;

namespace Event_Management.Repositories.CommentRepositoryFolder
{
    public interface ICommentRepository
    {
        Task<IEnumerable<Comment>> GetCommentsAsync();
        Task<Comment> GetCommentByIdAsync(int id);
        Task<Comment> AddCommentAsync(CommentCreateDto commentCreateDto);
        Task<bool> UpdateCommentAsync(int id, CommentUpdateDto commentUpdateDto);
        Task<bool> DeleteCommentAsync(int id);
    }
}
