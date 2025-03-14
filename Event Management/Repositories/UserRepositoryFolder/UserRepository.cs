using AutoMapper;
using Event_Management.Data;
using Event_Management.Models;
using Event_Management.Models.Dtos.UserDtos;
using Microsoft.EntityFrameworkCore;

namespace Event_Management.Repositories.UserRepositoryFolder
{
    public class UserRepository : IUserRepository
    {
        private readonly DataContext _context;
        private readonly IMapper _mapper;

        public UserRepository(DataContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<IEnumerable<UserDto>> GetUsersAsync()
        {
            var users = await _context.Users
                .Include(x => x.Participants)
                .Include(x => x.Purchases)
                .Include(x => x.Organizer)
                .Include(x => x.Tickets)
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
                .Where(x => x.UserType == Models.Enums.UserType.Speaker)
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
                .Where(x => x.UserType == Models.Enums.UserType.Artist)
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

        public async Task<bool> UpdateUserAsync(int id, UserUpdateDto userUpdateDto)
        {
            var existingUser = await _context.Users.FindAsync(id);
            if (existingUser == null) return false;

            var user = _mapper.Map(userUpdateDto, existingUser);
            _context.Users.Update(user);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteUserAsync(int id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null) return false;

            _context.Users.Remove(user);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
