using AutoMapper;
using Event_Management.Data;
using Event_Management.Models;
using Event_Management.Models.Dtos.ParticipantDtos;
using Microsoft.EntityFrameworkCore;

namespace Event_Management.Repositories.ParticipantRepositoryFolder
{
    public class ParticipantRepository : IParticipantRepository
    {
        private readonly DataContext _context;
        private readonly IMapper _mapper;

        public ParticipantRepository(DataContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<IEnumerable<ParticipantDto>> GetParticipantsAsync()
        {
            var participants = await _context.Participants
                .Include(x => x.Event)
                .Include(x => x.Ticket)
                .Include(x => x.User)
                .ToListAsync();

            var participantDtos = _mapper.Map<IEnumerable<ParticipantDto>>(participants);

            return participantDtos;
        }

        public async Task<ParticipantDto> GetParticipantByIdAsync(int id)
        {
            var participant = await _context.Participants
                .Include(x => x.Event)
                .Include(x => x.Ticket)
                .Include(x => x.User)
                .FirstOrDefaultAsync(x => x.Id == id);

            var participantDto = _mapper.Map<ParticipantDto>(participant);

            return participantDto;
        }

        public async Task<ParticipantDto> AddParticipantAsync(ParticipantCreateDto participantCreateDto)
        {
            var participant = _mapper.Map<Participant>(participantCreateDto);

            await _context.Participants.AddAsync(participant);
            await _context.SaveChangesAsync();

            var participantDto = _mapper.Map<ParticipantDto>(participant);

            return participantDto;
        }

        public async Task<bool> UpdateParticipantAsync(int id, ParticipantUpdateDto participantUpdateDto)
        {
            var existingParticipant = await _context.Participants.FindAsync(id);
            if (existingParticipant == null) return false;

            var participant = _mapper.Map(participantUpdateDto, existingParticipant);
            _context.Participants.Update(participant);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteParticipantAsync(int id)
        {
            var Participant = await _context.Participants.FindAsync(id);
            if (Participant == null) return false;

            _context.Participants.Remove(Participant);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
