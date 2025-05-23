﻿using Event_Management.Exceptions;
using Event_Management.Models.Dtos.ParticipantDtos;
using Event_Management.Repositories.ParticipantRepositoryFolder;
using Event_Management.Repositories.TicketRepositoryFolder;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Event_Management.Controllers
{
    [Authorize] // This attribute ensures that all actions in this controller require authorization.
    [Route("api/[controller]")]
    [ApiController]
    public class ParticipantController : ControllerBase
    {
        private readonly IParticipantRepository _participantRepository; // This is the repository that will handle the data access for participants.
        private readonly ITicketRepository _ticketRepository; // This is the repository that will handle the data access for tickets.

        public ParticipantController(IParticipantRepository participantRepository, ITicketRepository ticketRepository)
        {
            _participantRepository = participantRepository;
            _ticketRepository = ticketRepository;
        }

        [Authorize(Roles = "ADMINISTRATOR")]
        [HttpGet("get-all-participants")]
        public async Task<ActionResult<IEnumerable<ParticipantDto>>> GetAllParticipants()
        {
            try
            {
                var participantDtos = await _participantRepository.GetParticipantsAsync();

                return participantDtos == null ? throw new NotFoundException("Participants can't be found!") : Ok(new { participantDtos });
            }
            catch (Exception ex)
            {
                throw new BadRequestException(ex.Message, ex.InnerException);
            }
        }

        [HttpGet("get-participant-by-id/{participantId}")]
        public async Task<ActionResult<ParticipantDto>> GetParticipantById(int participantId)
        {
            try
            {
                var participantDto = await _participantRepository.GetParticipantByIdAsync(participantId);

                return participantDto == null ? throw new NotFoundException("Participant can't be found!") : Ok(new { participantDto });
            }
            catch (Exception ex)
            {
                throw new BadRequestException(ex.Message, ex.InnerException);
            }
        }

        [HttpGet("get-participants-by-user-id/{userId}")]
        public async Task<ActionResult<IEnumerable<ParticipantDto>>> GetParticipantByUserId(int userId)
        {
            try
            {
                var participantDtos = await _participantRepository.GetParticipantsByUserIdAsync(userId);

                return participantDtos == null ? throw new NotFoundException("Participant can't be found!") : Ok(new { participantDtos });
            }
            catch (Exception ex)
            {
                throw new BadRequestException(ex.Message, ex.InnerException);
            }
        }

        [Authorize(Roles = "BASIC")]
        [HttpPost("register-user-as-participant")]
        public async Task<ActionResult<ParticipantDto>> AddParticipant([FromForm] ParticipantCreateDto participantCreateDto)
        {
            try
            {
                var participantDto = await _participantRepository.AddParticipantAsync(participantCreateDto);

                return participantDto == null ? throw new NotFoundException("Participant registration failed!") : Ok(new { participantDto });
            }
            catch (Exception ex)
            {
                throw new BadRequestException(ex.Message, ex.InnerException);
            }
        }

        [Authorize(Roles = "PARTICIPANT")]
        [HttpDelete("request-the-refund/{participantId}&{purchaseId}")]
        public async Task<ActionResult<string>> RequestTheRefund(int participantId, int purchaseId)
        {
            try
            {
                var deleted = await _participantRepository.DeleteParticipantAsync(participantId, purchaseId);

                return !deleted ? throw new NotFoundException("Participant not found or the ticket is already used!") : Ok(new { message = "Refund proccess executed successfully!" });
            }
            catch (Exception ex)
            {
                throw new BadRequestException(ex.Message, ex.InnerException);
            }
        }
    }
}
