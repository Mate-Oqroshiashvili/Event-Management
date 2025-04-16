using Event_Management.Models.Dtos.OrganizerDtos;
using FluentValidation;

namespace Event_Management.Validations.OrganizerValidations
{
    public class OrganizerCreateDtoValidator : AbstractValidator<OrganizerCreateDto>
    {
        private readonly string[] _allowedFileExtensions = { ".jpg", ".jpeg", ".png", ".gif" };
        private const int _maxFileSizeInMB = 5;

        public OrganizerCreateDtoValidator()
        {
            RuleFor(o => o.Description)
                .NotEmpty().WithMessage("Description is required.")
                .MaximumLength(500).WithMessage("Description must not exceed 500 characters.")
                .Must(desc =>
                    string.IsNullOrWhiteSpace(desc) ||
                    !(desc.Contains("spam", StringComparison.OrdinalIgnoreCase) ||
                      desc.Contains("free", StringComparison.OrdinalIgnoreCase) ||
                      desc.Contains("discount", StringComparison.OrdinalIgnoreCase)))
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

            RuleFor(o => o.UserId)
                .GreaterThan(0).WithMessage("User ID must be a positive integer.");
        }
    }
}
