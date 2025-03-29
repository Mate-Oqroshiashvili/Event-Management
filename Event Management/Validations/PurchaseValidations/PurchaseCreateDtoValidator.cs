using Event_Management.Models.Dtos.PurchaseDtos;
using FluentValidation;

namespace Event_Management.Validations.PurchaseValidations
{
    public class PurchaseCreateDtoValidator : AbstractValidator<PurchaseCreateDto>
    {
        public PurchaseCreateDtoValidator()
        {
            RuleFor(x => x.Tickets)
                .NotNull().WithMessage("Tickets list cannot be null.")
                .NotEmpty().WithMessage("At least one ticket must be selected.")
                .Must(tickets => tickets.All(t => t.TicketId > 0))
                .WithMessage("All ticket IDs must be greater than zero.");

            RuleFor(p => p.UserId)
                .GreaterThan(0).WithMessage("User ID must be a positive integer.");

            RuleFor(p => p.PromoCodeText)
                .Matches(@"^[a-zA-Z0-9]{8}$")
                .WithMessage("Promo code must be exactly 8 characters long, containing only English letters and numbers.");
        }
    }
}
