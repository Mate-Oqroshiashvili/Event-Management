using Event_Management.Models.Dtos.LoginDtos;
using Event_Management.Models.Dtos.UserDtos;

namespace Event_Management.Repositories.AuthRepositoryFolder
{
    public interface IAuthRepository
    {
        Task<UserDto> Registration(UserCreateDto registerUserDto);
        Task<string> Authorization(LoginDto logInDto);
    }
}
