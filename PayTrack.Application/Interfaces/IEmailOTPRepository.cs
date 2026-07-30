using PayTrack.Domain.Entities;
using PayTrack.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PayTrack.Application.Interfaces
{
    public interface IEmailOTPRepository
    {
        Task AddAsync(EmailOTP otp);
        Task UpdateAsync(EmailOTP otp);
        Task<EmailOTP?> GetLatestValidOtpAsync(string applicationUserId, EmailOTPPurpose purpose);
        Task DeleteExpiredOtpsAsync();
        Task InvalidatePreviousOtpsAsync(string applicationUserId, EmailOTPPurpose purpose);
    }
}
