using Event_Management.Models.Dtos.EventDtos;
using FluentValidation;

namespace Event_Management.Validations.EventValidations
{
    public class EventUpdateDtoValidator : AbstractValidator<EventUpdateDto>
    {
        public EventUpdateDtoValidator() 
        {
            RuleFor(e => e.Title)
                .MaximumLength(100).WithMessage("Title must not exceed 100 characters.");

            RuleFor(e => e.Description)
                .MaximumLength(500).WithMessage("Description must not exceed 500 characters.");

            RuleFor(e => e.StartDate)
                .GreaterThan(DateTime.UtcNow).WithMessage("Start date must be in the future.");

            RuleFor(e => e.EndDate)
                .GreaterThan(e => e.StartDate).WithMessage("End date must be later than the start date.");

            RuleFor(e => e.Capacity)
                .GreaterThan(0).WithMessage("Capacity must be greater than zero.");

            RuleFor(e => e.Status)
                .IsInEnum().WithMessage("Invalid event status.");

            RuleFor(e => e.BookedStaff)
                .GreaterThan(0).WithMessage("Booked staff must be greater than zero.");

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
    }
}
