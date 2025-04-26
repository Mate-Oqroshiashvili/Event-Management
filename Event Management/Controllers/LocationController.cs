using Event_Management.Exceptions;
using Event_Management.Models.Dtos.LocationDtos;
using Event_Management.Repositories.LocationRepositoryFolder;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Event_Management.Controllers
{
    [Authorize] // This attribute ensures that all actions in this controller require authorization.
    [Route("api/[controller]")]
    [ApiController]
    public class LocationController : ControllerBase
    {
        private readonly ILocationRepository _locationRepository; // This is the repository that will handle the data access for locations.

        public LocationController(ILocationRepository locationRepository)
        {
            _locationRepository = locationRepository;
        }

        [HttpGet("get-all-locations")]
        public async Task<ActionResult<IEnumerable<LocationDto>>> GetAllLocations()
        {
            try
            {
                var locations = await _locationRepository.GetLocationsAsync();

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

                return !removed ? throw new NotFoundException("Something went wrong during location removal process!") : Ok(new { message = "Location removed successfully!" });
            }
            catch (Exception ex)
            {
                throw new BadRequestException(ex.Message, ex.InnerException);
            }
        }
    }
}
