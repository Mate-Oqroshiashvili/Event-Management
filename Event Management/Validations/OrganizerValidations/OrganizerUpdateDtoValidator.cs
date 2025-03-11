using Event_Management.Models.Dtos.OrganizerDtos;
using FluentValidation;

namespace Event_Management.Validations.OrganizerValidations
{
    public class OrganizerUpdateDtoValidator : AbstractValidator<OrganizerUpdateDto>
    {
        private readonly string[] _allowedFileExtensions = { ".jpg", ".jpeg", ".png", ".gif" };
        private const int _maxFileSizeInMB = 5;

        public OrganizerUpdateDtoValidator()
        {
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

            RuleFor(o => o.Logo)
                .Must(file => file == null || _allowedFileExtensions.Contains(Path.GetExtension(file.FileName).ToLower()))
                .WithMessage("Invalid file type. Only JPG, JPEG, PNG, and GIF are allowed.")
                .Must(file => file == null || file.Length <= _maxFileSizeInMB * 1024 * 1024)
                .WithMessage($"File size cannot exceed {_maxFileSizeInMB} MB.");

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
