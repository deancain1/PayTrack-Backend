using MediatR;
using Microsoft.AspNetCore.Identity;
using PayTrack.Application.Interfaces;
using PayTrack.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PayTrack.Application.Features.Auth.Queries
{

    public class GetTwoFactorStatusQueryHandler
        : IRequestHandler<GetTwoFactorStatusQuery, bool>
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ICurrentUserService _currentUserService;

        public GetTwoFactorStatusQueryHandler(
            UserManager<ApplicationUser> userManager,
            ICurrentUserService currentUserService)
        {
            _userManager = userManager;
            _currentUserService = currentUserService;
        }

        public async Task<bool> Handle(
            GetTwoFactorStatusQuery request,
            CancellationToken cancellationToken)
        {
            var user = await _userManager.FindByIdAsync(
                _currentUserService.UserId!);

            if (user == null)
                return false;

            return user.TwoFactorEnabled;
        }
    }
}
