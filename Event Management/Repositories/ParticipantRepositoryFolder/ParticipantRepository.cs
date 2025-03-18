using AutoMapper;
using Event_Management.Data;
using Event_Management.Models;
using Event_Management.Models.Dtos.ParticipantDtos;
using Event_Management.Models.Enums;
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

        public async Task<ParticipantDto> GetParticipantByUserIdAsync(int id)
        {
            var participant = await _context.Participants
                .Include(x => x.Event)
                .Include(x => x.Ticket)
                .Include(x => x.User)
                .FirstOrDefaultAsync(x => x.UserId == id);

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

            _mapper.Map(participantUpdateDto, existingParticipant);

            await _context.SaveChangesAsync();
            return true;
        }

        // refund logic
        public async Task<bool> DeleteParticipantAsync(int id)
        {
            var participant = await _context.Participants
                .Include(x => x.Event)
                .Include(x => x.Ticket)
                .Include(x => x.User)
                .FirstOrDefaultAsync(x => x.Id == id);

            if (participant == null || participant.Ticket?.IsUsed == true) return false;

            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                var ticket = participant.Ticket;
                var purchase = ticket?.Purchase;
                var user = participant.User;

                if (ticket != null)
                {
                    // Restore ticket quantity
                    if (ticket.Quantity < int.MaxValue) ticket.Quantity += 1;

                    // Update ticket status if it was sold out
                    if (ticket.Status == TicketStatus.SOLD_OUT) ticket.Status = TicketStatus.AVAILABLE;

                    // Check if there are remaining participants using the ticket
                    var remainingParticipants = await _context.Participants
                        .Where(p => p.TicketId == ticket.Id && p.Id != participant.Id)
                        .ToListAsync();

                    if (!remainingParticipants.Any()) ticket.Participant = null;
                }

                // Process Refund (Only if there's a valid purchase)
                if (purchase != null)
                {
                    if (ticket == null)
                        throw new Exception($"There is no associated ticket on specific participant - {participant.User.Name}");

                    var remainingTickets = await _context.Tickets
                        .Where(t => t.PurchaseId == purchase.Id && t.Id != ticket.Id)
                        .ToListAsync();

                    decimal refundAmount = ticket.Price; // Base refund amount

                    // Handle promo code discount if used
                    if (purchase.isPromoCodeUsed && purchase.PromoCode != null)
                    {
                        refundAmount -= (refundAmount * purchase.PromoCode.SaleAmountInPercentages) / 100;
                    }

                    // Refund to user balance
                    user.Balance += refundAmount;

                    // If no tickets left in the purchase, delete purchase
                    if (remainingTickets.Count == 0) _context.Purchases.Remove(purchase);
                }

                // Remove participant
                _context.Participants.Remove(participant);
                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                return true;
            }
            catch (Exception)
            {
                await transaction.RollbackAsync();
                throw;
            }
        }
    }
}
