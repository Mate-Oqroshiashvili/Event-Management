﻿using AutoMapper;
using Event_Management.Data;
using Event_Management.Exceptions;
using Event_Management.Models;
using Event_Management.Models.Dtos.CommentDtos;
using Event_Management.Models.Dtos.ReviewDtos;
using Event_Management.Web_Sockets;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.ComponentModel.Design;

namespace Event_Management.Repositories.ReviewRepositoryFolder
{
    public class ReviewRepository : IReviewRepository
    {
        private readonly DataContext _context; // Database context for accessing the database
        private readonly IHubContext<ReviewHub> _hubContext; // Hub context for sending messages to clients
        private readonly IMapper _mapper; // AutoMapper for mapping between DTOs and entities

        public ReviewRepository(DataContext context, IHubContext<ReviewHub> hubContext, IMapper mapper)
        {
            _context = context;
            _hubContext = hubContext;
            _mapper = mapper;
        }

        /// <summary>
        /// Retrieves all reviews from the database.
        public async Task<IEnumerable<ReviewDto>> GetReviewsAsync()
        {
            try
            {
                var reviews = await _context.Reviews
                    .Include(x => x.Event)
                    .Include(x => x.User)
                    .ToListAsync() ?? throw new NotFoundException("No review found!");

                var reviewDtos = _mapper.Map<IEnumerable<ReviewDto>>(reviews);

                return reviewDtos;
            }
            catch (Exception ex) 
            {
                throw new BadRequestException(ex.Message, ex.InnerException);
            }
        }

        /// <summary>
        /// Retrieves a review by its ID.
        public async Task<ReviewDto> GetReviewByIdAsync(int id)
        {
            try
            {
                var review = await _context.Reviews
                    .Include(x => x.Event)
                    .Include(x => x.User)
                    .FirstOrDefaultAsync(x => x.Id == id)
                    ?? throw new NotFoundException("Review not found!");

                var reviewDto = _mapper.Map<ReviewDto>(review);

                return reviewDto;
            }
            catch (Exception ex)
            {
                throw new BadRequestException(ex.Message, ex.InnerException);
            }
        }

        /// <summary>
        /// Retrieves reviews associated with a specific event ID.
        public async Task<IEnumerable<ReviewDto>> GetReviewsByEventIdAsync(int eventId)
        {
            try
            {
                var reviews = await _context.Reviews
                    .Where(r => r.EventId == eventId)
                    .Include(x => x.Event)
                    .Include(x => x.User)
                    .ToListAsync() ?? throw new NotFoundException("No review found!");

                var reviewDtos = _mapper.Map<IEnumerable<ReviewDto>>(reviews);

                return reviewDtos;
            }
            catch (Exception ex)
            {
                throw new BadRequestException(ex.Message, ex.InnerException);
            }
        }

        /// <summary>
        /// Retrieves reviews associated with a specific user ID.
        public async Task<IEnumerable<ReviewDto>> GetReviewsByUserIdAsync(int userId)
        {
            try
            {
                var reviews = await _context.Reviews
                .Where(r => r.UserId == userId)
                .Include(x => x.Event)
                .Include(x => x.User)
                .ToListAsync() ?? throw new NotFoundException("No review found!");

                var reviewDtos = _mapper.Map<IEnumerable<ReviewDto>>(reviews);

                return reviewDtos;
            }
            catch (Exception ex)
            {
                throw new BadRequestException(ex.Message, ex.InnerException);
            }
        }

        /// <summary>
        /// Adds a new review to the database.
        public async Task<ReviewDto> AddReviewAsync(ReviewCreateDto reviewCreateDto)
        {
            try
            {
                var @event = await _context.Events
                    .Include(x => x.Reviews)
                    .FirstOrDefaultAsync(x => x.Id == reviewCreateDto.EventId) ?? throw new NotFoundException("Event not found!");

                if (@event.Reviews.Any(x => x.UserId == reviewCreateDto.UserId))
                    throw new BadRequestException("User has already reviewed the event!");

                var review = _mapper.Map<Review>(reviewCreateDto);
                review.UserId = reviewCreateDto.UserId;
                review.EventId = reviewCreateDto.EventId;

                await _context.Reviews.AddAsync(review);
                await _context.SaveChangesAsync();

                var reviewDto = _mapper.Map<ReviewDto>(review);

                var savedReview = await _context.Reviews
                .Include(c => c.User)
                .FirstOrDefaultAsync(c => c.Id == review.Id);

                var userObject = new
                {
                    Id = savedReview?.User.Id,
                    ProfilePicture = savedReview?.User.ProfilePicture,
                    Name = savedReview?.User.Name,
                    Role = savedReview?.User.Role,
                    UserType = savedReview?.User.UserType,
                };

                await _hubContext.Clients.All.SendAsync("ReceiveReview", new { reviewDto, userObject });

                return reviewDto;
            }
            catch (Exception ex) 
            {
                throw new BadRequestException(ex.Message, ex.InnerException);
            }
        }

        /// <summary>
        /// Updates an existing review in the database.
        public async Task<bool> UpdateReviewAsync(int reviewId, int userId, ReviewUpdateDto reviewUpdateDto)
        {
            var existingReview = await _context.Reviews.FindAsync(reviewId);
            if (existingReview == null) return false;

            if(existingReview.UserId != userId) 
                throw new BadRequestException("You don't have permission to update the review!");

            _mapper.Map(reviewUpdateDto, existingReview);

            var updatedReview = await _context.Reviews
                .FindAsync(reviewId);

            await _context.SaveChangesAsync();

            await _hubContext.Clients.All.SendAsync("ReceiveReviewForUpdate", new { updatedReview?.Id, updatedReview?.EventId, updatedReview?.StarCount });

            return true;
        }

        /// <summary>
        /// Deletes a review by its ID.
        public async Task<bool> DeleteReviewAsync(int reviewId, int userId)
        {
            var review = await _context.Reviews.FindAsync(reviewId);
            if (review == null) return false;

            if (review.UserId != userId)
                throw new BadRequestException("You don't have permission to delete the review!");

            _context.Reviews.Remove(review);
            await _context.SaveChangesAsync();

            await _hubContext.Clients.All.SendAsync("ReceiveReviewIdForDeletion", reviewId);

            return true;
        }
    }
}
