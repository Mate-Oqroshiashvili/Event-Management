﻿using AutoMapper;
using Event_Management.Data;
using Event_Management.Exceptions;
using Event_Management.Models;
using Event_Management.Models.Dtos.ParticipantDtos;
using Event_Management.Models.Enums;
using Event_Management.Web_Sockets;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;

namespace Event_Management.Repositories.ParticipantRepositoryFolder
{
    public class ParticipantRepository : IParticipantRepository
    {
        private readonly DataContext _context; // Database context for accessing the database
        private readonly IHubContext<ParticipantHub> _hubContext; // Hub context for sending messages to clients
        private readonly IMapper _mapper; // AutoMapper for mapping between DTOs and entities

        public ParticipantRepository(DataContext context, IHubContext<ParticipantHub> hubContext, IMapper mapper)
        {
            _context = context;
            _hubContext = hubContext;
            _mapper = mapper;
        }

        /// <summary>
        /// Retrieves all participants from the database.
        public async Task<IEnumerable<ParticipantDto>> GetParticipantsAsync()
        {
            var participants = await _context.Participants
                .Include(x => x.Event)
                .Include(x => x.Ticket)
                .Include(x => x.User)
                .Include(x => x.Purchase)
                .ToListAsync();

            var participantDtos = _mapper.Map<IEnumerable<ParticipantDto>>(participants);

            return participantDtos;
        }

        /// <summary>
        /// Retrieves a participant by its ID.
        public async Task<ParticipantDto> GetParticipantByIdAsync(int id)
        {
            var participant = await _context.Participants
                .Include(x => x.Event)
                .Include(x => x.Ticket)
                .Include(x => x.User)
                .Include(x => x.Purchase)
                .FirstOrDefaultAsync(x => x.Id == id);

            var participantDto = _mapper.Map<ParticipantDto>(participant);

            return participantDto;
        }

        /// <summary>
        /// Retrieves participants associated with a specific user ID.
        public async Task<IEnumerable<ParticipantDto>> GetParticipantsByUserIdAsync(int id)
        {
            var participants = await _context.Participants
                .Where(x => x.UserId == id)
                .Include(x => x.Event)
                .Include(x => x.Ticket)
                .Include(x => x.User)
                .Include(x => x.Purchase)
                .ToListAsync();

            var participantDtos = _mapper.Map<IEnumerable<ParticipantDto>>(participants);

            return participantDtos;
        }

        /// <summary>
        /// Adds a new participant to the database.
        public async Task<ParticipantDto> AddParticipantAsync(ParticipantCreateDto participantCreateDto)
        {
            try
            {
                var participant = _mapper.Map<Participant>(participantCreateDto);

                participant.Ticket = await _context.Tickets
                    .Include(x => x.Participants)
                    .Include(x => x.Purchases)
                    .Include(x => x.Event)
                    .Include(x => x.Users)
                    .FirstOrDefaultAsync(x => x.Id == participantCreateDto.TicketId)
                    ?? throw new NotFoundException($"Ticket with ID {participantCreateDto.TicketId} not found!");

                participant.Event = await _context.Events
                    .Include(x => x.Participants)
                    .Include(x => x.Tickets)
                    .Include(x => x.Location)
                    .Include(x => x.Organizer)
                    .FirstOrDefaultAsync(x => x.Id == participantCreateDto.EventId)
                    ?? throw new NotFoundException($"Event with ID {participantCreateDto.EventId} not found!");

                participant.User = await _context.Users
                    .Include(x => x.Participants)
                    .Include(x => x.Purchases)
                    .Include(x => x.Organizer)
                    .Include(x => x.Tickets)
                    .Include(x => x.Comments)
                    .Include(x => x.Reviews)
                    .FirstOrDefaultAsync(x => x.Id == participantCreateDto.UserId)
                    ?? throw new NotFoundException($"User with ID {participantCreateDto.UserId} not found!");

                await _context.Participants.AddAsync(participant);
                await _context.SaveChangesAsync();

                var participantDto = _mapper.Map<ParticipantDto>(participant);

                return participantDto;
            }
            catch (Exception ex)
            {
                throw new BadRequestException(ex.Message, ex.InnerException);
            }
        }

