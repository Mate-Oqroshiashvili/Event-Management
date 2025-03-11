using Event_Management.Models.Dtos.PurchaseDtos;
using FluentValidation;

namespace Event_Management.Validations.PurchaseValidations
{
    public class PurchaseUpdateDtoValidator : AbstractValidator<PurchaseUpdateDto>
    {
        public PurchaseUpdateDtoValidator()
        {
            RuleFor(p => p.Status)
                .IsInEnum().WithMessage("Invalid purchase status.");
        }
    }
}
