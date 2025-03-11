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

        private readonly DataContext _context;
        private readonly IMapper _mapper;
        private readonly ITokenRepository _tokenRepository;
        private readonly IUserRepository _userRepository;
        public AuthRepository(DataContext context, IMapper mapper, ITokenRepository tokenRepository, IUserRepository user)
        {
            _context = context;
            _mapper = mapper;
            _tokenRepository = tokenRepository;
            _userRepository = user;
        }

        public async Task<User> Registration(UserCreateDto registerUserDto)
        {
            var existingUser = await _userRepository.GetUserByEmailAsync(registerUserDto.Email);
            if (existingUser != null)
            {
                throw new InvalidOperationException("A user with this email already exists.");
            }

            var user = _mapper.Map<User>(registerUserDto);

            var passwordHash = BCrypt.Net.BCrypt.HashPassword(registerUserDto.Password);
            user.Role = Role.BASIC;
            user.PasswordHash = passwordHash;

            await _userRepository.AddUserAsync(user);

            return user;
        }

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
