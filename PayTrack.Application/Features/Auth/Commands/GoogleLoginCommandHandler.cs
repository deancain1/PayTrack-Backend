using MediatR;
using Microsoft.AspNetCore.Identity;
using PayTrack.Application.DTOs;
using PayTrack.Application.Interfaces.Services;
using PayTrack.Domain.Entities;

namespace PayTrack.Application.Features.Auth.Commands
{
    public class GoogleLoginCommandHandler
        : IRequestHandler<GoogleLoginCommand, AuthResponseDTO>
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ITokenService _tokenService;

        public GoogleLoginCommandHandler(
            UserManager<ApplicationUser> userManager,
            ITokenService tokenService)
        {
            _userManager = userManager;
            _tokenService = tokenService;
        }

        public async Task<AuthResponseDTO> Handle(
            GoogleLoginCommand request,
            CancellationToken cancellationToken)
        {
            // 1. Find account by Google login
            var user = await _userManager.FindByLoginAsync(
                request.Provider,
                request.ProviderKey
            );

            // 2. If Google isn't linked,
            //    try finding the account by email
            if (user == null)
            {
                user = await _userManager.FindByEmailAsync(
                    request.Email
                );
            }

            // 3. Google login is only for existing accounts
            if (user == null)
            {
                throw new UnauthorizedAccessException(
                    "No account is registered with this Google account. Please register first."
                );
            }

            // 4. Make sure Google login is linked
            var existingLogins =
                await _userManager.GetLoginsAsync(user);

            var googleLoginExists =
                existingLogins.Any(login =>
                    login.LoginProvider == request.Provider &&
                    login.ProviderKey == request.ProviderKey
                );

            if (!googleLoginExists)
            {
                var loginInfo = new UserLoginInfo(
                    request.Provider,
                    request.ProviderKey,
                    "Google"
                );

                var loginResult =
                    await _userManager.AddLoginAsync(
                        user,
                        loginInfo
                    );

                if (!loginResult.Succeeded)
                {
                    throw new Exception(
                        string.Join(
                            ", ",
                            loginResult.Errors.Select(
                                e => e.Description
                            )
                        )
                    );
                }
            }

            // 5. Get roles
            var roles =
                await _userManager.GetRolesAsync(user);

            // 6. Generate JWT
            var accessToken =
                _tokenService.GenerateAccessToken(
                    user,
                    roles
                );

            // 7. Generate refresh token
            var refreshToken =
                await _tokenService
                    .GenerateAndSaveRefreshTokenAsync(
                        user
                    );

            // 8. Return authentication response
            return new AuthResponseDTO(
                FullName: user.FullName,
                PhoneNumber:
                    user.PhoneNumber ?? string.Empty,
                Email:
                    user.Email ?? string.Empty,
                Token:
                    accessToken,
                RefreshToken:
                    refreshToken,
                UserId:
                    user.Id,
                Role:
                    roles.FirstOrDefault() ?? string.Empty,
                RequiresTwoFactor:
                    false
            );
        }
    }
}