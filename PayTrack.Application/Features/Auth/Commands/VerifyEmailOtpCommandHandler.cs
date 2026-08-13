using MediatR;
using Microsoft.AspNetCore.Identity;
using PayTrack.Application.DTOs;
using PayTrack.Application.Interfaces.Repositories;
using PayTrack.Application.Interfaces.Services;
using PayTrack.Domain.Entities;
using PayTrack.Domain.Enums;

namespace PayTrack.Application.Features.Auth.Commands
{
    public class VerifyEmailOtpCommandHandler: IRequestHandler<VerifyEmailOtpCommand, AuthResponseDTO>
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IEmailOTPRepository _emailOTPRepository;
        private readonly ITokenService _tokenService;

        public VerifyEmailOtpCommandHandler(UserManager<ApplicationUser> userManager, IEmailOTPRepository emailOTPRepository, ITokenService tokenService)
        {
            _userManager = userManager;
            _emailOTPRepository = emailOTPRepository;
            _tokenService = tokenService;
        }

        public async Task<AuthResponseDTO> Handle( VerifyEmailOtpCommand request, CancellationToken cancellationToken)
        {
            // Find user
            var user = await _userManager.FindByIdAsync(request.UserId);

            if (user == null)
            {
                throw new UnauthorizedAccessException("User not found.");
            }

            // Get latest unused OTP
            var otp = await _emailOTPRepository.GetLatestValidOtpAsync(
                user.Id,
                EmailOTPPurpose.Login);

            if (otp == null)
            {
                throw new UnauthorizedAccessException(
                    "Verification code not found.");
            }

            // Already used
            if (otp.IsUsed)
            {
                throw new UnauthorizedAccessException(
                    "Verification code has already been used.");
            }

            // Expired
            if (otp.ExpiresAt < DateTime.UtcNow)
            {
                throw new UnauthorizedAccessException(
                    "Verification code has expired.");
            }

            // Wrong OTP
            if (otp.Code != request.Otp)
            {
                otp.FailedAttempts++;

                 _emailOTPRepository.Update(otp);
               await  _emailOTPRepository.SaveChangesAsync();

                throw new UnauthorizedAccessException(
                    "Invalid verification code.");
            }

            // Mark OTP as used
            otp.IsUsed = true;

             _emailOTPRepository.Update(otp);
            await _emailOTPRepository.SaveChangesAsync();
            // Get roles
            var roles = await _userManager.GetRolesAsync(user);

            // Generate access token
            var accessToken =
                _tokenService.GenerateAccessToken(user, roles);

            // Generate refresh token
            var refreshToken =
                await _tokenService.GenerateAndSaveRefreshTokenAsync(user);

            // Return authenticated response
            return new AuthResponseDTO(
                FullName: user.FullName,
                PhoneNumber: user.PhoneNumber ?? string.Empty,
                Email: user.Email ?? string.Empty,
                Token: accessToken,
                RefreshToken: refreshToken,
                UserId: user.Id,
                Role: roles.FirstOrDefault() ?? string.Empty,
                RequiresTwoFactor: false
            );
        }
    }
}