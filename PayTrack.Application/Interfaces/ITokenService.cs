using PayTrack.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PayTrack.Application.Interfaces
{
    public interface ITokenService
    {
        string GenerateAccessToken(ApplicationUser user, IList<string> roles);
        string GenerateRefreshToken();
        Task<string> GenerateAndSaveRefreshTokenAsync(ApplicationUser user);
        Task<ApplicationUser?> GetUserByRefreshTokenAsync(string refreshToken);
        Task<bool> ValidateRefreshTokenAsync(ApplicationUser user, string refreshToken);
    }
}
