using Event_Management.Models.Dtos.EventDtos;
using FluentValidation;

namespace Event_Management.Validations.EventValidations
{
    public class EventCreateDtoValidator : AbstractValidator<EventCreateDto>
    {
        private readonly List<string> _blacklistedWords = new() { "spam", "fake", "test" };
        private readonly int _maxEventYears = 5;
        private readonly int _maxCapacity = 10000;

        public EventCreateDtoValidator()
        {
            RuleFor(e => e.Title)
                .NotEmpty().WithMessage("Title is required.")
                .MaximumLength(100).WithMessage("Title must not exceed 100 characters.")
                .Must(NotContainBlacklistedWords).WithMessage("Title contains inappropriate words.")
                .Matches(@"^[a-zA-Z0-9\s\-_,.!]+$").WithMessage("Title contains invalid characters.");

            RuleFor(e => e.Description)
                .NotEmpty().WithMessage("Description is required.")
                .MaximumLength(500).WithMessage("Description must not exceed 500 characters.")
                .Matches(@"^[a-zA-Z0-9\s\-_,.!]+$").WithMessage("Description contains invalid characters.");

            RuleFor(e => e.StartDate)
                .GreaterThan(DateTime.UtcNow).WithMessage("Start date must be in the future.")
                .LessThan(DateTime.UtcNow.AddYears(_maxEventYears)).WithMessage($"Events cannot be scheduled more than {_maxEventYears} years ahead.");

            RuleFor(e => e.EndDate)
                .GreaterThan(e => e.StartDate).WithMessage("End date must be later than the start date.");

            RuleFor(e => e.Capacity)
                .GreaterThan(0).WithMessage("Capacity must be greater than zero.")
                .LessThanOrEqualTo(_maxCapacity).WithMessage($"Capacity must not exceed {_maxCapacity} attendees.");

            RuleFor(e => e.LocationId)
                .GreaterThan(0).WithMessage("Location ID must be a positive integer.");

            RuleFor(e => e.OrganizerId)
                .GreaterThan(0).WithMessage("Organizer ID must be a positive integer.");

            RuleForEach(x => x.Images)
                .Must(file => file.Length > 0).WithMessage("Uploaded file is empty.")
                .Must(BeAValidFileType).WithMessage("Invalid file type. Only JPG, JPEG, PNG, and GIF are allowed.")
                .Must(BeAReasonableFileSize).WithMessage("File size must be less than 5 MB.");

            RuleFor(x => x.Images)
                .Must(images => images.Count() <= 10).WithMessage("You can upload a maximum of 10 images.");
        }

        private bool BeAValidFileType(IFormFile file)
        {
            var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif" };
            var fileExtension = Path.GetExtension(file.FileName).ToLower();
            return allowedExtensions.Contains(fileExtension);
        }

        private bool BeAReasonableFileSize(IFormFile file)
        {
            const long maxFileSize = 5 * 1024 * 1024;
            return file.Length <= maxFileSize;
        }

        private bool NotContainBlacklistedWords(string title)
        {
            if (string.IsNullOrWhiteSpace(title))
                return true;

            foreach (var word in _blacklistedWords)
            {
                if (title.ToLower().Contains(word))
                    return false;
            }
            return true;
        }
    }
}
