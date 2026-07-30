using Microsoft.EntityFrameworkCore;
using PayTrack.Application.Interfaces;
using PayTrack.Domain.Entities;
using PayTrack.Domain.Enums;
using PayTrack.Infrastructure.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PayTrack.Infrastructure.Repository
{
    internal class EmailOTPRepository : IEmailOTPRepository
    {
        private readonly AppDbContext _context;
        public EmailOTPRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(EmailOTP otp)
        {
            await _context.EmailOTPs.AddAsync(otp);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(EmailOTP otp)
        {
            _context.EmailOTPs.Update(otp);
            await _context.SaveChangesAsync();
        }

        public async Task<EmailOTP?> GetLatestValidOtpAsync(
            string applicationUserId,
            EmailOTPPurpose purpose)
        {
            return await _context.EmailOTPs
                .Where(x =>
                    x.ApplicationUserId == applicationUserId &&
                    x.Purpose == purpose &&
                    !x.IsUsed)
                .OrderByDescending(x => x.CreatedAt)
                .FirstOrDefaultAsync();
        }

        public async Task InvalidatePreviousOtpsAsync(
            string applicationUserId,
            EmailOTPPurpose purpose)
        {
            var otps = await _context.EmailOTPs
                .Where(x =>
                    x.ApplicationUserId == applicationUserId &&
                    x.Purpose == purpose &&
                    !x.IsUsed)
                .ToListAsync();

            foreach (var otp in otps)
            {
                otp.IsUsed = true;
            }

            await _context.SaveChangesAsync();
        }

        public async Task DeleteExpiredOtpsAsync()
        {
            var expired = await _context.EmailOTPs
                .Where(x => x.ExpiresAt < DateTime.UtcNow)
                .ToListAsync();

            _context.EmailOTPs.RemoveRange(expired);

            await _context.SaveChangesAsync();
        }
    }
}
