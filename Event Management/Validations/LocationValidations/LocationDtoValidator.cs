using Event_Management.Models.Dtos.LocationDtos;
using FluentValidation;

namespace Event_Management.Validations.LocationValidations
{
    public class LocationDtoValidator : AbstractValidator<LocationDto>
    {
        public LocationDtoValidator() 
        {
            RuleFor(l => l.Id)
                .GreaterThan(0).WithMessage("Location ID must be a positive integer.");

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
                .LessThan(10000).WithMessage("Capacity must be realistic (less than 10,000).");

            RuleFor(l => l.AvailableStaff)
                .GreaterThan(0).WithMessage("Number of available Staff must be greater than zero.")
                .LessThan(500).WithMessage("Available staff count must be realistic (less than 500).");

            RuleFor(l => l.BookedStaff)
                .GreaterThan(0).WithMessage("Number of booked Staff must be greater than zero.")
                .LessThan(l => l.AvailableStaff + 1).WithMessage("Booked staff count must be realistic.");

            RuleFor(l => l.Description)
                .MaximumLength(500).WithMessage("Description must not exceed 500 characters.")
                .Must(desc => !desc.Contains("free") && !desc.Contains("discount") && !desc.Contains("spam"))
                .WithMessage("Description contains forbidden words.");

            RuleFor(l => l.ImageUrl)
                .Must(url => string.IsNullOrEmpty(url) ||
                             (Uri.IsWellFormedUriString(url, UriKind.Absolute) &&
                              (url.EndsWith(".jpg") || url.EndsWith(".jpeg") || url.EndsWith(".png") || url.EndsWith(".gif"))))
                .WithMessage("Invalid image URL format. Allowed formats: .jpg, .jpeg, .png, .gif.");
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