        /// <summary>
        /// Updates an existing participant in the database.
        public async Task<bool> UpdateParticipantAsync(int id, ParticipantUpdateDto participantUpdateDto)
        {
            var existingParticipant = await _context.Participants.FindAsync(id);
            if (existingParticipant == null) return false;

            _mapper.Map(participantUpdateDto, existingParticipant);

            await _context.SaveChangesAsync();
            return true;
        }

        /// <summary>
        /// Deletes a participant by its ID and handles the refund process.
        public async Task<bool> DeleteParticipantAsync(int participantId, int purchaseId)
        {
            var participant = await _context.Participants
                .Include(p => p.Ticket)
                .ThenInclude(t => t.Purchases)
                .Include(p => p.User)
                .FirstOrDefaultAsync(p => p.Id == participantId);

            if (participant == null || participant.IsUsed) return false;

            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                var ticket = participant.Ticket ?? throw new NotFoundException("Ticket not found!");
                var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == participant.UserId) ?? throw new NotFoundException("User not found!");

                // Ensure the purchase exists and is associated with this participant's ticket
                var purchase = await _context.Purchases
                    .Include(p => p.Tickets)
                    .Include(p => p.PromoCode)
                    .FirstOrDefaultAsync(p => p.Id == purchaseId && p.Tickets.Any(t => t.Id == participant.TicketId) && p.Participants.Any(x => x.Id == participantId)) ?? throw new NotFoundException("The provided purchase does not belong to this participant.");

                // Refund the user before deleting anything
                decimal refundAmount = ticket.Price;

                // Apply promo code discount if used in this purchase
                if (purchase.isPromoCodeUsed && purchase.PromoCode != null)
                {
                    refundAmount -= (refundAmount * purchase.PromoCode.SaleAmountInPercentages) / 100;
                    user.Balance += refundAmount;
                }
                else
                {
                    user.Balance += refundAmount;
                }

                _context.Entry(user).Property(u => u.Balance).IsModified = true;

                // Remove the participant first
                _context.Participants.Remove(participant);
                await _context.SaveChangesAsync(); // Ensure participant removal reflects in the next check

                // Check if there are any participants left for this purchase
                bool hasRemainingParticipants = await _context.Participants
                    .AnyAsync(p => p.PurchaseId == purchaseId);

                // Always restore ticket quantity if applicable
                if (ticket.Quantity < int.MaxValue)
                {
                    ticket.Quantity += 1;
                }

                // Always update ticket status if it was sold out
                if (ticket.Status == TicketStatus.SOLD_OUT)
                {
                    ticket.Status = TicketStatus.AVAILABLE;
                }

                // Then handle purchase deletion if no remaining participants
                if (!hasRemainingParticipants)
                {
                    _context.Purchases.Remove(purchase);
                }

                // Check if the user has any other participants left
                bool hasOtherParticipants = await _context.Participants.AnyAsync(p => p.UserId == user.Id);
                if (!hasOtherParticipants && user.Role != Role.ORGANIZER && user.Role != Role.ADMINISTRATOR)
                {
                    user.Role = Role.BASIC;
                }

                await _context.SaveChangesAsync();

                await _hubContext.Clients.User(user.Id.ToString()).SendAsync("GetBalance", user.Balance);

                await transaction.CommitAsync();

                return true;
            }
            catch (DbUpdateConcurrencyException)
            {
                await transaction.RollbackAsync();
                throw new InvalidOperationException("The user's balance was modified by another transaction. Please retry the refund.");
            }
            catch (Exception)
            {
                await transaction.RollbackAsync();
                throw;
            }
        }
    }
}
