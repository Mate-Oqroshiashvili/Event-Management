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
                .Matches(@"^[a-zA-Z0-9]{8}$")
                .WithMessage("Promo code must be exactly 8 characters long, containing only English letters and numbers.");

            RuleFor(x => x.PromoCodeAmount)
                .GreaterThan(0).WithMessage("PromoCodeAmount must be greater than zero.");
        }
    }
}
