using Event_Management.Models;
using Event_Management.Models.Dtos.LocationDtos;

namespace Event_Management.Repositories.LocationRepositoryFolder
{
    public interface ILocationRepository
    {
        Task<IEnumerable<LocationDto>> GetLocationsAsync(); // Retrieves all locations
        Task<LocationDto> GetLocationByIdAsync(int id); // Retrieves a location by its ID
        Task<IEnumerable<LocationDto>> GetLocationsByOrganizerIdAsync(int organizerId); // Retrieves locations by organizer ID
        Task<LocationDto> AddLocationAsync(LocationCreateDto locationCreateDto); // Adds a new location
        Task<bool> UpdateLocationAsync(int id, LocationUpdateDto locationUpdateDto); // Updates an existing location
        Task<bool> DeleteLocationAsync(int id); // Deletes a location by its ID
    }
}
