using Event_Management.Exceptions;
using Event_Management.Models.Dtos.TicketDtos;
using Event_Management.Repositories.TicketRepositoryFolder;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Event_Management.Controllers
{
    [Authorize] // This attribute ensures that all actions in this controller require authorization.
    [Route("api/[controller]")]
    [ApiController]
    public class TicketController : ControllerBase
    {
        private readonly ITicketRepository _ticketRepository; // This is the repository that will handle the data access for tickets.

        public TicketController(ITicketRepository ticketRepository)
        {
            _ticketRepository = ticketRepository;
        }

        [Authorize(Roles = "ORGANIZER")]
        [HttpGet("get-all-tickets")]
        public async Task<ActionResult<IEnumerable<TicketDto>>> GetAllTickets()
        {
            try
            {
                var tickets = await _ticketRepository.GetTicketsAsync();

                return tickets == null ? throw new NotFoundException("Tickets not found!") : Ok(new { tickets });
            }
            catch (Exception ex)
            {
                throw new BadRequestException(ex.Message, ex.InnerException);
            }
        }

        [HttpGet("get-ticket-by-id/{ticketId}")]
        public async Task<ActionResult<TicketDto>> GetTicketById(int ticketId)
        {
            try
            {
                var ticket = await _ticketRepository.GetTicketByIdAsync(ticketId);

                return ticket == null ? throw new NotFoundException("Ticket not found!") : Ok(new { ticket });
            }
            catch (Exception ex)
            {
                throw new BadRequestException(ex.Message, ex.InnerException);
            }
        }

        [HttpGet("get-tickets-by-event-id/{eventId}")]
        public async Task<ActionResult<IEnumerable<TicketDto>>> GetTicketsByEventId(int eventId)
        {
            try
            {
                var tickets = await _ticketRepository.GetTicketsByEventIdAsync(eventId);

                return tickets == null ? throw new NotFoundException($"No tickets are assigned to the event with id {eventId}") : Ok(new { tickets });
            }
            catch (Exception ex)
            {
                throw new BadRequestException(ex.Message, ex.InnerException);
            }
        }

        [HttpGet("get-tickets-by-user-id/{userId}")]
        public async Task<ActionResult<IEnumerable<TicketDto>>> GetTicketsByUserId(int userId)
        {
            try
            {
                var tickets = await _ticketRepository.GetTicketsByUserIdAsync(userId);

                return tickets == null ? throw new NotFoundException($"No tickets are assigned to the user with id {userId}") : Ok(new { tickets });
            }
            catch (Exception ex)
            {
                throw new BadRequestException(ex.Message, ex.InnerException);
            }
        }

        [Authorize(Roles = "ORGANIZER")]
        [HttpPost("add-ticket")]
        public async Task<ActionResult<TicketDto>> AddTicket([FromForm] TicketCreateDto ticketCreateDto)
        {
            try
            {
                var ticket = await _ticketRepository.AddTicketAsync(ticketCreateDto);

                return ticket == null ? throw new NotFoundException("Ticket creation process failed!") : Ok(new { ticket });
            }
            catch (Exception ex)
            {
                throw new BadRequestException(ex.Message, ex.InnerException);
            }
        }

        [Authorize(Roles = "ORGANIZER")]
        [HttpPost("validate-ticket")]
        public async Task<ActionResult<string>> ValidateTicket(IFormFile qrCodeImage)
        {
            try
            {
                var result = await _ticketRepository.ValidateTicketByQRCodeImage(qrCodeImage);

                return result == null ? throw new NotFoundException("Ticket validation process failed!") : Ok(new { result });
            }
            catch (Exception ex)
            {
                throw new BadRequestException(ex.Message, ex.InnerException);
            }
        }

        [Authorize(Roles = "ORGANIZER")]
        [HttpPut("update-ticket/{ticketId}")]
        public async Task<ActionResult<string>> UpdateTicket(int ticketId,[FromForm] TicketUpdateDto ticketUpdateDto)
        {
            try
            {
                var result = await _ticketRepository.UpdateTicketAsync(ticketId, ticketUpdateDto);

                return result == null ? throw new NotFoundException("Ticket update process failed!") : Ok(new { message = result });
            }
            catch (Exception ex)
            {
                throw new BadRequestException(ex.Message, ex.InnerException);
            }
        }

        [Authorize(Roles = "ORGANIZER")]
        [HttpDelete("remove-ticket/{ticketId}")]
        public async Task<ActionResult<string>> RemoveTicket(int ticketId)
        {
            try
            {
                var result = await _ticketRepository.DeleteTicketAsync(ticketId);

                return result == null ? throw new NotFoundException("Ticket deletion process failed!") : Ok(new { message = result });
            }
            catch (Exception ex)
            {
                throw new BadRequestException(ex.Message, ex.InnerException);
            }
        }
    }
}
