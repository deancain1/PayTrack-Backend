using FluentValidation;

namespace PayTrack.Application.Features.Auth.Commands
{
    public class RegisterCommandValidator
        : AbstractValidator<RegisterCommand>
    {
        public RegisterCommandValidator()
        {
            RuleFor(x => x.FullName)
                .NotEmpty()
                .WithMessage("Full name is required.");

            RuleFor(x => x.Email)
                .NotEmpty()
                .WithMessage("Email is required.")
                .EmailAddress()
                .WithMessage("Invalid email format.")
                .Must(email => email.EndsWith("@gmail.com"))
                .WithMessage("Only Gmail addresses are allowed.");

            // Password is required ONLY for normal registration
            When(x => !x.IsGoogleUser, () =>
            {
                RuleFor(x => x.Password)
                    .NotEmpty()
                    .WithMessage("Password is required.")
                    .MinimumLength(8)
                    .WithMessage(
                        "Password must be at least 8 characters long."
                    );
            });

            // Google users must have GoogleProviderKey
            When(x => x.IsGoogleUser, () =>
            {
                RuleFor(x => x.GoogleProviderKey)
                    .NotEmpty()
                    .WithMessage(
                        "Google provider key is required."
                    );
            });

            RuleFor(x => x.Role)
                .NotEmpty()
                .WithMessage("Role is required.")
                .Must(role =>
                    role == "Employee" ||
                    role == "CompanyAdmin" ||
                    role == "Admin"
                )
                .WithMessage(
                    "Role must be either 'Employee', 'CompanyAdmin', or 'Admin'."
                );
        }
    }
}