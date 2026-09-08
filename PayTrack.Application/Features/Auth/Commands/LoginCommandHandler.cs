using System.Security.Cryptography;
using MediatR;
using Microsoft.AspNetCore.Identity;
using PayTrack.Application.DTOs;
using PayTrack.Application.Interfaces.Repositories;
using PayTrack.Application.Interfaces.Services;
using PayTrack.Domain.Entities;
using PayTrack.Domain.Enums;

namespace PayTrack.Application.Features.Auth.Commands
{
    public class LoginCommandHandler
        : IRequestHandler<LoginCommand, AuthResponseDTO>
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly ITokenService _tokenService;
        private readonly IEmailOTPRepository _emailOTPRepository;
        private readonly IEmailService _emailService;
        private readonly IUserRepository _userRepository;

        public LoginCommandHandler(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            ITokenService tokenService,
            IEmailOTPRepository emailOTPRepository,
            IEmailService emailService,
            IUserRepository userRepository)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _tokenService = tokenService;
            _emailOTPRepository = emailOTPRepository;
            _emailService = emailService;
            _userRepository = userRepository;
        }

        public async Task<AuthResponseDTO> Handle(LoginCommand request, CancellationToken cancellationToken)
        {
           
            var user = await _userManager.FindByEmailAsync(
                request.Email
            );

            if (user == null)
            {
                throw new UnauthorizedAccessException(
                    "Invalid credentials."
                );
            }

          
            var result = await _signInManager.CheckPasswordSignInAsync(
                user,
                request.Password,
                lockoutOnFailure: false
            );

            if (!result.Succeeded)
            {
                throw new UnauthorizedAccessException(
                    "Invalid credentials."
                );
            }


            if (!user.IsEmailVerified)
            {
                throw new UnauthorizedAccessException(
                    "Please verify your email address before logging in."
                );
            }


            if (user.TwoFactorEnabled)
            {

                await _emailOTPRepository.InvalidatePreviousOtpsAsync(
                    user.Id,
                    EmailOTPPurpose.Login
                );

              
                string otpCode = RandomNumberGenerator
                    .GetInt32(100000, 1000000)
                    .ToString();

             
                var otp = new EmailOTP
                {
                    ApplicationUserId = user.Id,
                    Code = otpCode,
                    Purpose = EmailOTPPurpose.Login,
                    ExpiresAt = DateTime.UtcNow.AddMinutes(5),
                    IsUsed = false,
                    FailedAttempts = 0
                };

                await _emailOTPRepository.AddAsync(otp);

               
                await _emailService.SendLoginOtpEmailAsync(
                    user.Email!,
                    user.FullName,
                    otpCode
                );

               
                return new AuthResponseDTO(
                    FullName: user.FullName,
                    PhoneNumber: user.PhoneNumber ?? string.Empty,
                    Email: user.Email ?? string.Empty,
                    Token: string.Empty,
                    RefreshToken: string.Empty,
                    UserId: user.Id,
                    Role: string.Empty,
                    RequiresTwoFactor: true
                );
            }

           
            await _signInManager.SignInAsync(
                user,
                isPersistent: false
            );

           
            var roles = await _userManager.GetRolesAsync(user);

            
            var accessToken = _tokenService.GenerateAccessToken(
                user,
                roles
            );

           
            var refreshToken =
                await _tokenService.GenerateAndSaveRefreshTokenAsync(
                    user
                );

           
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