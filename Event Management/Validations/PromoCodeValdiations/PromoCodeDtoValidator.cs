using Event_Management.Models.Dtos.PromoCodeDtos;
using Event_Management.Models.Enums;
using FluentValidation;

namespace Event_Management.Validations.PromoCodeValdiations
{
    public class PromoCodeDtoValidator : AbstractValidator<PromoCodeDto>
    {
        public PromoCodeDtoValidator() 
        {
            RuleFor(x => x.Id)
                .GreaterThan(0).WithMessage("Id must be a positive integer.");

            RuleFor(x => x.EventId)
                .GreaterThan(0).WithMessage("EventId must be a positive integer.");

            RuleFor(x => x.PromoCodeText)
                .NotEmpty().WithMessage("PromoCodeText is required.")
                .Matches(@"^[a-zA-Z0-9]{8}$")
                .WithMessage("Promo code must be exactly 8 characters long, containing only English letters and numbers.");

            RuleFor(x => x.PromoCodeStatus)
                .Must(promoCodeStatus => Enum.IsDefined(typeof(PromoCodeStatus), promoCodeStatus))
                .WithMessage("Invalid promo code status.");
        }
    }
}
