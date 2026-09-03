using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using PayTrack.Application.Interfaces.Repositories;
using PayTrack.Application.Interfaces.Services;
using PayTrack.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PayTrack.Application.Features.Auth.Commands
{
    public class DeleteAccountCommandHandler : IRequestHandler<DeleteAccountCommand, bool>
    {
        private readonly ICurrentUserService _currentUserService;
        private readonly IUserRepository _userRepository;
        private readonly UserManager<ApplicationUser> _userManager;
        public DeleteAccountCommandHandler(
            ICurrentUserService currentUserService,
            IUserRepository userRepository,
            UserManager<ApplicationUser> userManager)
        {
            _currentUserService = currentUserService;
            _userRepository = userRepository;
            _userManager = userManager;
        }
        public async Task<bool> Handle(DeleteAccountCommand request, CancellationToken cancellationToken)
        {
            var currentUserId = _currentUserService.UserId;

            if (string.IsNullOrEmpty(currentUserId))
            {
                throw new UnauthorizedAccessException(
                    "You must be logged in to delete your account.");
            }

            var user = await _userManager.FindByIdAsync(currentUserId);

            if (user == null)
            {
                throw new KeyNotFoundException(
                    "User account not found.");
            }

            _userRepository.Delete(user);

            await _userRepository.SaveChangesAsync();

            return true;
        }
    }
}
