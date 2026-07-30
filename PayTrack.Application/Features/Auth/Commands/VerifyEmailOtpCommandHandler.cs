using MediatR;
using Microsoft.AspNetCore.Identity;
using PayTrack.Application.DTOs;
using PayTrack.Application.Interfaces;
using PayTrack.Domain.Entities;
using PayTrack.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

                await _emailOTPRepository.UpdateAsync(otp);

                throw new UnauthorizedAccessException(
                    "Invalid verification code.");
            }

            // Mark OTP as used
            otp.IsUsed = true;

            await _emailOTPRepository.UpdateAsync(otp);

            // Get roles
            var roles = await _userManager.GetRolesAsync(user);

            // Generate tokens
            var accessToken =
                _tokenService.GenerateAccessToken(user, roles);

            var refreshToken =
                await _tokenService.GenerateAndSaveRefreshTokenAsync(user);

            return new AuthResponseDTO
            {
                FullName = user.FullName,
                Email = user.Email,
                UserId = user.Id,
                Role = roles.FirstOrDefault(),
                Token = accessToken,
                RefreshToken = refreshToken,
                RequiresTwoFactor = false
            };
        }
    }
}