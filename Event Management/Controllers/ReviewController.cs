﻿using Event_Management.Exceptions;
using Event_Management.Models.Dtos.ReviewDtos;
using Event_Management.Repositories.ReviewRepositoryFolder;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Event_Management.Controllers
{
    [Authorize] // This attribute ensures that all actions in this controller require authorization.
    [Route("api/[controller]")]
    [ApiController]
    public class ReviewController : ControllerBase
    {
        private readonly IReviewRepository _reviewRepository; // This is the repository that will handle the data access for reviews.

        public ReviewController(IReviewRepository reviewRepository)
        {
            _reviewRepository = reviewRepository;
        }

        [Authorize(Roles = "ORGANIZER")]
        [HttpGet("get-all-reviews")]
        public async Task<ActionResult<IEnumerable<ReviewDto>>> GetAllReviews()
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
        public async Task<ActionResult<ReviewDto>> GetReviewsById(int reviewId)
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
        public async Task<ActionResult<IEnumerable<ReviewDto>>> GetReviewsByEventId(int eventId)
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
        public async Task<ActionResult<IEnumerable<ReviewDto>>> GetReviewsByUserId(int userId)
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
        public async Task<ActionResult<ReviewDto>> AddReview([FromForm] ReviewCreateDto reviewCreateDto)
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
