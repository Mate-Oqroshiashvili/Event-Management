using Event_Management.Models;
using Event_Management.Models.Dtos.OrganizerDtos;

namespace Event_Management.Repositories.OrganizerRepositoryFolder
{
    public interface IOrganizerRepository
    {
        Task<IEnumerable<OrganizerDto>> GetOrganizersAsync();
        Task<OrganizerDto> GetOrganizerByIdAsync(int id);
        Task<OrganizerDto> AddOrganizerAsync(OrganizerCreateDto organizerCreateDto);
        Task<bool> UpdateOrganizerAsync(int id, OrganizerUpdateDto organizerUpdateDto);
        Task<bool> DeleteOrganizerAsync(int id);
    }
}
