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
        private readonly DataContext _context;
        private readonly IImageRepository _imageRepository;
        private readonly IMapper _mapper;

        public LocationRepository(DataContext context, IMapper mapper, IImageRepository imageRepository)
        {
            _context = context;
            _mapper = mapper;
            _imageRepository = imageRepository;
        }

        public async Task<IEnumerable<LocationDto>> GetLocationsAsync()
        {
            var locations = await _context.Locations
                .Include(x => x.Events)
                .Include(x => x.Organizers)
                .ToListAsync();

            var locationsDtos = _mapper.Map<IEnumerable<LocationDto>>(locations);

            return locationsDtos;
        }

        public async Task<LocationDto> GetLocationByIdAsync(int id)
        {
            var location = await _context.Locations
                .Include(x => x.Events)
                .Include(x => x.Organizers)
                .FirstOrDefaultAsync(x => x.Id == id);

            var locationDto = _mapper.Map<LocationDto>(location);

            return locationDto;
        }

        public async Task<LocationDto> AddLocationAsync(LocationCreateDto locationCreateDto)
        {
            var location = _mapper.Map<Location>(locationCreateDto);

            var imageUrl = await _imageRepository.GenerateImageSource(locationCreateDto.Image);
            location.ImageUrl = imageUrl;

            await _context.Locations.AddAsync(location);
            await _context.SaveChangesAsync();

            var locationDto = _mapper.Map<LocationDto>(location);

            return locationDto;
        }

        public async Task<bool> UpdateLocationAsync(int id, LocationUpdateDto locationUpdateDto)
        {
            var existingLocation = await _context.Locations.FirstOrDefaultAsync(l => l.Id == id);
            if (existingLocation == null) return false;

            var location = _mapper.Map(locationUpdateDto, existingLocation);

            _context.Locations.Update(location);
            await _context.SaveChangesAsync();
            return true;
        }

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
