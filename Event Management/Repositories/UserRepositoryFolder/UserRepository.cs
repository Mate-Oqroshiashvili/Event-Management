﻿using AutoMapper;
using Event_Management.Data;
using Event_Management.Exceptions;
using Event_Management.Models;
using Event_Management.Models.Dtos.UserDtos;
using Event_Management.Models.Enums;
using Event_Management.Repositories.ImageRepositoryFolder;
using Event_Management.Web_Sockets;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;

namespace Event_Management.Repositories.UserRepositoryFolder
{
    public class UserRepository : IUserRepository
    {
        private readonly DataContext _context; // Database context for accessing the database
        private readonly IImageRepository _imageRepository; // Image repository for handling image-related operations
        private readonly IHubContext<UserHub> _hubContext; // Hub context for sending messages to clients
        private readonly IMapper _mapper; // AutoMapper for mapping between DTOs and entities

        public UserRepository(DataContext context, IImageRepository imageRepository, IHubContext<UserHub> hubContext, IMapper mapper)
        {
            _context = context;
            _imageRepository = imageRepository;
            _hubContext = hubContext;
            _mapper = mapper;
        }

        /// <summary>
        /// Retrieves user analytics for a specific user.
        public async Task<UserAnalyticsDto> GetUserAnalyticsAsync(int userId)
        {
            var user = await _context.Users
                .Include(u => u.Purchases)
                    .ThenInclude(x => x.Participants)
                .Include(u => u.Tickets)
                    .ThenInclude(u => u.Users)
                .Include(u => u.Comments)
                .Include(u => u.Reviews)
                .Include(u => u.UsedPromoCodes)
                .Include(u => u.Participants)
                .Include(u => u.Organizer)
                .FirstOrDefaultAsync(u => u.Id == userId)
                ?? throw new NotFoundException("User not found!");

            var totalSpent = user.Purchases
                .Where(p => p.Status == PurchaseStatus.COMPLETED)
                .Sum(p => p.TotalAmount);

            var totalTickets = user.Purchases
                .Where(p => p.Status == PurchaseStatus.COMPLETED)
                .Sum(x => x.Participants.Count());

            var eventsWithAttendance = await _context.Tickets
                .Where(t => t.Participants.Any(p => p.UserId == userId && p.Attendance == true))
                .Select(t => t.EventId)
                .Distinct()
                .CountAsync();

            var artistOrSpeakerEvents = await _context.Events
                .CountAsync(e => e.SpeakersAndArtists.Any(u => u.Id == userId));

            var dto = new UserAnalyticsDto
            {
                TotalBalance = user.Balance,
                TotalSpent = totalSpent,
                TotalPurchases = user.Purchases.Count(),
                TotalTicketsBought = totalTickets,
                EventsParticipatedIn = eventsWithAttendance,
                EventsAsArtistOrSpeaker = artistOrSpeakerEvents,
                TotalComments = user.Comments.Count(),
                TotalReviews = user.Reviews.Count(),
                UsedPromoCodesCount = user.UsedPromoCodes.Count()
            };

            return dto;
        }

        /// <summary>
        /// Retrieves admin analytics for the entire system.
        public async Task<AdminAnalyticsDto> GetAdminAnalyticsAsync()
        {
            var dto = new AdminAnalyticsDto
            {
                TotalUsers = await _context.Users.CountAsync(),
                TotalOrganizers = await _context.Users.CountAsync(u => u.Role == Role.ORGANIZER),
                TotalParticipants = await _context.Users.CountAsync(u => u.Role == Role.PARTICIPANT),
                TotalArtists = await _context.Users.CountAsync(u => u.UserType == UserType.ARTIST),
                TotalSpeakers = await _context.Users.CountAsync(u => u.UserType == UserType.SPEAKER),

                TotalEvents = await _context.Events.CountAsync(),
                DraftedEvents = await _context.Events.CountAsync(e => e.Status == EventStatus.DRAFT),
                PublishedEvents = await _context.Events.CountAsync(e => e.Status == EventStatus.PUBLISHED),
                CompletedEvents = await _context.Events.CountAsync(e => e.Status == EventStatus.COMPLETED),
                DeletedEvents = await _context.Events.CountAsync(e => e.Status == EventStatus.DELETED),

                TotalRevenue = await _context.Purchases
                    .Where(p => p.Status == PurchaseStatus.COMPLETED)
                    .SumAsync(p => (decimal?)p.TotalAmount) ?? 0,

                TotalUserBalances = await _context.Users.SumAsync(u => (decimal?)u.Balance) ?? 0,

                TotalComments = await _context.Comments.CountAsync(),
                TotalReviews = await _context.Reviews.CountAsync(),
                TotalPromoCodes = await _context.PromoCodes.CountAsync()
            };

            return dto;
        }

        /// <summary>
        /// Retrieves all users from the database.
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

        /// <summary>
        /// Retrieves a user by its ID.
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

        /// <summary>
        /// Retrieves a user by their email address.
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

        /// <summary>
        /// Retrieves all speakers from the database.
        public async Task<IEnumerable<UserDto>> GetSpeakersAsync()
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

        /// <summary>
        /// Retrieves all artists from the database.
        public async Task<IEnumerable<UserDto>> GetArtistsAsync()
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

        /// <summary>
        /// Validates a password against a hashed password.
        public bool ValidatePassword(string passwordToVerify, string password)
        {
            if (passwordToVerify == null)
                return false;

            return BCrypt.Net.BCrypt.Verify(password, passwordToVerify);
        }

        /// <summary>
        /// Adds a new user to the database.
        public async Task<UserDto> AddUserAsync(User user)
        {
            await _context.Users.AddAsync(user);
            await _context.SaveChangesAsync();

            var userDto = _mapper.Map<UserDto>(user);

            return userDto;
        }

        /// <summary>
        /// Adds balance to a user's account.
        public async Task<decimal> AddBalanceAsync(int userId, decimal balanceToDeposit)
        {
            var user = await _context.Users
                .FirstOrDefaultAsync(x => x.Id == userId)
                ?? throw new NotFoundException("User not found!");

            user.Balance += balanceToDeposit;
            await _context.SaveChangesAsync();

            await _hubContext.Clients.User(user.Id.ToString()).SendAsync("GetBalance", user.Balance);

            return user.Balance;
        }

        /// <summary>
        /// Updates an existing user in the database.
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

        /// <summary>
        /// Updates the login status of a user.
        public async Task<bool> UpdateLoginStatusAsync(int id)
        {
            var existingUser = await _context.Users.FindAsync(id);
            if (existingUser == null) return false;

            existingUser.IsLoggedIn = false;

            await _context.SaveChangesAsync();
            return true;
        }

        /// <summary>
        /// Updates the user type of a user.
        public async Task<bool> UpdateUserTypeAsync(int id, UserType userType)
        {
            var existingUser = await _context.Users.FindAsync(id);
            if (existingUser == null) return false;

            existingUser.UserType = userType;

            await _context.SaveChangesAsync();
            return true;
        }

        /// <summary>
        /// Updates the password of a user.
        public async Task<bool> UpdateUserPasswordAsync(int id, string password)
        {
            var existingUser = await _context.Users.FindAsync(id);
            if (existingUser == null) return false;

            existingUser.PasswordHash = BCrypt.Net.BCrypt.HashPassword(password);

            await _context.SaveChangesAsync();
            return true;
        }

        /// <summary>
        /// Deletes a user from the database.
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
