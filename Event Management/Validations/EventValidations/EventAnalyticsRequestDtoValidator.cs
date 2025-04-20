using Event_Management.Models.Dtos.EventDtos;
using FluentValidation;

namespace Event_Management.Validations.EventValidations
{
    public class EventAnalyticsRequestDtoValidator : AbstractValidator<EventAnalyticsRequestDto>
    {
        public EventAnalyticsRequestDtoValidator()
        {
            RuleFor(x => x.OrganizerId)
                .GreaterThan(0).WithMessage("OrganizerId must be greater than 0.");

            RuleFor(x => x.EventId)
                .GreaterThan(0).WithMessage("EventId must be greater than 0.");
        }
    }
}
