using Google;
using Microsoft.EntityFrameworkCore;
using PayTrack.Application.Interfaces.Repositories;
using PayTrack.Domain.Entities;
using PayTrack.Infrastructure.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PayTrack.Infrastructure.Repository
{
    public class PasswordResetRepository : BaseRepository<PasswordResetOTP>, IPasswordResetRepository
    {
        public PasswordResetRepository(AppDbContext context): base(context) {}

        public async Task InvalidOtpAsync(string userId)
        {
            var otps = await _dbSet
                .Where(x =>
                    x.UserId == userId &&
                    !x.IsUsed)
                .ToListAsync();

            foreach (var otp in otps)
            {
                otp.IsUsed = true;
            }

            await _context.SaveChangesAsync();
        }

        public async Task<PasswordResetOTP?> GetValidOtpAsync(string userId, string otpCode)
        {
            return await _dbSet
                .FirstOrDefaultAsync(x =>
                    x.UserId == userId &&
                    x.OtpCode == otpCode &&
                    !x.IsUsed &&
                    x.ExpiresAt > DateTime.UtcNow);
        }
    }
}
