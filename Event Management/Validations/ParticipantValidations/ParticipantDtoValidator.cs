using Event_Management.Models.Dtos.ParticipantDtos;
using FluentValidation;

namespace Event_Management.Validations.ParticipantValidations
{
    public class ParticipantDtoValidator : AbstractValidator<ParticipantDto>
    {
        public ParticipantDtoValidator()
        {
            RuleFor(p => p.Id)
                .GreaterThan(0).WithMessage("Participant ID must be a positive integer.");

            RuleFor(p => p.RegistrationDate)
                .NotEmpty().WithMessage("Registration date is required.")
                .LessThanOrEqualTo(DateTime.UtcNow).WithMessage("Registration date cannot be in the future.");
        }
    }
}
