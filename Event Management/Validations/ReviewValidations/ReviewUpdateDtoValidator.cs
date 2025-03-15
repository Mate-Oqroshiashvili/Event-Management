using Event_Management.Models.Dtos.ReviewDtos;
using FluentValidation;

namespace Event_Management.Validations.ReviewValidations
{
    public class ReviewUpdateDtoValidator : AbstractValidator<ReviewUpdateDto>
    {
        public ReviewUpdateDtoValidator() 
        {
            RuleFor(x => x.StarCount)
                .InclusiveBetween(1, 5).WithMessage("Star count must be between 1 and 5.")
                .NotEmpty().WithMessage("Star count is required.");
        }
    }
}
