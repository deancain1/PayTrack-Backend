using MediatR;
using Microsoft.AspNetCore.Identity;
using PayTrack.Application.Interfaces.Repositories;
using PayTrack.Application.Interfaces.Services;
using PayTrack.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PayTrack.Application.Features.Users.Commands
{
    public class DeactivateAccountCommandHandler : IRequestHandler<DeactivateAccountCommand, bool>
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ICurrentUserService _currentUserService;
        private readonly IUserRepository _userRepository;

        public DeactivateAccountCommandHandler(UserManager<ApplicationUser> userManager, ICurrentUserService currentUserService, IUserRepository userRepository)
        {
            _userManager = userManager;
            _currentUserService = currentUserService;
            _userRepository = userRepository;
        }
        public async Task<bool> Handle(DeactivateAccountCommand request, CancellationToken cancellationToken)
        {
            var user = await _userManager.FindByIdAsync(_currentUserService.UserId!);

            if (user == null)
                throw new Exception("User not found.");

            if (!user.IsActive)
                throw new Exception("Your account is already deactivated.");


            await _userRepository.DeactivateUserAsync(user);
            return true;
        }
    }
}
