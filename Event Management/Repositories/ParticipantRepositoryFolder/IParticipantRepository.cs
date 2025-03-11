using Event_Management.Models;
using Event_Management.Models.Dtos.ParticipantDtos;

namespace Event_Management.Repositories.ParticipantRepositoryFolder
{
    public interface IParticipantRepository
    {
        Task<IEnumerable<ParticipantDto>> GetParticipantsAsync();
        Task<ParticipantDto> GetParticipantByIdAsync(int id);
        Task<ParticipantDto> AddParticipantAsync(ParticipantCreateDto participantCreateDto);
        Task<bool> UpdateParticipantAsync(int id, ParticipantUpdateDto participantUpdateDto);
        Task<bool> DeleteParticipantAsync(int id);
    }
}
