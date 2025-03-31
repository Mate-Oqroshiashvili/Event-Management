using AutoMapper;
using Event_Management.Data;
using Event_Management.Exceptions;
using Event_Management.Models;
using Event_Management.Models.Dtos.ReviewDtos;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

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

        public async Task<ReviewDto> AddReviewAsync(ReviewCreateDto reviewCreateDto)
        {
            try
            {
                var review = _mapper.Map<Review>(reviewCreateDto);
                review.UserId = reviewCreateDto.UserId;
                review.EventId = reviewCreateDto.EventId;

                await _context.Reviews.AddAsync(review);
                await _context.SaveChangesAsync();

                var reviewDto = _mapper.Map<ReviewDto>(review);

                return reviewDto;
            }
            catch (Exception ex) 
            {
                throw new BadRequestException(ex.Message, ex.InnerException);
            }
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
