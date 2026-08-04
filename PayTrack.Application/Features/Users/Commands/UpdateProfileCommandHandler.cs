using MediatR;
using Microsoft.AspNetCore.Identity;
using PayTrack.Application.Interfaces;
using PayTrack.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PayTrack.Application.Features.Users.Commands
{
    public class UpdateProfileCommandHandler : IRequestHandler<UpdateProfileCommand, Unit>
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ICurrentUserService _currentUserService;

        public UpdateProfileCommandHandler(UserManager<ApplicationUser> userManager, ICurrentUserService currentUserService)
        {
            _userManager = userManager;
            _currentUserService = currentUserService;
        }
        public async Task<Unit> Handle(UpdateProfileCommand request, CancellationToken cancellationToken)
        {
            var user = await _userManager.FindByIdAsync(_currentUserService.UserId!);

            if (user is null)
                throw new UnauthorizedAccessException("User not found.");

            var existingUser = await _userManager.FindByEmailAsync(request.Email);

            if (existingUser is not null && existingUser.Id != user.Id)
                throw new Exception("Email is already being used.");

            user.FullName = request.FullName;

            if (!string.Equals(user.Email, request.Email, StringComparison.OrdinalIgnoreCase))
            {
                user.Email = request.Email;
                user.UserName = request.Email;
                user.NormalizedEmail = request.Email.ToUpperInvariant();
                user.NormalizedUserName = request.Email.ToUpperInvariant();
            }

            if (!string.IsNullOrWhiteSpace(request.ProfilePicture))
            {
                var base64 = request.ProfilePicture.Contains(',')
                    ? request.ProfilePicture.Split(',')[1]
                    : request.ProfilePicture;

                user.ProfilePicture = Convert.FromBase64String(base64);
            }

            var result = await _userManager.UpdateAsync(user);

            if (!result.Succeeded)
            {
                throw new Exception(
                    string.Join(
                        Environment.NewLine,
                        result.Errors.Select(x => x.Description)));
            }

            return Unit.Value;
        }
    }
}
