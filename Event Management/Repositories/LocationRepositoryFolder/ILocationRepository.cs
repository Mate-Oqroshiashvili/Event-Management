using Event_Management.Models;
using Event_Management.Models.Dtos.LocationDtos;

namespace Event_Management.Repositories.LocationRepositoryFolder
{
    public interface ILocationRepository
    {
        Task<IEnumerable<LocationDto>> GetLocationsAsync();
        Task<LocationDto> GetLocationByIdAsync(int id);
        Task<LocationDto> AddLocationAsync(LocationCreateDto locationCreateDto);
        Task<bool> UpdateLocationAsync(int id, LocationUpdateDto locationUpdateDto);
        Task<bool> DeleteLocationAsync(int id);
    }
}
