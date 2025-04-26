using AutoMapper;
using Event_Management.Data;
using Event_Management.Models;
using Event_Management.Models.Dtos.LocationDtos;
using Event_Management.Repositories.ImageRepositoryFolder;
using Microsoft.EntityFrameworkCore;

namespace Event_Management.Repositories.LocationRepositoryFolder
{
    public class LocationRepository : ILocationRepository
    {
        private readonly DataContext _context; // Database context for accessing the database
        private readonly IImageRepository _imageRepository; // Image repository for handling image uploads
        private readonly IMapper _mapper; // AutoMapper for mapping between DTOs and entities

        public LocationRepository(DataContext context, IMapper mapper, IImageRepository imageRepository)
        {
            _context = context;
            _mapper = mapper;
            _imageRepository = imageRepository;
        }

        /// <summary>
        /// Retrieves all locations from the database.
        public async Task<IEnumerable<LocationDto>> GetLocationsAsync()
        {
            var locations = await _context.Locations
                .ToListAsync();

            var locationsDtos = _mapper.Map<IEnumerable<LocationDto>>(locations);

            return locationsDtos;
        }

        /// <summary>
        /// Retrieves a location by its ID.
        public async Task<LocationDto> GetLocationByIdAsync(int id)
        {
            var location = await _context.Locations
                .FirstOrDefaultAsync(x => x.Id == id);

            var locationDto = _mapper.Map<LocationDto>(location);

            return locationDto;
        }

        /// <summary>
        /// Retrieves locations associated with a specific organizer ID.
        public async Task<IEnumerable<LocationDto>> GetLocationsByOrganizerIdAsync(int organizerId)
        {
            var locations = await _context.Locations
                .Where(l => l.Organizers.Any(o => o.Id == organizerId))
                .ToListAsync();

            var locationsDtos = _mapper.Map<IEnumerable<LocationDto>>(locations);

            return locationsDtos;
        }

        /// <summary>
        /// Adds a new location to the database.
        public async Task<LocationDto> AddLocationAsync(LocationCreateDto locationCreateDto)
        {
            var location = _mapper.Map<Location>(locationCreateDto);

            location.RemainingCapacity = locationCreateDto.MaxCapacity;
            var imageUrl = await _imageRepository.GenerateImageSource(locationCreateDto.Image);
            location.ImageUrl = imageUrl;

            await _context.Locations.AddAsync(location);
            await _context.SaveChangesAsync();

            var locationDto = _mapper.Map<LocationDto>(location);

            return locationDto;
        }

        /// <summary>
        /// Updates an existing location in the database.
        public async Task<bool> UpdateLocationAsync(int id, LocationUpdateDto locationUpdateDto)
        {
            var existingLocation = await _context.Locations.FirstOrDefaultAsync(l => l.Id == id);
            if (existingLocation == null) return false;

            if (locationUpdateDto.Image != null)
            {
                var imageUrl = await _imageRepository.GenerateImageSource(locationUpdateDto.Image);
                existingLocation.ImageUrl = imageUrl;
            }

            var location = _mapper.Map(locationUpdateDto, existingLocation);

            await _context.SaveChangesAsync();
            return true;
        }

        /// <summary>
        /// Deletes a location from the database.
        public async Task<bool> DeleteLocationAsync(int id)
        {
            var location = await _context.Locations.FindAsync(id);
            if (location == null) return false;

            _context.Locations.Remove(location);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
