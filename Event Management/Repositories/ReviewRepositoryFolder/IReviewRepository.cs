using Event_Management.Models;
using Event_Management.Models.Dtos.ReviewDtos;

namespace Event_Management.Repositories.ReviewRepositoryFolder
{
    public interface IReviewRepository
    {
        Task<IEnumerable<Review>> GetReviewsAsync();
        Task<Review> GetReviewByIdAsync(int id);
        Task<IEnumerable<Review>> GetReviewsByEventIdAsync(int eventId);
        Task<IEnumerable<Review>> GetReviewsByUserIdAsync(int userId);
        Task<Review> AddReviewAsync(ReviewCreateDto reviewCreateDto);
        Task<bool> UpdateReviewAsync(int id, int userId, ReviewUpdateDto reviewUpdateDto);
        Task<bool> DeleteReviewAsync(int id, int userId);
    }
}
