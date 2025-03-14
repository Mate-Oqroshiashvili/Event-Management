using Event_Management.Models.Dtos.PromoCodeDtos;
using FluentValidation;

namespace Event_Management.Validations.PromoCodeValdiations
{
    public class PromoCodeCreateDtoValidator : AbstractValidator<PromoCodeCreateDto>
    {
        public PromoCodeCreateDtoValidator()
        {
            RuleFor(x => x.EventId)
                .GreaterThan(0).WithMessage("EventId must be a positive integer.");

            RuleFor(x => x.PromoCodeText)
                .NotEmpty().WithMessage("PromoCodeText is required.")
                .MaximumLength(50).WithMessage("PromoCodeText must not exceed 50 characters.");

            RuleFor(x => x.PromoCodeAmount)
                .GreaterThan(0).WithMessage("PromoCodeAmount must be greater than zero.");
        }
    }
}
