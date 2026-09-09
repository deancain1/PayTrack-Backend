using MediatR;
using Microsoft.AspNetCore.Identity;
using PayTrack.Application.Interfaces.Repositories;
using PayTrack.Application.Interfaces.Services;
using PayTrack.Domain.Entities;

namespace PayTrack.Application.Features.Auth.Commands
{
    public class ForgotPasswordCommandHandler: IRequestHandler<ForgotPasswordCommand>
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IPasswordResetRepository _passwordResetRepository;
        private readonly IEmailService _emailService;

        public ForgotPasswordCommandHandler(
            UserManager<ApplicationUser> userManager,
            IPasswordResetRepository passwordResetRepository,
            IEmailService emailService)
        {
            _userManager = userManager;
            _passwordResetRepository = passwordResetRepository;
            _emailService = emailService;
        }

        public async Task Handle(ForgotPasswordCommand request, CancellationToken cancellationToken)
        {
            var user = await _userManager.FindByEmailAsync(request.Email);

            if (user == null)
            {
                throw new KeyNotFoundException("Email does not exist.");
            }

            if (user.IsGoogleUser)
            {
                throw new InvalidOperationException(
                    "This account uses Google login and cannot reset the password using OTP.");
            }

            await _passwordResetRepository.InvalidOtpAsync(user.Id);

            var otp = Random.Shared
                .Next(100000, 999999)
                .ToString();

            await _passwordResetRepository.AddAsync(
                new PasswordResetOTP
                {
                    UserId = user.Id,
                    Email = user.Email,
                    OtpCode = otp,
                    ExpiresAt = DateTime.UtcNow.AddMinutes(5),
                    IsUsed = false
                });

            await _emailService.SendOtpAsync(
                user.Email!,
                otp);
        }
    }
}