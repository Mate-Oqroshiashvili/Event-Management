using AutoMapper;
using Event_Management.Data;
using Event_Management.Exceptions;
using Event_Management.Models;
using Event_Management.Models.Dtos.OrganizerDtos;
using Event_Management.Repositories.ImageRepositoryFolder;
using Microsoft.EntityFrameworkCore;

namespace Event_Management.Repositories.OrganizerRepositoryFolder
{
    public class OrganizerRepository : IOrganizerRepository
    {
        private readonly DataContext _context; // Database context for accessing the database
        private readonly IImageRepository _imageRepository; // Image repository for handling image uploads
        private readonly IMapper _mapper; // AutoMapper for mapping between DTOs and entities

        public OrganizerRepository(DataContext context, IImageRepository imageRepository, IMapper mapper)
        {
            _context = context;
            _imageRepository = imageRepository;
            _mapper = mapper;
        }

        /// <summary>
        /// Retrieves all organizers from the database.
        public async Task<IEnumerable<OrganizerDto>> GetOrganizersAsync()
        {
            var organizers = await _context.Organizers
                .Include(x => x.User)
                .Include(x => x.Locations)
                .ToListAsync();

            var organizerDtos = _mapper.Map<IEnumerable<OrganizerDto>>(organizers);

            return organizerDtos;
        }

        /// <summary>
        /// Retrieves an organizer by its ID.
        public async Task<OrganizerDto> GetOrganizerByIdAsync(int id)
        {
            var organizer = await _context.Organizers
                .Include(x => x.User)
                .Include(x => x.Locations)
                .FirstOrDefaultAsync(x => x.Id == id);

            var organizerDto = _mapper.Map<OrganizerDto>(organizer);

            return organizerDto;
        }

        /// <summary>
        /// Retrieves an organizer by its user ID.
        public async Task<OrganizerDto> GetOrganizerByUserIdAsync(int userId)
        {
            var organizer = await _context.Organizers
                .Include(x => x.Events)
                .Include(x => x.Locations)
                .Include(x => x.User)
                .FirstOrDefaultAsync(x => x.UserId == userId);

            var organizerDto = _mapper.Map<OrganizerDto>(organizer);

            return organizerDto;
        }

        /// <summary>
        /// Retrieves organizers associated with a specific location ID.
        public async Task<IEnumerable<OrganizerDto>> GetOrganizersByLocationIdAsync(int locationId)
        {
            var organizers = await _context.Organizers
                .Where(x => x.Locations.Any(x => x.Id == locationId))
                .Include(x => x.Events)
                .Include(x => x.Locations)
                .Include(x => x.User)
                .ToListAsync();

            var organizerDtos = _mapper.Map<IEnumerable<OrganizerDto>>(organizers);

            return organizerDtos;
        }

        /// <summary>
        /// Adds a new organizer to the database.
        public async Task<OrganizerDto> AddOrganizerAsync(OrganizerCreateDto organizerCreateDto)
        {
            try
            {
                var existingOrganizer = await _context.Organizers.FirstOrDefaultAsync(x => x.UserId == organizerCreateDto.UserId);

                if (existingOrganizer != null)
                    throw new BadRequestException("Organizer already exists!");

                var organizer = _mapper.Map<Organizer>(organizerCreateDto);

                organizer.User = await _context.Users
                    .Include(x => x.Participants)
                    .Include(x => x.Purchases)
                    .Include(x => x.Tickets)
                    .Include(x => x.Comments)
                    .Include(x => x.Reviews)
                    .Include(x => x.UsedPromoCodes)
                    .FirstOrDefaultAsync(u => u.Id == organizerCreateDto.UserId)
                    ?? throw new NotFoundException("User not found!");

                organizer.Name = organizer.User.Name;
                organizer.Email = organizer.User.Email;
                organizer.PhoneNumber = organizer.User.PhoneNumber;

                var logoUrl = await _imageRepository.GenerateImageSource(organizerCreateDto.Logo);
                organizer.LogoUrl = logoUrl;
                organizer.UserId = organizerCreateDto.UserId;
                organizer.User.Role = Models.Enums.Role.ORGANIZER;

                await _context.Organizers.AddAsync(organizer);
                await _context.SaveChangesAsync();

                var organizerDto = _mapper.Map<OrganizerDto>(organizer);
                return organizerDto;
            }
            catch (Exception ex) 
            {
                throw new BadRequestException(ex.Message, ex.InnerException);
            }
        }

        /// <summary>
        /// Adds an organizer to a location.
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

        /// <summary>
        /// Removes an organizer from a location.
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

                if (location.Events.Any(x => x.OrganizerId == organizerId))
                    throw new BadRequestException("Location can't be removed from organizer's locations list, there is the event planned on this location!");

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

        /// <summary>
        /// Updates an existing organizer's details.
        public async Task<bool> UpdateOrganizerAsync(int id, OrganizerUpdateDto organizerUpdateDto)
        {
            var existingOrganizer = await _context.Organizers.FirstOrDefaultAsync(o => o.Id == id);
            if (existingOrganizer == null) return false;

            if (organizerUpdateDto.Logo != null) 
            {
                var logoUrl = await _imageRepository.ChangeOrganizerLogoImage(id, organizerUpdateDto.Logo);
            }

            _mapper.Map(organizerUpdateDto, existingOrganizer);

            await _context.SaveChangesAsync();
            return true;
        }

        /// <summary>
        /// Verifies an organizer.
        public async Task<bool> VerifyOrganizerAsync(int id)
        {
            var existingOrganizer = await _context.Organizers.FindAsync(id);
            if (existingOrganizer == null || existingOrganizer.IsVerified) return false;

            existingOrganizer.IsVerified = true;

            await _context.SaveChangesAsync();
            return true;
        }

        /// <summary>
        /// Deletes an organizer by its ID.
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
    }
}
