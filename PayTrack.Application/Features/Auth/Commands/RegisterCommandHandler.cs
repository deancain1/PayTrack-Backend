using MediatR;
using Microsoft.AspNetCore.Identity;
using PayTrack.Application.DTOs;
using PayTrack.Application.Interfaces;
using PayTrack.Domain.Entities;

namespace PayTrack.Application.Features.Auth.Commands
{
    public class RegisterCommandHandler
        : IRequestHandler<RegisterCommand, AuthResponseDTO>
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IEmailService _emailService;

        public RegisterCommandHandler(
            UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole> roleManager,
            IEmailService emailService)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _emailService = emailService;
        }

        public async Task<AuthResponseDTO> Handle(
            RegisterCommand request,
            CancellationToken cancellationToken)
        {
            // Generate verification token
            var token = Guid.NewGuid().ToString();

            // Create user
            var user = new ApplicationUser
            {
                FullName = request.FullName,
                Email = request.Email,
                PhoneNumber = request.PhoneNumber,
                UserName = request.Email,
                Role = request.Role,

                // Email verification
                IsEmailVerified = false,
                EmailVerificationToken = token
            };

            // Save user
            var result = await _userManager.CreateAsync(
                user,
                request.Password
            );

            // Validation errors
            if (!result.Succeeded)
            {
                throw new Exception(
                    string.Join(
                        ", ",
                        result.Errors.Select(e => e.Description)
                    )
                );
            }

            // Create role if not exists
            if (!await _roleManager.RoleExistsAsync(request.Role))
            {
                await _roleManager.CreateAsync(
                    new IdentityRole(request.Role)
                );
            }

            // Assign role
            await _userManager.AddToRoleAsync(
                user,
                request.Role
            );

            // Send verification email
            await _emailService.SendVerificationEmailAsync(
                user.Email!,
                token
            );

            // Return response
            return new AuthResponseDTO
            {
                FullName = user.FullName,
                Email = user.Email,
                PhoneNumber = user.PhoneNumber,
                UserId = user.Id,
                Role = user.Role
            };
        }
    }
}