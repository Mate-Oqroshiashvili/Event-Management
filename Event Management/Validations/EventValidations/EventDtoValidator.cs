using Event_Management.Models.Dtos.EventDtos;
using FluentValidation;
using System.Text.RegularExpressions;

namespace Event_Management.Validations.EventValidations
{
    public class EventDtoValidator : AbstractValidator<EventDto>
    {
        public EventDtoValidator() 
        {
            RuleFor(e => e.Id)
                .GreaterThan(0).WithMessage("Event ID must be a positive integer.");

            RuleFor(e => e.Title)
                .NotEmpty().WithMessage("Title is required.")
                .MaximumLength(100).WithMessage("Title must not exceed 100 characters.");

            RuleFor(e => e.Description)
                .NotEmpty().WithMessage("Description is required.")
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

            RuleFor(e => e.Location)
                .NotNull().WithMessage("Location is required.");

            RuleFor(e => e.Organizer)
                .NotNull().WithMessage("Organizer is required.");

            RuleForEach(x => x.Images)
                .NotEmpty().WithMessage("Image URL cannot be empty.")
                .Must(BeAValidImageUrl).WithMessage("Invalid image URL format.");
        }

        private bool BeAValidImageUrl(string url)
        {
            var imageUrlPattern = @"^(https?:\/\/.*\.(?:png|jpg|jpeg|gif))$";
            return Regex.IsMatch(url, imageUrlPattern, RegexOptions.IgnoreCase);
        }
    }
}
