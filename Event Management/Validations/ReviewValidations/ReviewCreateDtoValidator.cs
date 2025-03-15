using Event_Management.Models.Dtos.ReviewDtos;
using FluentValidation;

namespace Event_Management.Validations.ReviewValidations
{
    public class ReviewCreateDtoValidator : AbstractValidator<ReviewCreateDto>
    {
        public ReviewCreateDtoValidator() 
        {
            RuleFor(x => x.StarCount)
                .InclusiveBetween(1, 5).WithMessage("Star count must be between 1 and 5.")
                .NotEmpty().WithMessage("Star count is required.");

            RuleFor(x => x.UserId)
                .GreaterThan(0).WithMessage("UserId is required and must be a positive number.");

            RuleFor(x => x.EventId)
                .GreaterThan(0).WithMessage("EventId is required and must be a positive number.");
        }
    }
}
