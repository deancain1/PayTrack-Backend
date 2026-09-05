using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PayTrack.Application.Interfaces.Services
{
    public interface IEmailService
    {
        Task SendVerificationEmailAsync(string email, string token);

        Task SendEmployeeVerificationEmailAsync(
            string email,
            string fullName,
            string companyName,
            string token);

        Task SendLoginOtpEmailAsync(
            string email,
            string fullName,
            string otp);

        Task SendLeaveApprovedEmailAsync(
            string email,
            string fullName,
            DateTime startDate,
            DateTime endDate,
            string? remarks);

        Task SendLeaveRejectedEmailAsync(
            string email,
            string fullName,
            DateTime startDate,
            DateTime endDate,
            string remarks);

        Task SendTimeOutReminderEmailAsync(
            string email,
            string fullName,
            TimeSpan? scheduledEndTime,
            TimeSpan? overtimeEndTime);
    }
}