using AutoMapper;
using Event_Management.Data;
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
                .FirstOrDefaultAsync(x => x.Id == id);
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

        public async Task<bool> UpdateReviewAsync(int id, ReviewUpdateDto reviewUpdateDto)
        {
            var existingReview = await _context.Reviews.FindAsync(id);
            if (existingReview == null) return false;

            _mapper.Map(reviewUpdateDto, existingReview);

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteReviewAsync(int id)
        {
            var review = await _context.Reviews.FindAsync(id);
            if (review == null) return false;

            _context.Reviews.Remove(review);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
