using MediatR;
using Microsoft.AspNetCore.Identity;
using PayTrack.Domain.Entities;

namespace PayTrack.Application.Features.Employee.Commands
{
    public class SetEmployeePasswordCommandHandler
        : IRequestHandler<
            SetEmployeePasswordCommand,
            string>
    {
        private readonly UserManager<ApplicationUser> _userManager;

        public SetEmployeePasswordCommandHandler(
            UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }

        public async Task<string> Handle(
            SetEmployeePasswordCommand request,
            CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(request.UserId))
            {
                throw new Exception("User ID is required.");
            }

            if (string.IsNullOrWhiteSpace(request.Token))
            {
                throw new Exception("Password setup token is required.");
            }

            if (string.IsNullOrWhiteSpace(request.NewPassword))
            {
                throw new Exception("Password is required.");
            }

            // Find employee account
            var user = await _userManager.FindByIdAsync(
                request.UserId);

            if (user == null)
            {
                throw new Exception("User account not found.");
            }

            // Email must already be verified
            if (!user.IsEmailVerified ||
                !user.EmailConfirmed)
            {
                throw new Exception(
                    "Please verify your email address first.");
            }

            // Make sure this is an employee
            if (!string.Equals(
                    user.Role,
                    "Employee",
                    StringComparison.OrdinalIgnoreCase))
            {
                throw new Exception(
                    "Password setup is only available for employees.");
            }

            // Don't allow this endpoint to replace an existing password
            var hasPassword =
                await _userManager.HasPasswordAsync(user);

            if (hasPassword)
            {
                throw new Exception(
                    "Your password has already been set. Please log in.");
            }

            // Use ASP.NET Identity's built-in reset token
            var result =
                await _userManager.ResetPasswordAsync(
                    user,
                    request.Token,
                    request.NewPassword);

            if (!result.Succeeded)
            {
                throw new Exception(
                    string.Join(
                        ", ",
                        result.Errors.Select(
                            x => x.Description)));
            }

            return "Password created successfully. You can now log in.";
        }
    }
}