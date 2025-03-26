using Event_Management.Exceptions;
using Event_Management.Models;
using Event_Management.Models.Dtos.ReviewDtos;
using Event_Management.Repositories.ReviewRepositoryFolder;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Event_Management.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReviewController : ControllerBase
    {
        private readonly IReviewRepository _reviewRepository;

        public ReviewController(IReviewRepository reviewRepository)
        {
            _reviewRepository = reviewRepository;
        }

        [Authorize(Roles = "ADMINISTRATOR")]
        [HttpGet("get-all-reviews")]
        public async Task<ActionResult<IEnumerable<Review>>> GetAllReviews()
        {
            try
            {
                var reviews = await _reviewRepository.GetReviewsAsync();

                return reviews == null ? throw new NotFoundException("Reviews not found!") : Ok(new { reviews });
            }
            catch (Exception ex)
            {
                throw new BadRequestException(ex.Message, ex.InnerException);
            }
        }

        [HttpGet("get-review-by-id/{reviewId}")]
        public async Task<ActionResult<Review>> GetReviewsById(int reviewId)
        {
            try
            {
                var review = await _reviewRepository.GetReviewByIdAsync(reviewId);

                return review == null ? throw new NotFoundException("Review not found!") : Ok(new { review });
            }
            catch (Exception ex)
            {
                throw new BadRequestException(ex.Message, ex.InnerException);
            }
        }

        [HttpGet("get-reviews-by-event-id/{eventId}")]
        public async Task<ActionResult<IEnumerable<Review>>> GetReviewsByEventId(int eventId)
        {
            try
            {
                var reviews = await _reviewRepository.GetReviewsByEventIdAsync(eventId);

                return reviews == null ? throw new NotFoundException("Reviews not found!") : Ok(new { reviews });
            }
            catch (Exception ex)
            {
                throw new BadRequestException(ex.Message, ex.InnerException);
            }
        }

        [HttpGet("get-reviews-by-user-id/{userId}")]
        public async Task<ActionResult<IEnumerable<Review>>> GetReviewsByUserId(int userId)
        {
            try
            {
                var reviews = await _reviewRepository.GetReviewsByUserIdAsync(userId);

                return reviews == null ? throw new NotFoundException("reviews not found!") : Ok(new { reviews });
            }
            catch (Exception ex)
            {
                throw new BadRequestException(ex.Message, ex.InnerException);
            }
        }

        [HttpPost("add-review")]
        public async Task<ActionResult<Review>> AddReview([FromForm] ReviewCreateDto reviewCreateDto)
        {
            try
            {
                var review = await _reviewRepository.AddReviewAsync(reviewCreateDto);

                return review == null ? throw new NotFoundException("Review creation process failed!") : Ok(new { review });
            }
            catch (Exception ex)
            {
                throw new BadRequestException(ex.Message, ex.InnerException);
            }
        }

        [HttpPut("update-review/{reviewId}&{userId}")]
        public async Task<ActionResult<string>> UpdateReview(int reviewId, int userId, [FromForm] ReviewUpdateDto reviewUpdateDto)
        {
            try
            {
                var isUpdated = await _reviewRepository.UpdateReviewAsync(reviewId, userId, reviewUpdateDto);

                return !isUpdated ? throw new NotFoundException("Review update process failed!") : Ok(new { message = "Review updated successfully!" });
            }
            catch (Exception ex)
            {
                throw new BadRequestException(ex.Message, ex.InnerException);
            }
        }

        [HttpDelete("remove-review/{reviewId}&{userId}")]
        public async Task<ActionResult<string>> RemoveReview(int reviewId, int userId)
        {
            try
            {
                var isRemoved = await _reviewRepository.DeleteReviewAsync(reviewId, userId);

                return !isRemoved ? throw new NotFoundException("Review deletion process failed!") : Ok(new { message = "Review removed successfully!" });
            }
            catch (Exception ex)
            {
                throw new BadRequestException(ex.Message, ex.InnerException);
            }
        }
    }
}
