using MediatR;
using Microsoft.AspNetCore.Identity;
using PayTrack.Application.DTOs;
using PayTrack.Application.Interfaces;
using PayTrack.Domain.Entities;

namespace PayTrack.Application.Features.Auth.Commands
{
    public class LoginCommandHandler
        : IRequestHandler<LoginCommand, AuthResponseDTO>
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly ITokenService _tokenService;

        public LoginCommandHandler(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            ITokenService tokenService)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _tokenService = tokenService;
        }

        public async Task<AuthResponseDTO> Handle(
            LoginCommand request,
            CancellationToken cancellationToken)
        {
            // 1. Find user
            var user = await _userManager.FindByEmailAsync(request.Email);

            if (user == null)
            {
                throw new UnauthorizedAccessException("Invalid credentials");
            }

            // 2. CHECK EMAIL VERIFICATION (IMPORTANT FIX)
            if (!user.IsEmailVerified)
            {
                throw new UnauthorizedAccessException("Please verify your email before logging in.");
            }

            // 3. Check password
            var result = await _signInManager.CheckPasswordSignInAsync(
                user,
                request.Password,
                false
            );

            if (!result.Succeeded)
            {
                throw new UnauthorizedAccessException("Invalid credentials");
            }

            // 4. Sign in user
            await _signInManager.SignInAsync(user, isPersistent: false);

            // 5. Get roles
            var roles = await _userManager.GetRolesAsync(user);

            // 6. Generate tokens
            var accessToken = _tokenService.GenerateAccessToken(user, roles);
            var refreshToken = await _tokenService.GenerateAndSaveRefreshTokenAsync(user);

            // 7. Return response
            return new AuthResponseDTO
            {
                FullName = user.FullName,
                Email = user.Email,
                Token = accessToken,
                RefreshToken = refreshToken,
                UserId = user.Id.ToString(),
                Role = roles.FirstOrDefault()
            };
        }
    }
}