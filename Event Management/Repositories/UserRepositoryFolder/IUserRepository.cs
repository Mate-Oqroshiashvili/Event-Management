using Event_Management.Models;
using Event_Management.Models.Dtos.UserDtos;
using Event_Management.Models.Enums;

namespace Event_Management.Repositories.UserRepositoryFolder
{
    public interface IUserRepository
    {
        Task<IEnumerable<UserDto>> GetUsersAsync();
        Task<UserDto> GetUserByIdAsync(int id);
        Task<UserDto> GetUserByEmailAsync(string email);
        Task<IEnumerable<UserDto>> GetArtists();
        Task<IEnumerable<UserDto>> GetSpeakers();
        bool ValidatePassword(string passwordToVerify, string password);
        Task<UserDto> AddUserAsync(User user);
        Task<bool> UpdateUserAsync(int id, UserUpdateDto userUpdateDto);
        Task<bool> UpdateUserTypeAsync(int id, UserType userType);
        Task<bool> UpdateUserPasswordAsync(int id, string password);
        Task<bool> DeleteUserAsync(int id);
    }
}
