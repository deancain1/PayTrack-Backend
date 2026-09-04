using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
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
        private readonly ILogger<RegisterCommandHandler> _logger;

        public RegisterCommandHandler(
            UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole> roleManager,
            ICompanyDetailsRepository companyDetailsRepository,
            IEmailService emailService,
            ILogger<RegisterCommandHandler> logger)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _companyDetailsRepository = companyDetailsRepository;
            _emailService = emailService;
            _logger = logger;
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
               
                result = await _userManager.CreateAsync(user);
            }
            else
            {
               
                if (string.IsNullOrWhiteSpace(request.Password))
                {
                    throw new Exception("Password is required.");
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
                        result.Errors.Select(e => e.Description)
                    )
                );
            }

            
            if (!await _roleManager.RoleExistsAsync(request.Role))
            {
                var createRoleResult =
                    await _roleManager.CreateAsync(
                        new IdentityRole(request.Role)
                    );

                if (!createRoleResult.Succeeded)
                {
                    throw new Exception(
                        string.Join(
                            ", ",
                            createRoleResult.Errors.Select(
                                e => e.Description
                            )
                        )
                    );
                }
            }

          
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
                        providerKey: request.GoogleProviderKey,
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


            if (!request.IsGoogleUser)
            {
                try
                {
                    await _emailService.SendVerificationEmailAsync(
                        user.Email!,
                        token
                    );

                    _logger.LogInformation(
                        "Verification email sent successfully to {Email}",
                        user.Email
                    );
                }
                catch (Exception ex)
                {
                    _logger.LogError(
                        ex,
                        "Failed to send verification email to {Email}",
                        user.Email
                    );

                }
            }

            // Return successful registration response
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