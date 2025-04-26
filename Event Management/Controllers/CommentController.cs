using Event_Management.Exceptions;
using Event_Management.Models;
using Event_Management.Models.Dtos.CommentDtos;
using Event_Management.Repositories.CommentRepositoryFolder;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Event_Management.Controllers
{
    [Authorize] // This attribute ensures that all actions in this controller require authorization.
    [Route("api/[controller]")]
    [ApiController]
    public class CommentController : ControllerBase
    {
        private readonly ICommentRepository _commentRepository; // This is the repository that will handle the data access for comments.

        public CommentController(ICommentRepository commentRepository)
        {
            _commentRepository = commentRepository;
        }

        [Authorize(Roles = "ADMINISTRATOR")]
        [HttpGet("get-all-comments")]
        public async Task<ActionResult<IEnumerable<Comment>>> GetAllComments()
        {
            try
            {
                var comments = await _commentRepository.GetCommentsAsync();

                return comments == null ? throw new NotFoundException("Comments not found!") : Ok(new { comments });
            }
            catch (Exception ex)
            {
                throw new BadRequestException(ex.Message, ex.InnerException);
            }
        }

        [HttpGet("get-comment-by-id/{commentId}")]
        public async Task<ActionResult<Comment>> GetCommentsById(int commentId)
        {
            try
            {
                var comment = await _commentRepository.GetCommentByIdAsync(commentId);

                return comment == null ? throw new NotFoundException("Comment not found!") : Ok(new { comment });
            }
            catch (Exception ex)
            {
                throw new BadRequestException(ex.Message, ex.InnerException);
            }
        }

        [HttpGet("get-comments-by-event-id/{eventId}")]
        public async Task<ActionResult<IEnumerable<Comment>>> GetCommentsByEventId(int eventId)
        {
            try
            {
                var comments = await _commentRepository.GetCommentsByEventIdAsync(eventId);

                return comments == null ? throw new NotFoundException("Comments not found!") : Ok(new { comments });
            }
            catch (Exception ex)
            {
                throw new BadRequestException(ex.Message, ex.InnerException);
            }
        }

        [HttpGet("get-comments-by-user-id/{userId}")]
        public async Task<ActionResult<IEnumerable<Comment>>> GetCommentsByUserId(int userId)
        {
            try
            {
                var comments = await _commentRepository.GetCommentsByUserIdAsync(userId);

                return comments == null ? throw new NotFoundException("Comments not found!") : Ok(new { comments });
            }
            catch (Exception ex)
            {
                throw new BadRequestException(ex.Message, ex.InnerException);
            }
        }

        [HttpPost("add-comment")]
        public async Task<ActionResult<Comment>> AddComment([FromForm] CommentCreateDto commentCreateDto)
        {
            try
            {
                var comment = await _commentRepository.AddCommentAsync(commentCreateDto);

                return comment == null ? throw new NotFoundException("Comment creation process failed!") : Ok(new { comment });
            }
            catch (Exception ex)
            {
                throw new BadRequestException(ex.Message, ex.InnerException);
            }
        }

        [HttpPut("update-comment/{commentId}&{userId}")]
        public async Task<ActionResult<string>> UpdateComment(int commentId, int userId, [FromForm] CommentUpdateDto commentUpdateDto)
        {
            try
            {
                var isUpdated = await _commentRepository.UpdateCommentAsync(commentId, userId, commentUpdateDto);

                return !isUpdated ? throw new NotFoundException("Comment update process failed!") : Ok(new { message = "Comment updated successfully!" });
            }
            catch (Exception ex)
            {
                throw new BadRequestException(ex.Message, ex.InnerException);
            }
        }

        [HttpDelete("remove-comment/{commentId}&{userId}")]
        public async Task<ActionResult<string>> RemoveComment(int commentId, int userId)
        {
            try
            {
                var isRemoved = await _commentRepository.DeleteCommentAsync(commentId, userId);

                return !isRemoved ? throw new NotFoundException("Comment deletion process failed!") : Ok(new { message = "Comment removed successfully!" });
            }
            catch (Exception ex)
            {
                throw new BadRequestException(ex.Message, ex.InnerException);
            }
        }
    }
}
