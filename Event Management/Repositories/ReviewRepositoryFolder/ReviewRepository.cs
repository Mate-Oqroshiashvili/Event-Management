using AutoMapper;
using Event_Management.Data;
using Event_Management.Exceptions;
using Event_Management.Models;
using Event_Management.Models.Dtos.ReviewDtos;
using Microsoft.EntityFrameworkCore;

namespace Event_Management.Repositories.ReviewRepositoryFolder
{
    public class ReviewRepository : IReviewRepository
    {
        private readonly DataContext _context;
        private readonly IMapper _mapper;

        public ReviewRepository(DataContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<IEnumerable<Review>> GetReviewsAsync()
        {
            return await _context.Reviews
                .Include(x => x.Event)
                .Include(x => x.User)
                .ToListAsync();
        }

        public async Task<Review> GetReviewByIdAsync(int id)
        {
            return await _context.Reviews
                .Include(x => x.Event)
                .Include(x => x.User)
                .FirstOrDefaultAsync(x => x.Id == id)
                ?? throw new NotFoundException("Review not found!");
        }

        public async Task<IEnumerable<Review>> GetReviewsByEventIdAsync(int eventId)
        {
            return await _context.Reviews
                .Where(r => r.EventId == eventId)
                .Include(x => x.Event)
                .Include(x => x.User)
                .ToListAsync();
        }

        public async Task<IEnumerable<Review>> GetReviewsByUserIdAsync(int userId)
        {
            return await _context.Reviews
                .Where(r => r.UserId == userId)
                .Include(x => x.Event)
                .Include(x => x.User)
                .ToListAsync();
        }

        public async Task<Review> AddReviewAsync(ReviewCreateDto reviewCreateDto)
        {
            var review = _mapper.Map<Review>(reviewCreateDto);
            review.UserId = reviewCreateDto.UserId;
            review.EventId = reviewCreateDto.EventId;

            await _context.Reviews.AddAsync(review);
            await _context.SaveChangesAsync();

            return review;
        }

        public async Task<bool> UpdateReviewAsync(int reviewId, int userId, ReviewUpdateDto reviewUpdateDto)
        {
            var existingReview = await _context.Reviews.FindAsync(reviewId);
            if (existingReview == null) return false;

            if(existingReview.UserId == userId) 
                throw new BadRequestException("You don't have permission to update the review!");

            _mapper.Map(reviewUpdateDto, existingReview);

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteReviewAsync(int reviewId, int userId)
        {
            var review = await _context.Reviews.FindAsync(reviewId);
            if (review == null) return false;

            if (review.UserId == userId)
                throw new BadRequestException("You don't have permission to delete the review!");

            _context.Reviews.Remove(review);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
