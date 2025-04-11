using Event_Management.Models.Dtos.UserDtos;
using Event_Management.Models.Enums;
using FluentValidation;

namespace Event_Management.Validations.UserValidations
{
    public class UserUpdateDtoValidator : AbstractValidator<UserUpdateDto>
    {
        private readonly string[] _allowedFileExtensions = { ".jpg", ".jpeg", ".png", ".gif" };
        private const int _maxFileSizeInMB = 5;

        public UserUpdateDtoValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Name is required.")
                .MaximumLength(100).WithMessage("Name cannot exceed 100 characters.")
                .Matches(@"^[a-zA-Z\s]+$").WithMessage("Name can only contain alphabets and spaces."); ;

            RuleFor(x => x.Email)
                .NotEmpty().WithMessage("Email is required.")
                .EmailAddress().WithMessage("Invalid email format.");

            RuleFor(x => x.PhoneNumber)
                .NotEmpty().WithMessage("Phone number is required.")
                .Matches(@"^\+?[1-9]\d{1,14}$").WithMessage("Invalid phone number format.");

            RuleFor(x => x.ProfilePicture)
                .Must(file => file == null || _allowedFileExtensions.Contains(Path.GetExtension(file.FileName).ToLower()))
                .WithMessage("Invalid file type. Only JPG, JPEG, PNG, and GIF are allowed.")
                .Must(file => file == null || file.Length <= _maxFileSizeInMB * 1024 * 1024)
                .WithMessage($"File size cannot exceed {_maxFileSizeInMB} MB.");
        }
    }
}
