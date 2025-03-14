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
                .MaximumLength(50).WithMessage("PromoCodeText must not exceed 50 characters.");

            RuleFor(x => x.PromoCodeStatus)
                .Must(promoCodeStatus => Enum.IsDefined(typeof(PromoCodeStatus), promoCodeStatus))
                .WithMessage("Invalid promo code status.");
        }
    }
}
