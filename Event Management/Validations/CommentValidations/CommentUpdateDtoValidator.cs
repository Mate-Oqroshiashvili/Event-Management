using Event_Management.Models.Dtos.CommentDtos;
using FluentValidation;

namespace Event_Management.Validations.CommentValidations
{
    public class CommentUpdateDtoValidator : AbstractValidator<CommentUpdateDto>
    {
        public CommentUpdateDtoValidator() 
        {
            RuleFor(x => x.CommentContent)
                .NotEmpty().WithMessage("Comment content is required.")
                .MaximumLength(500).WithMessage("Comment content must not exceed 500 characters.");
        }
    }
}
