using MediatR;
using Microsoft.AspNetCore.Identity;
using PayTrack.Application.DTOs;
using PayTrack.Application.Interfaces.Repositories;
using PayTrack.Application.Interfaces.Services;
using PayTrack.Domain.Entities;

namespace PayTrack.Application.Features.Auth.Commands
{
    public class RegisterCommandHandler
        : IRequestHandler<
            RegisterCommand,
            RegisterCompanyAdminResponseDTO>
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly ICompanyDetailsRepository _companyDetailsRepository;
        private readonly IEmailService _emailService;

        public RegisterCommandHandler(
            UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole> roleManager,
            ICompanyDetailsRepository companyDetailsRepository,
            IEmailService emailService)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _companyDetailsRepository = companyDetailsRepository;
            _emailService = emailService;
        }

        public async Task<RegisterCompanyAdminResponseDTO> Handle(RegisterCommand request, CancellationToken cancellationToken)
        {
            // Generate email verification token
            var token = Guid.NewGuid().ToString();

            var user = new ApplicationUser
            {
                FullName = request.FullName,
                Email = request.Email,
                PhoneNumber = request.PhoneNumber,
                UserName = request.Email,
                Role = request.Role,
                IsGoogleUser = request.IsGoogleUser,
                IsEmailVerified = request.IsGoogleUser,

                EmailVerificationToken =
                    request.IsGoogleUser
                        ? null
                        : token,

                EmailConfirmed =
                    request.IsGoogleUser
            };

            IdentityResult result;

            if (request.IsGoogleUser)
            {
                // Google user does not have a password
                result = await _userManager.CreateAsync(user);
            }
            else
            {
                // Normal registration requires password
                if (string.IsNullOrWhiteSpace(request.Password))
                {
                    throw new Exception(
                        "Password is required."
                    );
                }

                result = await _userManager.CreateAsync(
                    user,
                    request.Password
                );
            }

            if (!result.Succeeded)
            {
                throw new Exception(
                    string.Join(
                        ", ",
                        result.Errors.Select(
                            e => e.Description
                        )
                    )
                );
            }

            // Create role if it does not exist
            if (!await _roleManager.RoleExistsAsync(request.Role))
            {
                await _roleManager.CreateAsync(
                    new IdentityRole(request.Role)
                );
            }

            // Assign role to user
            var roleResult =
                await _userManager.AddToRoleAsync(
                    user,
                    request.Role
                );

            if (!roleResult.Succeeded)
            {
                throw new Exception(
                    string.Join(
                        ", ",
                        roleResult.Errors.Select(
                            e => e.Description
                        )
                    )
                );
            }

            // Add Google login provider
            if (request.IsGoogleUser)
            {
                if (string.IsNullOrWhiteSpace(
                    request.GoogleProviderKey))
                {
                    throw new Exception(
                        "Google provider key is required."
                    );
                }

                var googleLogin =
                    new UserLoginInfo(
                        loginProvider: "Google",
                        providerKey:
                            request.GoogleProviderKey,
                        displayName: "Google"
                    );

                var googleResult =
                    await _userManager.AddLoginAsync(
                        user,
                        googleLogin
                    );

                if (!googleResult.Succeeded)
                {
                    throw new Exception(
                        string.Join(
                            ", ",
                            googleResult.Errors.Select(
                                e => e.Description
                            )
                        )
                    );
                }
            }

            // Create company details
            var company = new CompanyDetails
            {
                Id = Guid.NewGuid(),

                CompanyName =
                    request.CompanyName,

                BusinessType =
                    request.BusinessType,

                CompanyAdminId =
                    user.Id
            };

            await _companyDetailsRepository.AddAsync(
                company
            );

            // Send verification email
            if (!request.IsGoogleUser)
            {
                await _emailService.SendVerificationEmailAsync(
                    user.Email!,
                    token
                );
            }

            return new RegisterCompanyAdminResponseDTO(
                FullName: user.FullName,
                PhoneNumber: user.PhoneNumber ?? string.Empty,
                Email: user.Email ?? string.Empty,
                UserId: user.Id,
                Role: user.Role,
                CompanyId: company.Id
            );
        }
    }
}