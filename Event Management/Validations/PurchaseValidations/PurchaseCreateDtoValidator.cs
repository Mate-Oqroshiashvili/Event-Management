using Event_Management.Models.Dtos.PurchaseDtos;
using FluentValidation;

namespace Event_Management.Validations.PurchaseValidations
{
    public class PurchaseCreateDtoValidator : AbstractValidator<PurchaseCreateDto>
    {
        public PurchaseCreateDtoValidator()
        {
            RuleFor(x => x.Tickets)
                .NotEmpty().WithMessage("At least one ticket must be selected.")
                .Must(ticketIds => ticketIds.All(t => t.TicketId > 0))
                .WithMessage("All ticket IDs must be greater than zero.");

            RuleFor(p => p.UserId)
                .GreaterThan(0).WithMessage("User ID must be a positive integer.");

            RuleFor(p => p.PromoCodeText)
                .Matches(@"^[a-zA-Z0-9]{8}$")
                .WithMessage("Promo code must be exactly 8 characters long, containing only English letters and numbers.");

            RuleFor(p => p.Quantity)
                .GreaterThan(0).WithMessage("Quantity must be at least 1.");

            RuleFor(p => p.PurchaseDate)
                .NotEmpty().WithMessage("Purchase date is required.")
                .LessThanOrEqualTo(DateTime.UtcNow).WithMessage("Purchase date cannot be in the future.");

            RuleFor(p => p.Status)
                .IsInEnum().WithMessage("Invalid purchase status.");
        }
    }
}
