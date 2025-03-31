using Event_Management.Models.Dtos.CommentDtos;

namespace Event_Management.Repositories.CommentRepositoryFolder
{
    public interface ICommentRepository
    {
        Task<IEnumerable<CommentDto>> GetCommentsAsync();
        Task<CommentDto> GetCommentByIdAsync(int id);
        Task<IEnumerable<CommentDto>> GetCommentsByEventIdAsync(int eventId);
        Task<IEnumerable<CommentDto>> GetCommentsByUserIdAsync(int userId);
        Task<CommentDto> AddCommentAsync(CommentCreateDto commentCreateDto);
        Task<bool> UpdateCommentAsync(int CommentId, int userId, CommentUpdateDto commentUpdateDto);
        Task<bool> DeleteCommentAsync(int id, int userId);
    }
}
