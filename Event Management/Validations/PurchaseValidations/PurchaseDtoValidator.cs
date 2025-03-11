using Event_Management.Models.Dtos.PurchaseDtos;
using FluentValidation;

namespace Event_Management.Validations.PurchaseValidations
{
    public class PurchaseDtoValidator : AbstractValidator<PurchaseDto>
    {
        public PurchaseDtoValidator()
        {
            RuleFor(p => p.Id)
                .GreaterThan(0).WithMessage("Purchase ID must be a positive integer.");

            RuleFor(p => p.Quantity)
                .GreaterThan(0).WithMessage("Quantity must be at least 1.");

            RuleFor(p => p.TotalAmount)
                .GreaterThan(0).WithMessage("Total amount must be greater than zero.");

            RuleFor(p => p.PurchaseDate)
                .NotEmpty().WithMessage("Purchase date is required.")
                .LessThanOrEqualTo(DateTime.UtcNow).WithMessage("Purchase date cannot be in the future.");
        }
    }
}
