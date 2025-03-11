using Event_Management.Models.Dtos.LocationDtos;
using FluentValidation;

namespace Event_Management.Validations.LocationValidations
{
    public class LocationCreateDtoValidator : AbstractValidator<LocationCreateDto>
    {
        private readonly string[] _allowedFileExtensions = { ".jpg", ".jpeg", ".png", ".gif" };
        private const int _maxFileSizeInMB = 5;

        public LocationCreateDtoValidator() 
        {
            RuleFor(l => l.Name)
                .NotEmpty().WithMessage("Location name is required.")
                .MaximumLength(100).WithMessage("Location name must not exceed 100 characters.")
                .Must(name => name.Trim() == name).WithMessage("Location name should not have leading or trailing spaces.");

            RuleFor(l => l.Address)
                .NotEmpty().WithMessage("Address is required.")
                .MaximumLength(200).WithMessage("Address must not exceed 200 characters.");

            RuleFor(l => l.City)
                .NotEmpty().WithMessage("City is required.")
                .MaximumLength(100).WithMessage("City must not exceed 100 characters.");

            RuleFor(l => l.State)
                .NotEmpty().WithMessage("State is required.")
                .MaximumLength(100).WithMessage("State must not exceed 100 characters.")
                .Must((dto, state) => IsValidState(dto.Country, state))
                .WithMessage("Invalid state for the selected country.");

            RuleFor(l => l.Country)
                .NotEmpty().WithMessage("Country is required.")
                .MaximumLength(100).WithMessage("Country must not exceed 100 characters.");

            RuleFor(l => l.PostalCode)
                .NotEmpty().WithMessage("Postal code is required.")
                .Matches("^[0-9A-Za-z -]+$").WithMessage("Invalid postal code format.")
                .MaximumLength(20).WithMessage("Postal code must not exceed 20 characters.")
                .When(l => l.Country == "US", ApplyConditionTo.CurrentValidator)
                .Matches(@"^\d{5}(-\d{4})?$").WithMessage("Invalid US postal code format.")
                .When(l => l.Country == "CA", ApplyConditionTo.CurrentValidator)
                .Matches(@"^[A-Za-z]\d[A-Za-z] ?\d[A-Za-z]\d$").WithMessage("Invalid Canadian postal code format.");

            RuleFor(l => l.Capacity)
                .GreaterThan(0).WithMessage("Capacity must be greater than zero.")
                .LessThan(100000).WithMessage("Capacity must be realistic (less than 100,000).");

            RuleFor(l => l.Description)
                .MaximumLength(500).WithMessage("Description must not exceed 500 characters.")
                .Must(desc => !desc.Contains("free") && !desc.Contains("discount") && !desc.Contains("spam"))
                .WithMessage("Description contains forbidden words.");

            RuleFor(l => l.Image)
                .Must(file => file == null || _allowedFileExtensions.Contains(Path.GetExtension(file.FileName).ToLower()))
                .WithMessage("Invalid file type. Only JPG, JPEG, PNG, and GIF are allowed.")
                .Must(file => file == null || file.Length <= _maxFileSizeInMB * 1024 * 1024)
                .WithMessage($"File size cannot exceed {_maxFileSizeInMB} MB.");
        }

        private bool IsValidState(string country, string state)
        {
            var validStates = new Dictionary<string, List<string>>()
            {
                { "US", new List<string> { "California", "New York", "Texas", "Florida", "Illinois" } },
                { "CA", new List<string> { "Ontario", "Quebec", "British Columbia", "Alberta", "Manitoba" } }
            };

            return string.IsNullOrEmpty(state) || (validStates.ContainsKey(country) && validStates[country].Contains(state));
        }
    }
}
