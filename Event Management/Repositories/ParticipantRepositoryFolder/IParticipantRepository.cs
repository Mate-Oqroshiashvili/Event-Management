using Event_Management.Models;
using Event_Management.Models.Dtos.ParticipantDtos;

namespace Event_Management.Repositories.ParticipantRepositoryFolder
{
    public interface IParticipantRepository
    {
        Task<IEnumerable<ParticipantDto>> GetParticipantsAsync(); // Retrieves all participants
        Task<ParticipantDto> GetParticipantByIdAsync(int id); // Retrieves a participant by its ID
        Task<IEnumerable<ParticipantDto>> GetParticipantsByUserIdAsync(int id); // Retrieves participants by user ID
        Task<ParticipantDto> AddParticipantAsync(ParticipantCreateDto participantCreateDto); // Adds a new participant
        Task<bool> UpdateParticipantAsync(int id, ParticipantUpdateDto participantUpdateDto); // Updates an existing participant
        Task<bool> DeleteParticipantAsync(int participantId, int purchaseId); // Deletes a participant by its ID
    } 
}
