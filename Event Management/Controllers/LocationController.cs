using Event_Management.Exceptions;
using Event_Management.Extensions;
using Event_Management.Models.Dtos.LocationDtos;
using Event_Management.Repositories.LocationRepositoryFolder;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;

namespace Event_Management.Controllers
{
    [Authorize] // This attribute ensures that all actions in this controller require authorization.
    [Route("api/[controller]")]
    [ApiController]
    public class LocationController : ControllerBase
    {
        private readonly ILocationRepository _locationRepository; // This is the repository that will handle the data access for locations.
        private readonly IDistributedCache _distributedCache; // This is the distributed cache for caching data.

        public LocationController(ILocationRepository locationRepository, IDistributedCache distributedCache)
        {
            _locationRepository = locationRepository;
            _distributedCache = distributedCache;
        }

        [HttpGet("get-all-locations")]
        public async Task<ActionResult<IEnumerable<LocationDto>>> GetAllLocations()
        {
            try
            {
                // Check if the locations are already cached in Redis
                if (!string.IsNullOrEmpty(await _distributedCache.GetStringAsync("Locations")))
                {
                    // If cached, retrieve the locations from Redis
                    var redisResult = await _distributedCache.GetValue<List<LocationDto>>("Locations");
                    return Ok(redisResult);
                }

                // If not cached, retrieve the locations from the database
                var locations = await _locationRepository.GetLocationsAsync();
                // Cache the locations in Redis
                await _distributedCache.SetValue("Locations", locations);
                return locations == null ? throw new NotFoundException("Locations not found!") : Ok(new { locations });
            }
            catch (Exception ex)
            {
                throw new BadRequestException(ex.Message, ex.InnerException);
            }
        }

        [HttpGet("get-location-by-id/{locationId}")]
        public async Task<ActionResult<LocationDto>> GetLocationById(int locationId)
        {
            try
            {
                var location = await _locationRepository.GetLocationByIdAsync(locationId);

                return location == null ? throw new NotFoundException("Location not found!") : Ok(new { location });
            }
            catch (Exception ex)
            {
                throw new BadRequestException(ex.Message, ex.InnerException);
            }
        }

        [HttpGet("get-locations-by-organizer-id/{organizerId}")]
        public async Task<ActionResult<IEnumerable<LocationDto>>> GetLocationsByOrganizerId(int organizerId)
        {
            try
            {
                var locations = await _locationRepository.GetLocationsByOrganizerIdAsync(organizerId);

                return locations == null ? throw new NotFoundException("Locations not found!") : Ok(new { locations });
            }
            catch (Exception ex)
            {
                throw new BadRequestException(ex.Message, ex.InnerException);
            }
        }

        [Authorize(Roles = "ADMINISTRATOR")]
        [HttpPost("add-location")]
        public async Task<ActionResult<LocationDto>> AddLocation([FromForm] LocationCreateDto locationCreateDto)
        {
            try
            {
                var location = await _locationRepository.AddLocationAsync(locationCreateDto);
                // Clear the cache after adding a new location
                await _distributedCache.RemoveAsync("Locations");
                return location == null ? throw new NotFoundException("Something went wrong during location addition process!") : Ok(new { location });
            }
            catch (Exception ex)
            {
                throw new BadRequestException(ex.Message, ex.InnerException);
            }
        }

        [Authorize(Roles = "ADMINISTRATOR")]
        [HttpPut("update-location/{locationId}")]
        public async Task<ActionResult<string>> UpdateLocation(int locationId, [FromForm] LocationUpdateDto locationUpdateDto)
        {
            try
            {
                var updated = await _locationRepository.UpdateLocationAsync(locationId, locationUpdateDto);
                // Clear the cache after updating a location
                await _distributedCache.RemoveAsync("Locations");
                return !updated ? throw new NotFoundException("Something went wrong during location update process!") : Ok(new { message = "Location updated successfully!" });
            }
            catch (Exception ex)
            {
                throw new BadRequestException(ex.Message, ex.InnerException);
            }
        }

        [Authorize(Roles = "ADMINISTRATOR")]
        [HttpDelete("remove-location/{locationId}")]
        public async Task<ActionResult<string>> RemoveLocation(int locationId)
        {
            try
            {
                var removed = await _locationRepository.DeleteLocationAsync(locationId);
                // Clear the cache after removing a location
                await _distributedCache.RemoveAsync("Locations");
                return !removed ? throw new NotFoundException("Something went wrong during location removal process!") : Ok(new { message = "Location removed successfully!" });
            }
            catch (Exception ex)
            {
                throw new BadRequestException(ex.Message, ex.InnerException);
            }
        }
    }
}
