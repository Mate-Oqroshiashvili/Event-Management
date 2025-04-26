using Event_Management.Models;
using Event_Management.Models.Dtos.UserDtos;
using Event_Management.Models.Enums;

namespace Event_Management.Repositories.UserRepositoryFolder
{
    public interface IUserRepository
    {
        Task<UserAnalyticsDto> GetUserAnalyticsAsync(int userId); // Retrieves user analytics
        Task<AdminAnalyticsDto> GetAdminAnalyticsAsync(); // Retrieves admin analytics
        Task<IEnumerable<UserDto>> GetUsersAsync(); // Retrieves all users
        Task<UserDto> GetUserByIdAsync(int id); // Retrieves a user by its ID
        Task<UserDto> GetUserByEmailAsync(string email); // Retrieves a user by their email
        Task<IEnumerable<UserDto>> GetArtistsAsync(); // Retrieves all artists
        Task<IEnumerable<UserDto>> GetSpeakersAsync(); // Retrieves all speakers
        bool ValidatePassword(string passwordToVerify, string password); // Validates a password
        Task<UserDto> AddUserAsync(User user); // Adds a new user
        Task<decimal> AddBalanceAsync(int userId, decimal balanceToDeposit); // Adds balance to a user's account
        Task<bool> UpdateUserAsync(int id, UserUpdateDto userUpdateDto); // Updates an existing user
        Task<bool> UpdateLoginStatusAsync(int id); // Updates a user's login status
        Task<bool> UpdateUserTypeAsync(int id, UserType userType); // Updates a user's type
        Task<bool> UpdateUserPasswordAsync(int id, string password); // Updates a user's password
        Task<bool> DeleteUserAsync(int id); // Deletes a user by their ID
    }
}
