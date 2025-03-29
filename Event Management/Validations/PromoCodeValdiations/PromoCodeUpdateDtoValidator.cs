using Event_Management.Models.Dtos.PromoCodeDtos;
using Event_Management.Models.Enums;
using FluentValidation;

namespace Event_Management.Validations.PromoCodeValdiations
{
    public class PromoCodeUpdateDtoValidator : AbstractValidator<PromoCodeUpdateDto>
    {
        public PromoCodeUpdateDtoValidator()
        {
            RuleFor(x => x.PromoCodeAmount)
                .GreaterThan(0).WithMessage("PromoCodeAmount must be greater than zero.");
        }
    }
}
