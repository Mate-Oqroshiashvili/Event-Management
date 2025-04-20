using Event_Management.Models.Dtos.LocationDtos;
using FluentValidation;

namespace Event_Management.Validations.LocationValidations
{
    public class LocationCreateDtoValidator : AbstractValidator<LocationCreateDto>
    {
        private readonly string[] _allowedFileExtensions = { ".jpg", ".jpeg", ".png", ".gif", ".webp" };
        private const int _maxFileSizeInMB = 5;

        public LocationCreateDtoValidator() 
        {
            RuleFor(l => l.Name)
               .NotEmpty().WithMessage("Location name is required.")
               .MaximumLength(100).WithMessage("Location name must not exceed 100 characters.")
               .Must(name => name == null || name.Trim() == name)
               .WithMessage("Location name should not have leading or trailing spaces.");

            RuleFor(l => l.Address)
                .NotEmpty().WithMessage("Address is required.")
                .MaximumLength(200).WithMessage("Address must not exceed 200 characters.");

            RuleFor(l => l.City)
                .NotEmpty().WithMessage("City is required.")
                .MaximumLength(100).WithMessage("City must not exceed 100 characters.");

            RuleFor(l => l.State)
                .NotEmpty().WithMessage("State is required.")
                .MaximumLength(100).WithMessage("State must not exceed 100 characters.");

            RuleFor(l => l.Country)
                .NotEmpty().WithMessage("Country is required.")
                .MaximumLength(100).WithMessage("Country must not exceed 100 characters.");

            RuleFor(l => l.PostalCode)
                .NotEmpty().WithMessage("Postal code is required.")
                .Matches("^[0-9A-Za-z -]+$").WithMessage("Invalid postal code format.")
                .MaximumLength(20).WithMessage("Postal code must not exceed 20 characters.");

            RuleFor(l => l.MaxCapacity)
                .GreaterThan(0).WithMessage("Maximum capacity must be greater than zero.")
                .LessThan(20000).WithMessage("Maximum capacity must be realistic (less than 20,000).");

            RuleFor(l => l.AvailableStaff)
                .GreaterThan(0).WithMessage("Number of available Staff must be greater than zero.")
                .LessThan(500).WithMessage("Available staff count must be realistic (less than 500).");

            RuleFor(l => l.Description)
                .NotEmpty().WithMessage("Description is required!")
                .MaximumLength(500).WithMessage("Description must not exceed 500 characters.");

            RuleFor(l => l.Description)
                .Must(desc => !desc.Contains("free") && !desc.Contains("discount") && !desc.Contains("spam"))
                .WithMessage("Description contains forbidden words.")
                .When(l => !string.IsNullOrWhiteSpace(l.Description));

            RuleFor(l => l.Image)
                .Must(file => file == null || _allowedFileExtensions.Contains(Path.GetExtension(file.FileName).ToLower()))
                .WithMessage("Invalid file type. Only JPG, JPEG, PNG, GIF and WEBP are allowed.")
                .Must(file => file == null || file.Length <= _maxFileSizeInMB * 1024 * 1024)
                .WithMessage($"File size cannot exceed {_maxFileSizeInMB} MB.");
        }
    }
}
