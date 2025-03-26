using Event_Management.Exceptions;
using Event_Management.Models.Dtos.LoginDtos;
using Event_Management.Models.Dtos.UserDtos;
using Event_Management.Models.Enums;
using Event_Management.Repositories.AuthRepositoryFolder;
using Event_Management.Repositories.UserRepositoryFolder;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Event_Management.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserRepository _userRepository;
        private readonly IAuthRepository _authRepository;

        public UserController(IUserRepository userRepository, IAuthRepository authRepository)
        {
            _userRepository = userRepository;
            _authRepository = authRepository;
        }

        [Authorize(Roles = "ADMINISTRATOR")]
        [HttpGet("get-all-users")]
        public async Task<ActionResult<IEnumerable<UserDto>>> GetAllUsers()
        {
            try
            {
                var userDtos = await _userRepository.GetUsersAsync();

                return userDtos == null ? throw new NotFoundException("Users can't be found!") : Ok(new { userDtos });
            }
            catch (Exception ex)
            {
                throw new BadRequestException(ex.Message, ex.InnerException);
            }
        }

        [HttpGet("get-user-by-id/{userId}")]
        public async Task<ActionResult<UserDto>> GetUserById(int userId)
        {
            try
            {
                var userDto = await _userRepository.GetUserByIdAsync(userId);

                return userDto == null ? throw new NotFoundException("User can't be found!") : Ok(new { userDto });
            }
            catch (Exception ex)
            {
                throw new BadRequestException(ex.Message, ex.InnerException);
            }
        }

        [HttpGet("get-user-by-email/{emailAddress}")]
        public async Task<ActionResult<UserDto>> GetUserByEmail(string emailAddress)
        {
            try
            {
                var userDto = await _userRepository.GetUserByEmailAsync(emailAddress);

                return userDto == null ? throw new NotFoundException("User can't be found!") : Ok(new { userDto });
            }
            catch (Exception ex)
            {
                throw new BadRequestException(ex.Message, ex.InnerException);
            }
        }

        [HttpPost("register-user")]
        public async Task<ActionResult<UserDto>> RegisterUser([FromForm] UserCreateDto userCreateDto)
        {
            try
            {
                var userDto = await _authRepository.Registration(userCreateDto);

                return userDto == null ? throw new NotFoundException("User registration failed!") : Ok(new { userDto });
            }
            catch (Exception ex)
            {
                throw new BadRequestException(ex.Message, ex.InnerException);
            }
        }

        [HttpPost("login-user")]
        public async Task<ActionResult<string>> LoginUser([FromForm] LoginDto loginDto)
        {
            try
            {
                var token = await _authRepository.Authorization(loginDto);

                return token == null ? throw new BadRequestException("Token not generated succesfully") : Ok(new { token });
            }
            catch (Exception ex)
            {
                throw new BadRequestException(ex.Message, ex.InnerException);
            }
        }

        [Authorize(Roles = "BASIC,ORGANIZER")]
        [HttpPatch("add-balance/{userId}")]
        public async Task<ActionResult<string>> AddBalance(int userId, decimal balanceToDeposit)
        {
            try
            {
                var totalBalance = await _userRepository.AddBalanceAsync(userId, balanceToDeposit);

                if (totalBalance <= 0)
                {
                    return BadRequest(new { message = "Something went wrong while depositing the balance!" });
                }

                return Ok(new { message = "Balance updated successfully.", newBalance = totalBalance });
            }
            catch (Exception ex)
            {
                throw new BadRequestException(ex.Message, ex.InnerException);
            }
        }

        [Authorize(Roles = "BASIC,ORGANIZER")]
        [HttpPatch("change-user-type/{userId}")]
        public async Task<ActionResult<string>> ChangeUserType(int userId, UserType userType)
        {
            try
            {
                var updated = await _userRepository.UpdateUserTypeAsync(userId, userType);

                return !updated ? throw new NotFoundException("User not found!") : Ok(new { message = "UserType updated successfully." });
            }
            catch (Exception ex)
            {
                throw new BadRequestException(ex.Message, ex.InnerException);
            }
        }

        [Authorize(Roles = "BASIC,ORGANIZER")]
        [HttpPatch("change-user-password/{userId}")]
        public async Task<ActionResult<string>> ChangeUserPassword(int userId, [FromForm] ChangePasswordDto changePasswordDto)
        {
            try
            {
                var updated = await _userRepository.UpdateUserPasswordAsync(userId, changePasswordDto.Password);

                return !updated ? throw new NotFoundException("User not found!") : Ok(new { message = "Password updated successfully." });
            }
            catch (Exception ex)
            {
                throw new BadRequestException(ex.Message, ex.InnerException);
            }
        }

        [Authorize(Roles = "BASIC,ORGANIZER")]
        [HttpPut("update-user-information/{userId}")]
        public async Task<ActionResult<bool>> UpdateUserInformation(int userId, [FromForm] UserUpdateDto userUpdateDto)
        {
            try
            {
                if (userUpdateDto == null)
                    throw new BadRequestException("No user data provided");

                bool updated = await _userRepository.UpdateUserAsync(userId, userUpdateDto);

                return !updated ? throw new NotFoundException("User not found!") : Ok(new { message = "User information updated successfully!" });
            }
            catch (Exception ex)
            {
                throw new BadRequestException(ex.Message, ex.InnerException);
            }
        }

        [Authorize(Roles = "BASIC,ORGANIZER")]
        [HttpDelete("remove-user/{userId}")]
        public async Task<ActionResult<string>> RemoveUser(int userId)
        {
            try
            {
                bool isDeleted = await _userRepository.DeleteUserAsync(userId);

                return !isDeleted ? throw new NotFoundException("User not found!") : Ok(new { message = "User account removed successfully!" });
            }
            catch (Exception ex)
            {
                throw new BadRequestException(ex.Message, ex.InnerException);
            }
        }
    }
}
