using Event_Management.Models.Dtos.ParticipantDtos;
using FluentValidation;

namespace Event_Management.Validations.ParticipantValidations
{
    public class ParticipantUpdateDtoValidator : AbstractValidator<ParticipantUpdateDto>
    {
        public ParticipantUpdateDtoValidator()
        {
            RuleFor(p => p.Attendance)
                .NotNull().WithMessage("Attendance status must be specified.");
        }
    }
}
