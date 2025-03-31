using AutoMapper;
using Event_Management.Data;
using Event_Management.Exceptions;
using Event_Management.Models;
using Event_Management.Models.Dtos.UserDtos;
using Event_Management.Models.Enums;
using Event_Management.Repositories.ImageRepositoryFolder;
using Microsoft.EntityFrameworkCore;

namespace Event_Management.Repositories.UserRepositoryFolder
{
    public class UserRepository : IUserRepository
    {
        private readonly DataContext _context;
        private readonly IImageRepository _imageRepository;
        private readonly IMapper _mapper;

        public UserRepository(DataContext context, IImageRepository imageRepository, IMapper mapper)
        {
            _context = context;
            _imageRepository = imageRepository;
            _mapper = mapper;
        }

        public async Task<IEnumerable<UserDto>> GetUsersAsync()
        {
            var users = await _context.Users
                .Include(x => x.Participants)
                .Include(x => x.Purchases)
                .Include(x => x.Organizer)
                .Include(x => x.Tickets)
                .Include(x => x.Comments)
                .Include(x => x.Reviews)
                .Include(x => x.UsedPromoCodes)
                .ToListAsync();

            var userDtos = _mapper.Map<IEnumerable<UserDto>>(users);

            return userDtos;
        }

        public async Task<UserDto> GetUserByIdAsync(int id)
        {
            var user = await _context.Users
                .Include(x => x.Participants)
                .Include(x => x.Purchases)
                .Include(x => x.Organizer)
                .Include(x => x.Tickets)
                .Include(x => x.Comments)
                .Include(x => x.Reviews)
                .Include(x => x.UsedPromoCodes)
                .FirstOrDefaultAsync(x => x.Id == id);

            var userDto = _mapper.Map<UserDto>(user);

            return userDto;
        }

        public async Task<UserDto> GetUserByEmailAsync(string email)
        {
            var user = await _context.Users
                .Include(x => x.Participants)
                .Include(x => x.Purchases)
                .Include(x => x.Organizer)
                .Include(x => x.Tickets)
                .Include(x => x.Comments)
                .Include(x => x.Reviews)
                .Include(x => x.UsedPromoCodes)
                .FirstOrDefaultAsync(x => x.Email == email);

            var userDto = _mapper.Map<UserDto>(user);

            return userDto;
        }

        public async Task<IEnumerable<UserDto>> GetSpeakers()
        {
            var speakers = await _context.Users
                .Include(x => x.Participants)
                .Include(x => x.Purchases)
                .Include(x => x.Organizer)
                .Include(x => x.Tickets)
                .Include(x => x.Comments)
                .Include(x => x.Reviews)
                .Include(x => x.UsedPromoCodes)
                .Where(x => x.UserType == UserType.SPEAKER)
                .ToListAsync();

            var speakerDtos = _mapper.Map<IEnumerable<UserDto>>(speakers);

            return speakerDtos;
        }

        public async Task<IEnumerable<UserDto>> GetArtists()
        {
            var artists = await _context.Users
                .Include(x => x.Participants)
                .Include(x => x.Purchases)
                .Include(x => x.Organizer)
                .Include(x => x.Tickets)
                .Include(x => x.Comments)
                .Include(x => x.Reviews)
                .Include(x => x.UsedPromoCodes)
                .Where(x => x.UserType == UserType.ARTIST)
                .ToListAsync();

            var artistDtos = _mapper.Map<IEnumerable<UserDto>>(artists);

            return artistDtos;
        }

        public bool ValidatePassword(string passwordToVerify, string password)
        {
            if (passwordToVerify == null)
                return false;

            return BCrypt.Net.BCrypt.Verify(password, passwordToVerify);
        }

        public async Task<UserDto> AddUserAsync(User user)
        {
            await _context.Users.AddAsync(user);
            await _context.SaveChangesAsync();

            var userDto = _mapper.Map<UserDto>(user);

            return userDto;
        }

        public async Task<decimal> AddBalanceAsync(int userId, decimal balanceToDeposit)
        {
            var user = await _context.Users
                .FirstOrDefaultAsync(x => x.Id == userId)
                ?? throw new NotFoundException("User not found!");

            user.Balance += balanceToDeposit;

            await _context.SaveChangesAsync();

            return user.Balance;
        }

        public async Task<bool> UpdateUserAsync(int id, UserUpdateDto userUpdateDto)
        {
            var existingUser = await _context.Users.FindAsync(id);
            if (existingUser == null) return false;

            _mapper.Map(userUpdateDto, existingUser);

            if (userUpdateDto.ProfilePicture != null) 
            {
                var imageSource = await _imageRepository.GenerateImageSource(userUpdateDto.ProfilePicture);
                existingUser.ProfilePicture = imageSource;
            }

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> UpdateLoginStatus(int id)
        {
            var existingUser = await _context.Users.FindAsync(id);
            if (existingUser == null) return false;

            existingUser.IsLoggedIn = false;

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> UpdateUserTypeAsync(int id, UserType userType)
        {
            var existingUser = await _context.Users.FindAsync(id);
            if (existingUser == null) return false;

            existingUser.UserType = userType;

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> UpdateUserPasswordAsync(int id, string password)
        {
            var existingUser = await _context.Users.FindAsync(id);
            if (existingUser == null) return false;

            existingUser.PasswordHash = BCrypt.Net.BCrypt.HashPassword(password);

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteUserAsync(int id)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                var user = await _context.Users
                    .Include(u => u.Purchases)
                    .Include(u => u.Participants)
                    .FirstOrDefaultAsync(u => u.Id == id);

                if (user == null) return false;
                if (user.Role == Role.ADMINISTRATOR) return false; // Prevent admin deletion

                // Log or process related entities before deletion
                if (user.Purchases.Any())
                {
                    Console.WriteLine($"Deleting user {id}: Removing {user.Purchases.Count()} purchases.");
                }
                if (user.Participants.Any())
                {
                    Console.WriteLine($"Deleting user {id}: Removing {user.Participants.Count()} participant records.");
                }

                // Remove user (cascades to Purchases & Participants)
                _context.Users.Remove(user);
                await _context.SaveChangesAsync();

                // Commit transaction
                await transaction.CommitAsync();
                return true;
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                Console.WriteLine($"Error deleting user {id}: {ex.Message}");
                return false;
            }
        }
    }
}
