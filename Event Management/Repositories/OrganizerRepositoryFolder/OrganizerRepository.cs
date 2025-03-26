using AutoMapper;
using Event_Management.Data;
using Event_Management.Exceptions;
using Event_Management.Models;
using Event_Management.Models.Dtos.OrganizerDtos;
using Event_Management.Repositories.CodeRepositoryFolder;
using Event_Management.Repositories.ImageRepositoryFolder;
using Microsoft.EntityFrameworkCore;

namespace Event_Management.Repositories.OrganizerRepositoryFolder
{
    public class OrganizerRepository : IOrganizerRepository
    {
        private readonly DataContext _context;
        private readonly IImageRepository _imageRepository;
        private readonly ICodeRepository _codeRepository;
        private readonly IMapper _mapper;

        public OrganizerRepository(DataContext context, IImageRepository imageRepository, ICodeRepository codeRepository, IMapper mapper)
        {
            _context = context;
            _imageRepository = imageRepository;
            _codeRepository = codeRepository;
            _mapper = mapper;
        }

        public async Task<IEnumerable<OrganizerDto>> GetOrganizersAsync()
        {
            var organizers = await _context.Organizers
                .Include(x => x.Events)
                .Include(x => x.Locations)
                .Include(x => x.User)
                .ToListAsync();

            var organizerDtos = _mapper.Map<IEnumerable<OrganizerDto>>(organizers);

            return organizerDtos;
        }

        public async Task<OrganizerDto> GetOrganizerByIdAsync(int id)
        {
            var organizer = await _context.Organizers
                .Include(x => x.Events)
                .Include(x => x.Locations)
                .Include(x => x.User)
                .FirstOrDefaultAsync(x => x.Id == id);

            var organizerDto = _mapper.Map<OrganizerDto>(organizer);

            return organizerDto;
        }

        public async Task<OrganizerDto> AddOrganizerAsync(OrganizerCreateDto organizerCreateDto)
        {
            var organizer = _mapper.Map<Organizer>(organizerCreateDto);

            organizer.User = await _context.Users
                .Include(x => x.Participants)
                .Include(x => x.Purchases)
                .Include(x => x.Organizer)
                    .ThenInclude(x => x!.Events)
                .Include(x => x.Organizer)
                    .ThenInclude(x => x!.Locations)
                .Include(x => x.Tickets)
                .Include(x => x.Comments)
                .Include(x => x.Reviews)
                .Include(x => x.UsedPromoCodes)
                .FirstOrDefaultAsync(u => u.Id == organizerCreateDto.UserId) 
                ?? throw new NotFoundException("User not found!");

            var logoUrl = await _imageRepository.GenerateImageSource(organizerCreateDto.Logo);
            organizer.LogoUrl = logoUrl;
            organizer.UserId = organizerCreateDto.UserId;
            organizer.User.Role = Models.Enums.Role.ORGANIZER;

            await _context.Organizers.AddAsync(organizer);
            await _context.SaveChangesAsync();

            var organizerDto = _mapper.Map<OrganizerDto>(organizer);
            return organizerDto;
        }

        public async Task<string> AddOrganizerOnLocationAsync(int organizerId, int locationId)
        {
            try
            {
                var organizer = await _context.Organizers
                    .Include(o => o.Locations)
                    .FirstOrDefaultAsync(o => o.Id == organizerId)
                    ?? throw new NotFoundException("Organizer not found!");

                var location = await _context.Locations
                    .FirstOrDefaultAsync(l => l.Id == locationId)
                    ?? throw new NotFoundException("Location not found!");

                if (organizer.Locations.Any(l => l.Id == locationId))
                    throw new BadRequestException("This organizer is already linked to the location.");

                if (!organizer.IsVerified)
                    throw new BadRequestException("Organizer is not verified!");

                organizer.Locations.Add(location);

                await _context.SaveChangesAsync();

                return $"Organizer has been successfully added to this location - {location.Address}.";
            }
            catch (DbUpdateException dbEx)
            {
                throw new BadRequestException("Database update failed!", dbEx);
            }
            catch (Exception ex)
            {
                throw new BadRequestException(ex.Message, ex.InnerException);
            }
        }

        public async Task<string> RemoveOrganizerFromLocationAsync(int organizerId, int locationId)
        {
            try
            {
                var organizer = await _context.Organizers
                    .Include(o => o.Locations)
                    .FirstOrDefaultAsync(o => o.Id == organizerId)
                    ?? throw new NotFoundException("Organizer not found!");

                var location = await _context.Locations
                    .Include(l => l.Organizers)
                    .FirstOrDefaultAsync(l => l.Id == locationId)
                    ?? throw new NotFoundException("Location not found!");

                if (!organizer.Locations.Any(l => l.Id == locationId))
                    throw new BadRequestException("This organizer is not linked to the location.");

                if (!organizer.IsVerified)
                    throw new BadRequestException("Organizer is not verified!");

                organizer.Locations.Remove(location);
                await _context.SaveChangesAsync();

                return $"Organizer has been successfully removed from this location - {location.Address}.";
            }
            catch (Exception ex)
            {
                throw new BadRequestException(ex.Message, ex.InnerException);
            }
        }

        public async Task<bool> UpdateOrganizerAsync(int id, OrganizerUpdateDto organizerUpdateDto)
        {
            var existingOrganizer = await _context.Organizers.FirstOrDefaultAsync(o => o.Id == id);
            if (existingOrganizer == null) return false;

            _mapper.Map(organizerUpdateDto, existingOrganizer);

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> VerifyOrganizerAsync(int id)
        {
            var existingOrganizer = await _context.Organizers.FindAsync(id);
            if (existingOrganizer == null || existingOrganizer.IsVerified) return false;

            existingOrganizer.IsVerified = true;

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteOrganizerAsync(int id)
        {
            var organizer = await _context.Organizers.FindAsync(id);
            if (organizer == null) return false;

            var user = await _context.Users.FindAsync(organizer.UserId);

            if (user == null) return false;

            user.Role = Models.Enums.Role.BASIC;

            _context.Organizers.Remove(organizer);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<string> SendCodes(int userId)
        {
            try
            {
                var user = await _context.Users.FindAsync(userId)
                    ?? throw new NotFoundException($"User with the Id {userId} not found!");

                var rng = new Random();
                var emailCode = rng.Next(100000, 999999).ToString();
                var smsCode = rng.Next(100000, 999999).ToString();

                user.EmailVerificationCode = emailCode;
                user.SmsVerificationCode = smsCode;
                user.CodeExpiration = DateTime.UtcNow.AddMinutes(10);

                await _context.SaveChangesAsync();

                await _codeRepository.SendToEmail(user.Email, $"Your email verification code is {emailCode}");
                await _codeRepository.SendToPhone(user.PhoneNumber, $"Your SMS verification code is {smsCode}");

                return "Verification codes sent successfully.";
            }
            catch (Exception ex)
            {
                throw new BadRequestException(ex.Message, ex.InnerException);
            }
        }
    }
}
