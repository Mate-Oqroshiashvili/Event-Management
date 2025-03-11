using AutoMapper;
using Event_Management.Data;
using Event_Management.Models;
using Event_Management.Models.Dtos.TicketDtos;
using Microsoft.EntityFrameworkCore;

namespace Event_Management.Repositories.TicketRepositoryFolder
{
    public class TicketRepository : ITicketRepository
    {
        private readonly DataContext _context;
        private readonly IMapper _mapper;

        public TicketRepository(DataContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<IEnumerable<TicketDto>> GetTicketsAsync()
        {
            var tickets = await _context.Tickets
                .Include(x => x.Participant)
                .Include(x => x.Purchase)
                .Include(x => x.Event)
                .Include(x => x.User)
                .ToListAsync();

            var ticketDtos = _mapper.Map<IEnumerable<TicketDto>>(tickets);

            return ticketDtos;
        }

        public async Task<TicketDto> GetTicketByIdAsync(int id)
        {
            var ticket = await _context.Tickets
                .Include(x => x.Participant)
                .Include(x => x.Purchase)
                .Include(x => x.Event)
                .Include(x => x.User)
                .FirstOrDefaultAsync(x => x.Id == id);

            var ticketDto = _mapper.Map<TicketDto>(ticket);

            return ticketDto;
        }

        public async Task<TicketDto> AddTicketAsync(TicketCreateDto ticketCreateDto)
        {
            var ticket = _mapper.Map<Ticket>(ticketCreateDto);

            await _context.Tickets.AddAsync(ticket);
            await _context.SaveChangesAsync();

            var ticketDto = _mapper.Map<TicketDto>(ticket);

            return ticketDto;
        }

        public async Task<bool> UpdateTicketAsync(int id, TicketUpdateDto ticketUpdateDto)
        {
            var existingTicket = await _context.Tickets.FirstOrDefaultAsync(t => t.Id == id);
            if (existingTicket == null) return false;

            var ticket = _mapper.Map(ticketUpdateDto, existingTicket);
            _context.Tickets.Update(existingTicket);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteTicketAsync(int id)
        {
            var ticket = await _context.Tickets.FindAsync(id);
            if (ticket == null) return false;

            _context.Tickets.Remove(ticket);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
