using Event_Management.Models.Dtos.EventDtos;
using FluentValidation;

namespace Event_Management.Validations.EventValidations
{
    public class EventUpdateDtoValidator : AbstractValidator<EventUpdateDto>
    {
        public EventUpdateDtoValidator() 
        {
            RuleFor(e => e.Title)
                .MaximumLength(100).WithMessage("Title must not exceed 100 characters.");

            RuleFor(e => e.Description)
                .MaximumLength(500).WithMessage("Description must not exceed 500 characters.");

            RuleFor(e => e.StartDate)
                .GreaterThan(DateTime.UtcNow).WithMessage("Start date must be in the future.");

            RuleFor(e => e.EndDate)
                .GreaterThan(e => e.StartDate).WithMessage("End date must be later than the start date.");

            RuleFor(e => e.Capacity)
                .GreaterThan(0).WithMessage("Capacity must be greater than zero.");
        }
    }
}
