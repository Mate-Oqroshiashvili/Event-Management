using Event_Management.Models;
using Event_Management.Models.Dtos.UserDtos;
using Event_Management.Models.Enums;

namespace Event_Management.Repositories.UserRepositoryFolder
{
    public interface IUserRepository
    {
        Task<UserAnalyticsDto> GetUserAnalyticsAsync(int userId);
        Task<AdminAnalyticsDto> GetAdminAnalyticsAsync();
        Task<IEnumerable<UserDto>> GetUsersAsync();
        Task<UserDto> GetUserByIdAsync(int id);
        Task<UserDto> GetUserByEmailAsync(string email);
        Task<IEnumerable<UserDto>> GetArtistsAsync();
        Task<IEnumerable<UserDto>> GetSpeakersAsync();
        bool ValidatePassword(string passwordToVerify, string password);
        Task<UserDto> AddUserAsync(User user);
        Task<decimal> AddBalanceAsync(int userId, decimal balanceToDeposit);
        Task<bool> UpdateUserAsync(int id, UserUpdateDto userUpdateDto);
        Task<bool> UpdateLoginStatusAsync(int id);
        Task<bool> UpdateUserTypeAsync(int id, UserType userType);
        Task<bool> UpdateUserPasswordAsync(int id, string password);
        Task<bool> DeleteUserAsync(int id);
    }
}
