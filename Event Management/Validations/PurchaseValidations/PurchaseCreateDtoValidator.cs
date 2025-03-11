using Event_Management.Models.Dtos.PurchaseDtos;
using FluentValidation;

namespace Event_Management.Validations.PurchaseValidations
{
    public class PurchaseCreateDtoValidator : AbstractValidator<PurchaseCreateDto>
    {
        public PurchaseCreateDtoValidator()
        {
            RuleFor(x => x.TicketIds)
                .NotEmpty().WithMessage("At least one ticket must be selected.")
                .Must(ticketIds => ticketIds.All(id => id > 0))
                .WithMessage("All ticket IDs must be greater than zero.")
                .Must(ticketIds => ticketIds.Distinct().Count() == ticketIds.Count)
                .WithMessage("Duplicate ticket IDs are not allowed.");

            RuleFor(p => p.UserId)
                .GreaterThan(0).WithMessage("User ID must be a positive integer.");

            RuleFor(p => p.Quantity)
                .GreaterThan(0).WithMessage("Quantity must be at least 1.");

            RuleFor(p => p.TotalAmount)
                .GreaterThan(0).WithMessage("Total amount must be greater than zero.");

            RuleFor(p => p.PurchaseDate)
                .NotEmpty().WithMessage("Purchase date is required.")
                .LessThanOrEqualTo(DateTime.UtcNow).WithMessage("Purchase date cannot be in the future.");

            RuleFor(p => p.Status)
                .IsInEnum().WithMessage("Invalid purchase status.");
        }
    }
}
