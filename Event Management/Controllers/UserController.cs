using Event_Management.Exceptions;
using Event_Management.Models.Dtos.LoginDtos;
using Event_Management.Models.Dtos.UserDtos;
using Event_Management.Models.Enums;
using Event_Management.Repositories.AuthRepositoryFolder;
using Event_Management.Repositories.CodeRepositoryFolder;
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
        private readonly ICodeRepository _codeRepository;

        public UserController(IUserRepository userRepository, IAuthRepository authRepository, ICodeRepository codeRepository)
        {
            _userRepository = userRepository;
            _authRepository = authRepository;
            _codeRepository = codeRepository;
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

        [HttpGet("get-speakers")]
        public async Task<ActionResult<IEnumerable<UserDto>>> GetSpeakers()
        {
            try
            {
                var speakers = await _userRepository.GetSpeakers();

                return speakers == null ? throw new NotFoundException("Speakers can't be found!") : Ok(new { speakers });
            }
            catch (Exception ex)
            {
                throw new BadRequestException(ex.Message, ex.InnerException);
            }
        }

        [HttpGet("get-artists")]
        public async Task<ActionResult<IEnumerable<UserDto>>> GetArtists()
        {
            try
            {
                var artists = await _userRepository.GetArtists();

                return artists == null ? throw new NotFoundException("Artists can't be found!") : Ok(new { artists });
            }
            catch (Exception ex)
            {
                throw new BadRequestException(ex.Message, ex.InnerException);
            }
        }

        [HttpPost("send-codes")]
        public async Task<ActionResult<string>> SendCodes(string email, string phoneNumber)
        {
            try
            {
                var result = await _codeRepository.SendCodes(email, phoneNumber);

                if (!result)
                    throw new BadRequestException("Verification codes sending process failed!");

                return Ok(new { result, message = "Verification codes sent successfully!" });
            }
            catch (Exception ex)
            {
                throw new BadRequestException(ex.Message, ex.InnerException);
            }
        }

        [HttpPost("register-user")]
        public async Task<ActionResult<UserDto>> RegisterUser([FromBody] UserCreateDto userCreateDto)
        {
            try
            {
                string storedCodes = _codeRepository.GetCodes(userCreateDto.Email);

                if (string.IsNullOrEmpty(storedCodes))
                    throw new BadRequestException("Verification codes expired or never sent!");

                bool isVerified = $"{userCreateDto.EmailCode},{userCreateDto.PhoneNumberCode}" == storedCodes;

                if (!isVerified)
                    throw new BadRequestException("Verification failed!");

                var userDto = await _authRepository.Registration(userCreateDto);

                return userDto == null ? throw new NotFoundException("User registration failed!") : Ok(new { userDto });
            }
            catch (Exception ex)
            {
                throw new BadRequestException(ex.Message, ex.InnerException);
            }
        }

        [HttpPost("login-user")]
        public async Task<ActionResult<string>> LoginUser([FromBody] LoginDto loginDto)
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

        [Authorize(Roles = "BASIC,PARTICIPANT,ORGANIZER")]
        [HttpPatch("add-balance/{userId}")]
        public async Task<ActionResult<string>> AddBalance(int userId, [FromBody] decimal balanceToDeposit)
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

        [HttpPatch("change-login-status/{userId}")]
        public async Task<ActionResult<bool>> ChangeLoginStatus(int userId)
        {
            try
            {
                var updated = await _userRepository.UpdateLoginStatus(userId);

                return !updated ? throw new NotFoundException("User not found!") : Ok(new { updated ,message = "Login status updated successfully." });
            }
            catch (Exception ex)
            {
                throw new BadRequestException(ex.Message, ex.InnerException);
            }
        }

        [Authorize(Roles = "BASIC,ORGANIZER,PARTICIPANT")]
        [HttpPatch("change-user-type/{userId}")]
        public async Task<ActionResult<string>> ChangeUserType(int userId, [FromBody] UserType userType)
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

        [Authorize(Roles = "BASIC,ORGANIZER,PARTICIPANT")]
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

        [Authorize(Roles = "BASIC,ORGANIZER,PARTICIPANT")]
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

        [Authorize(Roles = "BASIC,ORGANIZER,PARTICIPANT")]
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
