using FluentValidation;
using UserApi.Models;

namespace UserApi.Validators
{
    public class UserValidator : AbstractValidator<User>
    {
        public UserValidator()
        {
            RuleFor(user => user.FirstName).NotEmpty().MaximumLength(128);
            RuleFor(user => user.LastName).MaximumLength(128);
            RuleFor(user => user.Email).NotEmpty().EmailAddress();
            RuleFor(user => user.DateOfBirth).NotEmpty().Must(BeAtLeast18YearsOld).WithMessage("User must be at least 18 years old.");
            RuleFor(user => user.PhoneNumber).NotEmpty().Matches(@"^\d{10}$").WithMessage("Phone number must be 10 digits.");
        }

        private bool BeAtLeast18YearsOld(DateTime dateOfBirth)
        {
            return dateOfBirth <= DateTime.Now.AddYears(-18);
        }
    }
}
