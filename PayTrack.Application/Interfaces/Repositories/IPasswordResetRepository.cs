using PayTrack.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PayTrack.Application.Interfaces.Repositories
{
    public interface IPasswordResetRepository : IBaseRepository<PasswordResetOTP>
    {
        Task InvalidOtpAsync(string userId);
        Task<PasswordResetOTP?> GetValidOtpAsync(string userId, string otpCode);
    }
}
