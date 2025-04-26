using Event_Management.Models;
using Event_Management.Models.Dtos.OrganizerDtos;

namespace Event_Management.Repositories.OrganizerRepositoryFolder
{
    public interface IOrganizerRepository
    {
        Task<IEnumerable<OrganizerDto>> GetOrganizersAsync(); // Retrieves all organizers
        Task<OrganizerDto> GetOrganizerByIdAsync(int id); // Retrieves an organizer by its ID
        Task<IEnumerable<OrganizerDto>> GetOrganizersByLocationIdAsync(int locationId); // Retrieves organizers by location ID
        Task<OrganizerDto> GetOrganizerByUserIdAsync(int userId); // Retrieves an organizer by user ID
        Task<OrganizerDto> AddOrganizerAsync(OrganizerCreateDto organizerCreateDto); // Adds a new organizer
        Task<string> AddOrganizerOnLocationAsync(int organizerId, int locationId); // Adds an organizer to a location
        Task<string> RemoveOrganizerFromLocationAsync(int organizerId, int locationId); // Removes an organizer from a location
        Task<bool> UpdateOrganizerAsync(int id, OrganizerUpdateDto organizerUpdateDto); // Updates an existing organizer
        Task<bool> VerifyOrganizerAsync(int id); // Verifies an organizer
        Task<bool> DeleteOrganizerAsync(int id); // Deletes an organizer by its ID
    }
}
