using Event_Management.Models.Dtos.ReviewDtos;

namespace Event_Management.Repositories.ReviewRepositoryFolder
{
    public interface IReviewRepository
    {
        Task<IEnumerable<ReviewDto>> GetReviewsAsync();
        Task<ReviewDto> GetReviewByIdAsync(int id);
        Task<IEnumerable<ReviewDto>> GetReviewsByEventIdAsync(int eventId);
        Task<IEnumerable<ReviewDto>> GetReviewsByUserIdAsync(int userId);
        Task<ReviewDto> AddReviewAsync(ReviewCreateDto reviewCreateDto);
        Task<bool> UpdateReviewAsync(int id, int userId, ReviewUpdateDto reviewUpdateDto);
        Task<bool> DeleteReviewAsync(int id, int userId);
    }
}
