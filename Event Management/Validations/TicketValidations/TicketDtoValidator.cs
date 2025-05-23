﻿using Event_Management.Models.Dtos.TicketDtos;
using Event_Management.Models.Enums;
using FluentValidation;

namespace Event_Management.Validations.TicketValidations
{
    public class TicketDtoValidator : AbstractValidator<TicketDto>
    {
        public TicketDtoValidator()
        {
            RuleFor(x => x.Id)
                .GreaterThan(0).WithMessage("Id must be greater than 0.");

            RuleFor(x => x.Type)
                .Must(ticketType => Enum.IsDefined(typeof(TicketType), ticketType))
                .WithMessage("Invalid ticket type.");

            RuleFor(x => x.Price)
                .GreaterThan(0).WithMessage("Price must be greater than 0.")
                .LessThanOrEqualTo(10000).WithMessage("Price cannot exceed 10,000.");

            RuleFor(x => x.Quantity)
                .GreaterThan(0).WithMessage("Quantity must be greater than 0.");

            RuleFor(x => x.Status)
                .IsInEnum().WithMessage("Invalid ticket status.");

            RuleFor(x => x.QRCodeData)
                .NotEmpty().WithMessage("QRCodeData cannot be empty.")
                .Matches(@"^[a-zA-Z0-9\-]+$").WithMessage("QRCodeData can only contain alphanumeric characters and dashes.")  // You can adjust this pattern based on your QR code format
                .Length(10, 100).WithMessage("QRCodeData must be between 10 and 100 characters long."); // Adjust the length as per your needs

            RuleFor(x => x.QRCodeImageUrl)
                .NotEmpty().WithMessage("QRCodeImageUrl cannot be empty.")
                .Must(BeAValidUrl).WithMessage("QRCodeImageUrl must be a valid URL.");

            RuleFor(x => x.Event)
                .NotNull().WithMessage("Event information is required.");
        }

        private bool BeAValidUrl(string url)
        {
            Uri uriResult;
            return Uri.TryCreate(url, UriKind.Absolute, out uriResult!)
                && (uriResult.Scheme == Uri.UriSchemeHttp || uriResult.Scheme == Uri.UriSchemeHttps);  // Ensure the URL is either http or https
        }
    }
}
