using Event_Management.Models.Dtos.OrganizerDtos;
using FluentValidation;

namespace Event_Management.Validations.OrganizerValidations
{
    public class OrganizerDtoValidator : AbstractValidator<OrganizerDto>
    {
        public OrganizerDtoValidator()
        {
            RuleFor(o => o.Id)
                .GreaterThan(0).WithMessage("Organizer ID must be a positive integer.");

            RuleFor(o => o.Name)
                .NotEmpty().WithMessage("Organizer name is required.")
                .MaximumLength(100).WithMessage("Organizer name must not exceed 100 characters.")
                .Must(name => name.Trim() == name).WithMessage("Organizer name should not have leading or trailing spaces.");

            RuleFor(o => o.Email)
                .NotEmpty().WithMessage("Email is required.")
                .EmailAddress().WithMessage("Invalid email format.")
                .Must(email => !email.EndsWith("@tempmail.com") && !email.EndsWith("@trashmail.com"))
                .WithMessage("Temporary email addresses are not allowed.");

            RuleFor(o => o.PhoneNumber)
                .NotEmpty().WithMessage("Phone number is required.")
                .Matches(@"^\+?[1-9]\d{6,14}$").WithMessage("Invalid phone number format. Must include country code.");

            RuleFor(o => o.Description)
                .MaximumLength(500).WithMessage("Description must not exceed 500 characters.")
                .Must(desc => !desc.Contains("spam") && !desc.Contains("free") && !desc.Contains("discount"))
                .WithMessage("Description contains forbidden words.");

            RuleFor(o => o.LogoUrl)
                .Must(url => string.IsNullOrEmpty(url) ||
                             (Uri.IsWellFormedUriString(url, UriKind.Absolute) &&
                              (url.EndsWith(".jpg") || url.EndsWith(".jpeg") || url.EndsWith(".png") || url.EndsWith(".gif"))))
                .WithMessage("Invalid logo URL format. Allowed formats: .jpg, .jpeg, .png, .gif.");

            RuleFor(o => o.Address)
                .MaximumLength(200).WithMessage("Address must not exceed 200 characters.")
                .Matches(@"^[A-Za-z0-9\s,.-]+$").WithMessage("Address contains invalid characters.");

            RuleFor(o => o.City)
                .MaximumLength(100).WithMessage("City must not exceed 100 characters.")
                .Matches(@"^[A-Za-z\s-]+$").WithMessage("City contains invalid characters.");

            RuleFor(o => o.Country)
                .MaximumLength(100).WithMessage("Country must not exceed 100 characters.")
                .Matches(@"^[A-Za-z\s-]+$").WithMessage("Country contains invalid characters.");
        }
    }
}
