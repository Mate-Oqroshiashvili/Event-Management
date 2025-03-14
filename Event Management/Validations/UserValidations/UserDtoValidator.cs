using Event_Management.Models.Dtos.UserDtos;
using Event_Management.Models.Enums;
using FluentValidation;

namespace Event_Management.Validations.UserValidations
{
    public class UserDtoValidator : AbstractValidator<UserDto>
    {
        public UserDtoValidator()
        {
            RuleFor(x => x.Id)
                .GreaterThan(0).WithMessage("Id must be greater than 0.");

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

            RuleFor(x => x.ProfilePictureUrl)
                .Must(url => string.IsNullOrEmpty(url) || Uri.TryCreate(url, UriKind.Absolute, out _))
                .WithMessage("Invalid profile picture URL.");

            RuleFor(x => x.Role)
                .Must(role => Enum.IsDefined(typeof(Role), role))
                .WithMessage("Invalid role.");
            
            RuleFor(x => x.UserType)
                .Must(userType => Enum.IsDefined(typeof(UserType), userType))
                .WithMessage("Invalid user type.");

            RuleFor(x => x.EmailVerificationCode)
                .MaximumLength(10).WithMessage("Email verification code cannot exceed 10 characters.");

            RuleFor(x => x.SmsVerificationCode)
                .MaximumLength(10).WithMessage("SMS verification code cannot exceed 10 characters.");

            RuleFor(x => x.CodeExpiration)
                .Must(date => date > DateTime.UtcNow.AddMinutes(2))
                .WithMessage("Code expiration must be at least 2 minutes in the future.");

            RuleFor(x => x.Organizer)
                .NotNull().WithMessage("Organizer information is required.");

            RuleFor(x => x.Tickets)
                .NotNull().WithMessage("Tickets list cannot be null.");

            RuleFor(x => x.Purchases)
                .NotNull().WithMessage("Purchases list cannot be null.");

            RuleFor(x => x.Participants)
                .NotNull().WithMessage("Participants list cannot be null.");
        }
    }
}
