using Event_Management.Models.Dtos.ReviewDtos;

namespace Event_Management.Repositories.ReviewRepositoryFolder
{
    public interface IReviewRepository
    {
        Task<IEnumerable<ReviewDto>> GetReviewsAsync(); // Retrieves all reviews
        Task<ReviewDto> GetReviewByIdAsync(int id); // Retrieves a review by its ID
        Task<IEnumerable<ReviewDto>> GetReviewsByEventIdAsync(int eventId); // Retrieves reviews by event ID
        Task<IEnumerable<ReviewDto>> GetReviewsByUserIdAsync(int userId); // Retrieves reviews by user ID
        Task<ReviewDto> AddReviewAsync(ReviewCreateDto reviewCreateDto); // Adds a new review
        Task<bool> UpdateReviewAsync(int id, int userId, ReviewUpdateDto reviewUpdateDto); // Updates an existing review
        Task<bool> DeleteReviewAsync(int id, int userId); // Deletes a review by its ID
    }
}
