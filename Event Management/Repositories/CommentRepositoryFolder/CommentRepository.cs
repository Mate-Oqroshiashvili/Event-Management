using AutoMapper;
using Event_Management.Data;
using Event_Management.Exceptions;
using Event_Management.Models;
using Event_Management.Models.Dtos.CommentDtos;
using Microsoft.EntityFrameworkCore;

namespace Event_Management.Repositories.CommentRepositoryFolder
{
    public class CommentRepository : ICommentRepository
    {
        private readonly DataContext _context;
        private readonly IMapper _mapper;

        public CommentRepository(DataContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<IEnumerable<Comment>> GetCommentsAsync()
        {
            return await _context.Comments
                .Include(x => x.Event)
                .Include(x => x.User)
                .ToListAsync();
        }

        public async Task<Comment> GetCommentByIdAsync(int id)
        {
            return await _context.Comments
                .Include(x => x.Event)
                .Include(x => x.User)
                .FirstOrDefaultAsync(x => x.Id == id)
                ?? throw new NotFoundException("Comment not found!");
        }

        public async Task<IEnumerable<Comment>> GetCommentsByEventIdAsync(int eventId)
        {
            return await _context.Comments
                .Where(c => c.EventId == eventId)
                .Include(x => x.Event)
                .Include(x => x.User)
                .ToListAsync();
        }

        public async Task<IEnumerable<Comment>> GetCommentsByUserIdAsync(int userId)
        {
            return await _context.Comments
                .Where(c => c.UserId == userId)
                .Include(x => x.Event)
                .Include(x => x.User)
                .ToListAsync();
        }

        public async Task<Comment> AddCommentAsync(CommentCreateDto commentCreateDto)
        {
            var comment = _mapper.Map<Comment>(commentCreateDto);
            comment.UserId = commentCreateDto.UserId;
            comment.EventId = commentCreateDto.EventId;

            await _context.Comments.AddAsync(comment);
            await _context.SaveChangesAsync();

            return comment;
        }

        public async Task<bool> UpdateCommentAsync(int commentId, int userId, CommentUpdateDto commentUpdateDto)
        {
            var existingComment = await _context.Comments.FindAsync(commentId);
            if (existingComment == null) return false;

            if (existingComment.UserId == userId)
                throw new BadRequestException("You don't have permission to update the comment");

            _mapper.Map(commentUpdateDto, existingComment);

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteCommentAsync(int id, int userId)
        {
            var comment = await _context.Comments.FindAsync(id);
            if (comment == null) return false;

            if (comment.UserId == userId)
                throw new BadRequestException("You don't have permission to delete the comment");

            _context.Comments.Remove(comment);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
