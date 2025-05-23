﻿using AutoMapper;
using Event_Management.Data;
using Event_Management.Exceptions;
using Event_Management.Models;
using Event_Management.Models.Dtos.CommentDtos;
using Event_Management.Web_Sockets;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;

namespace Event_Management.Repositories.CommentRepositoryFolder
{
    public class CommentRepository : ICommentRepository
    {
        private readonly DataContext _context; // Database context for accessing the database
        private readonly IHubContext<CommentHub> _hubContext; // Hub context for sending messages to clients
        private readonly IMapper _mapper; // AutoMapper instance for mapping between DTOs and entities

        public CommentRepository(DataContext context, IHubContext<CommentHub> hubContext, IMapper mapper)
        {
            _context = context;
            _hubContext = hubContext;
            _mapper = mapper;
        }

        /// <summary>
        /// Retrieves all comments from the database.
        public async Task<IEnumerable<CommentDto>> GetCommentsAsync()
        {
            try
            {
                var comments = await _context.Comments
                    .Include(x => x.Event)
                    .Include(x => x.User)
                    .ToListAsync() ?? throw new NotFoundException("No comment found!");

                var commentDtos = _mapper.Map<IEnumerable<CommentDto>>(comments);

                return commentDtos;
            }
            catch (Exception ex) 
            {
                throw new BadRequestException(ex.Message, ex.InnerException);
            }
        }

        /// <summary>
        /// Retrieves a comment by its ID from the database.
        public async Task<CommentDto> GetCommentByIdAsync(int id)
        {
            try
            {
                var comment = await _context.Comments
                    .Include(x => x.Event)
                    .Include(x => x.User)
                    .FirstOrDefaultAsync(x => x.Id == id)
                    ?? throw new NotFoundException("Comment not found!");

                var commentDto = _mapper.Map<CommentDto>(comment);

                return commentDto;
            }
            catch (Exception ex) 
            {
                throw new BadRequestException(ex.Message, ex.InnerException);
            }
        }

        /// <summary>
        /// Retrieves comments by event ID from the database.
        public async Task<IEnumerable<CommentDto>> GetCommentsByEventIdAsync(int eventId)
        {
            try
            {
                var comments = await _context.Comments
                .Where(c => c.EventId == eventId)
                .Include(x => x.Event)
                .Include(x => x.User)
                .ToListAsync() ?? throw new NotFoundException("No comment found!");
                
                var commentDtos = _mapper.Map<IEnumerable<CommentDto>>(comments);

                return commentDtos;
            }
            catch (Exception ex) 
            {
                throw new BadRequestException(ex.Message, ex.InnerException);
            }
        }

        /// <summary>
        /// Retrieves comments by user ID from the database.
        public async Task<IEnumerable<CommentDto>> GetCommentsByUserIdAsync(int userId)
        {
            try
            {
                var comments = await _context.Comments
                .Where(c => c.UserId == userId)
                .Include(x => x.Event)
                .Include(x => x.User)
                .ToListAsync() ?? throw new NotFoundException("No comment found!");

                var commentDtos = _mapper.Map<IEnumerable<CommentDto>>(comments);

                return commentDtos;
            }
            catch (Exception ex)
            {
                throw new BadRequestException(ex.Message, ex.InnerException);
            }
        }

        /// <summary>
        /// Adds a new comment to the database.
        public async Task<CommentDto> AddCommentAsync(CommentCreateDto commentCreateDto)
        {
            var comment = _mapper.Map<Comment>(commentCreateDto);
            comment.UserId = commentCreateDto.UserId;
            comment.EventId = commentCreateDto.EventId;

            await _context.Comments.AddAsync(comment);
            await _context.SaveChangesAsync();

            var commentDto = _mapper.Map<CommentDto>(comment);

            var savedComment = await _context.Comments
                .Include(c => c.User)
                .FirstOrDefaultAsync(c => c.Id == comment.Id);

            var userObject = new 
            {
                Id = savedComment?.User.Id,
                ProfilePicture = savedComment?.User.ProfilePicture,
                Name = savedComment?.User.Name,
                Role = savedComment?.User.Role,
                UserType = savedComment?.User.UserType,
            };

            await _hubContext.Clients.All.SendAsync("ReceiveComment", new { commentDto, userObject });

            return commentDto;
        }

        /// <summary>
        /// Updates an existing comment in the database.
        public async Task<bool> UpdateCommentAsync(int commentId, int userId, CommentUpdateDto commentUpdateDto)
        {
            var existingComment = await _context.Comments
                .Include(x => x.User)
                .FirstOrDefaultAsync(x => x.Id == commentId);

            if (existingComment == null) return false;

            if (existingComment.UserId != userId)
                throw new BadRequestException("You don't have permission to update the comment");

            _mapper.Map(commentUpdateDto, existingComment);

            await _context.SaveChangesAsync();

            var updatedComment = await _context.Comments
                .FindAsync(commentId);

            await _hubContext.Clients.All.SendAsync("ReceiveCommentForUpdate", new {updatedComment?.Id , updatedComment?.EventId, updatedComment?.CommentContent});

            return true;
        }

        /// <summary>
        /// Deletes a comment from the database.
        public async Task<bool> DeleteCommentAsync(int id, int userId)
        {
            var comment = await _context.Comments
                .Include(x => x.User)
                .FirstOrDefaultAsync(x => x.Id == id);

            if (comment == null) return false;

            if (comment.UserId != userId)
                throw new BadRequestException("You don't have permission to delete the comment");

            _context.Comments.Remove(comment);
            await _context.SaveChangesAsync();

            await _hubContext.Clients.All.SendAsync("ReceiveCommentIdForDeletion", id);

            return true;
        }
    }
}
