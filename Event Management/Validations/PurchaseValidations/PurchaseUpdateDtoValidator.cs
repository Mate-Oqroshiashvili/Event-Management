using Event_Management.Models.Dtos.PurchaseDtos;
using FluentValidation;

namespace Event_Management.Validations.PurchaseValidations
{
    public class PurchaseUpdateDtoValidator : AbstractValidator<PurchaseUpdateDto>
    {
        public PurchaseUpdateDtoValidator()
        {
            RuleFor(x => x.TicketIds)
                .NotEmpty().WithMessage("At least one ticket must be selected.")
                .Must(ticketIds => ticketIds.All(id => id > 0))
                .WithMessage("All ticket IDs must be greater than zero.")
                .Must(ticketIds => ticketIds.Distinct().Count() == ticketIds.Count)
                .WithMessage("Duplicate ticket IDs are not allowed.");

            RuleFor(p => p.Status)
                .IsInEnum().WithMessage("Invalid purchase status.");
        }
    }
}
