using MediatR;
using Microsoft.AspNetCore.Identity;
using PayTrack.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PayTrack.Application.Features.Auth.Commands
{
    public class VerifyEmailCommandHandler: IRequestHandler<VerifyEmailCommand, string>
    {
        private readonly UserManager<ApplicationUser> _userManager;

        public VerifyEmailCommandHandler(
            UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }

        public async Task<string> Handle(
            VerifyEmailCommand request,
            CancellationToken cancellationToken)
        {
            var user = _userManager.Users
                .FirstOrDefault(x =>
                    x.EmailVerificationToken == request.Token);

            if (user == null)
            {
                throw new Exception("Invalid token");
            }

            user.IsEmailVerified = true;
            user.EmailVerificationToken = null;

            await _userManager.UpdateAsync(user);

            return "Email verified successfully";
        }
    }
}
