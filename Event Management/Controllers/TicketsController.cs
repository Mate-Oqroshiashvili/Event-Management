using Event_Management.Models.Dtos.PurchaseDtos;
using Event_Management.Models.Dtos.TicketDtos;
using Microsoft.AspNetCore.Mvc;

namespace Event_Management.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TicketsController : ControllerBase
    {
        public TicketsController()
        {

        }

        //[HttpGet("get-all-tickets")]
        //public async Task<ActionResult<IEnumerable<TicketDto>>> GetAllTickets() { }

        //[HttpGet("get-ticket-by-id/{ticketId}")]
        //public async Task<ActionResult<TicketDto>> GetTicketById(int ticketId) { }

        //[HttpPost("add-tickets")]
        //public async Task<ActionResult<TicketDto>> AddTickets(TicketCreateDto ticketCreateDto) { }

        //[HttpPost("purchase-ticket")]
        //public async Task<ActionResult<PurchaseDto>> PurchaseTicket(PurchaseCreateDto purchaseCreateDto) { }

        //[HttpPost("validate")]
        //public async Task<ActionResult<???>> Validate() { } // ???
    }
}
