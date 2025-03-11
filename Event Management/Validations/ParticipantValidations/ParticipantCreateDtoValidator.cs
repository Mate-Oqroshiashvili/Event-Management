using Event_Management.Models.Dtos.ParticipantDtos;
using FluentValidation;

namespace Event_Management.Validations.ParticipantValidations
{
    public class ParticipantCreateDtoValidator : AbstractValidator<ParticipantCreateDto>
    {
        public ParticipantCreateDtoValidator()
        {
            RuleFor(p => p.EventId)
                .GreaterThan(0).WithMessage("Event ID must be a positive integer.");

            RuleFor(p => p.UserId)
                .GreaterThan(0).WithMessage("User ID must be a positive integer.");

            RuleFor(p => p.TicketId)
                .GreaterThan(0).WithMessage("Ticket ID must be a positive integer.");
        }
    }
}
