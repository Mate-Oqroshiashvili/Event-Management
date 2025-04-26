using AutoMapper;
using Event_Management.Data;
using Event_Management.Models;
using Event_Management.Models.Dtos.LoginDtos;
using Event_Management.Models.Dtos.UserDtos;
using Event_Management.Models.Enums;
using Event_Management.Repositories.JwtRepositoryFolder;
using Event_Management.Repositories.UserRepositoryFolder;
using Microsoft.EntityFrameworkCore;

namespace Event_Management.Repositories.AuthRepositoryFolder
{
    public class AuthRepository : IAuthRepository
    {

        private readonly DataContext _context; // Database context for accessing the database
        private readonly IMapper _mapper; // AutoMapper instance for mapping between DTOs and entities
        private readonly ITokenRepository _tokenRepository; // Token repository for generating JWT tokens
        private readonly IUserRepository _userRepository; // User repository for accessing user-related data

        public AuthRepository(DataContext context, IMapper mapper, ITokenRepository tokenRepository, IUserRepository user)
        {
            _context = context;
            _mapper = mapper;
            _tokenRepository = tokenRepository;
            _userRepository = user;
        }

        /// <summary>
        /// Registers a new user in the system.
        public async Task<UserDto> Registration(UserCreateDto registerUserDto)
        {
            var existingUser = await _userRepository.GetUserByEmailAsync(registerUserDto.Email);
            if (existingUser != null)
            {
                throw new InvalidOperationException("A user with this email already exists.");
            }

            var user = _mapper.Map<User>(registerUserDto);

            var passwordHash = BCrypt.Net.BCrypt.HashPassword(registerUserDto.Password);
            user.Role = Role.BASIC;
            user.UserType = UserType.BASIC;
            user.PasswordHash = passwordHash;
            user.PromoCodeIsClaimable = true;
            user.LastPromoClaimedAt = null;

            await _userRepository.AddUserAsync(user);

            var userDto = _mapper.Map<UserDto>(user);

            return userDto;
        }

        /// <summary>
        /// Authorizes a user by validating their credentials and generating a JWT token.
        public async Task<string> Authorization(LoginDto logInDto)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == logInDto.Email) ?? throw new UnauthorizedAccessException("User not found");
            bool isCorrectPassword = BCrypt.Net.BCrypt.Verify(logInDto.Password, user.PasswordHash);
            if (!isCorrectPassword)
                throw new UnauthorizedAccessException("password is not correct");

            user.IsLoggedIn = true;
            await _context.SaveChangesAsync();

            string token = _tokenRepository.GenerateToken(user);
            if (string.IsNullOrEmpty(token))
                throw new Exception("token not generated");

            return token;
        }
    }
}
