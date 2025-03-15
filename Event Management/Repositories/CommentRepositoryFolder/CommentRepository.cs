using AutoMapper;
using Event_Management.Data;
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
                .FirstOrDefaultAsync(x => x.Id == id);
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

        public async Task<bool> UpdateCommentAsync(int id, CommentUpdateDto commentUpdateDto)
        {
            var existingComment = await _context.Comments.FindAsync(id);
            if (existingComment == null) return false;

            _mapper.Map(commentUpdateDto, existingComment);

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteCommentAsync(int id)
        {
            var comment = await _context.Comments.FindAsync(id);
            if (comment == null) return false;

            _context.Comments.Remove(comment);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
