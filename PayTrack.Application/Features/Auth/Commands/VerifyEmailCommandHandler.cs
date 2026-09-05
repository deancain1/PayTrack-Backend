using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using PayTrack.Domain.Entities;

namespace PayTrack.Application.Features.Auth.Commands
{
    public class VerifyEmailCommandHandler
        : IRequestHandler<VerifyEmailCommand, string>
    {
        private readonly UserManager<ApplicationUser> _userManager;

        public VerifyEmailCommandHandler(
            UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }

        public async Task<string> Handle(VerifyEmailCommand request, CancellationToken cancellationToken)
        {
           
            var user = await _userManager.Users
                .SingleOrDefaultAsync(
                    x => x.EmailVerificationToken == request.Token,
                    cancellationToken);

            if (user == null)
            {
                throw new Exception(
                    "Invalid or expired verification token.");
            }

          
            user.IsEmailVerified = true;
            user.EmailConfirmed = true;

         
            user.EmailVerificationToken = null;

            var result =
                await _userManager.UpdateAsync(user);

            if (!result.Succeeded)
            {
                throw new Exception(
                    string.Join(
                        ", ",
                        result.Errors.Select(
                            e => e.Description)));
            }

            // Employee needs to create password
            if (string.Equals(
                    user.Role,
                    "Employee",
                    StringComparison.OrdinalIgnoreCase))
            {
                
                var passwordToken =
                    await _userManager
                        .GeneratePasswordResetTokenAsync(user);

                return
                    $"EMPLOYEE_PASSWORD_SETUP|{user.Id}|{Uri.EscapeDataString(passwordToken)}";
            }

            
            return "Email verified successfully";
        }
    }
}