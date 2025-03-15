using Event_Management.Models.Dtos.CommentDtos;
using FluentValidation;

namespace Event_Management.Validations.CommentValidations
{
    public class CommentCreateDtoValidator : AbstractValidator<CommentCreateDto>
    {
        public CommentCreateDtoValidator()
        {
            RuleFor(x => x.CommentContent)
                .NotEmpty().WithMessage("Comment content is required.")
                .MaximumLength(500).WithMessage("Comment content must not exceed 500 characters.");

            RuleFor(x => x.UserId)
                .GreaterThan(0).WithMessage("UserId is required and must be a positive number.");

            RuleFor(x => x.EventId)
                .GreaterThan(0).WithMessage("EventId is required and must be a positive number.");
        }
    }
}
