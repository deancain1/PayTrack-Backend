using MediatR;
using Microsoft.AspNetCore.Identity;
using PayTrack.Application.Interfaces.Repositories;
using PayTrack.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PayTrack.Application.Features.Auth.Commands
{
    public class ResetPasswordCommandHandler
       : IRequestHandler<ResetPasswordCommand, bool>
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IPasswordResetRepository _passwordResetRepository;

        public ResetPasswordCommandHandler(
            UserManager<ApplicationUser> userManager,
            IPasswordResetRepository passwordResetRepository)
        {
            _userManager = userManager;
            _passwordResetRepository = passwordResetRepository;
        }

        public async Task<bool> Handle(ResetPasswordCommand request, CancellationToken cancellationToken)
        {
            var user = await _userManager.FindByEmailAsync(request.Email);

            if (user == null)
                return false;

  
            if (user.IsGoogleUser)
                return false;

            
            var otp = await _passwordResetRepository.GetValidOtpAsync(
                user.Id,
                request.Otp);

            if (otp == null)
                return false;

         
            var resetToken =
                await _userManager.GeneratePasswordResetTokenAsync(user);

          
            var result = await _userManager.ResetPasswordAsync(
                user,
                resetToken,
                request.NewPassword);

            if (!result.Succeeded)
                return false;

           
            otp.IsUsed = true;

            _passwordResetRepository.Update(otp);

            await _passwordResetRepository.SaveChangesAsync();

            return true;
        }
    }
}
