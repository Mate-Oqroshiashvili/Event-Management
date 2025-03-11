using Event_Management.Models.Dtos.TicketDtos;
using FluentValidation;

namespace Event_Management.Validations.TicketValidations
{
    public class TicketUpdateDtoValidator : AbstractValidator<TicketUpdateDto>
    {
        public TicketUpdateDtoValidator()
        {
            RuleFor(x => x.Type)
                .NotEmpty().WithMessage("Type is required.")
                .MaximumLength(100).WithMessage("Type cannot exceed 100 characters.")
                .Matches(@"^[a-zA-Z0-9\s]+$").WithMessage("Type must contain only letters and numbers.");

            RuleFor(x => x.Price)
                .GreaterThan(0).WithMessage("Price must be greater than 0.")
                .LessThanOrEqualTo(10000).WithMessage("Price cannot exceed 10,000.");

            RuleFor(x => x.Quantity)
                .GreaterThan(0).WithMessage("Quantity must be greater than 0.");

            RuleFor(x => x.Status)
                .IsInEnum().WithMessage("Invalid ticket status.");

            RuleFor(x => x.QRCodeUrl)
                .NotEmpty().WithMessage("QR Code URL is required.")
                .Must(url => Uri.TryCreate(url, UriKind.Absolute, out _)).WithMessage("Invalid QR Code URL.")
                .Matches(@"^https?:\/\/.*$").WithMessage("QR Code URL must start with http:// or https://.");
        }
    }
}
