using MediatR;
using Microsoft.AspNetCore.Identity;
using PayTrack.Application.Interfaces;
using PayTrack.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PayTrack.Application.Features.Auth.Commands
{
    public class EnableTwoFactorCommandHandler: IRequestHandler<EnableTwoFactorCommand, bool>
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ICurrentUserService _currentUserService;

        public EnableTwoFactorCommandHandler(UserManager<ApplicationUser> userManager, ICurrentUserService currentUserService)
        {
            _userManager = userManager;
            _currentUserService = currentUserService;
        }

        public async Task<bool> Handle(EnableTwoFactorCommand request, CancellationToken cancellationToken)
        {
            var user = await _userManager.FindByIdAsync(
                _currentUserService.UserId!);

            if (user == null)
                return false;

            user.TwoFactorEnabled = true;

            var result = await _userManager.UpdateAsync(user);

            return result.Succeeded;
        }
    }
}
