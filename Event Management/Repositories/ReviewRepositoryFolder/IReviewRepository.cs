using Event_Management.Models;
using Event_Management.Models.Dtos.ReviewDtos;

namespace Event_Management.Repositories.ReviewRepositoryFolder
{
    public interface IReviewRepository
    {
        Task<IEnumerable<Review>> GetReviewsAsync();
        Task<Review> GetReviewByIdAsync(int id);
        Task<Review> AddReviewAsync(ReviewCreateDto reviewCreateDto);
        Task<bool> UpdateReviewAsync(int id, ReviewUpdateDto reviewUpdateDto);
        Task<bool> DeleteReviewAsync(int id);
    }
}
