using Event_Management.Models.Dtos.TicketDtos;
using Event_Management.Models.Enums;
using FluentValidation;

namespace Event_Management.Validations.TicketValidations
{
    public class TicketCreateDtoValidator : AbstractValidator<TicketCreateDto>
    {
        public TicketCreateDtoValidator()
        {
            RuleFor(x => x.EventId)
                .NotEmpty().WithMessage("Event Id is required.")
                .GreaterThan(0).WithMessage("Event Id must be greater than 0.");

            RuleFor(x => x.Type)
                .NotNull().WithMessage("Ticket type is required.")
                .Must(ticketType => Enum.IsDefined(typeof(TicketType), ticketType))
                .WithMessage("Invalid ticket type.");

            RuleFor(x => x.Price)
                .NotNull().WithMessage("Price is required.")
                .GreaterThan(0).WithMessage("Price must be greater than 0.")
                .LessThanOrEqualTo(10000).WithMessage("Price cannot exceed 10,000.");

            RuleFor(x => x.Quantity)
                .NotNull().WithMessage("Quantity is required.")
                .GreaterThan(0).WithMessage("Quantity must be greater than 0.");
        }
    }
}
