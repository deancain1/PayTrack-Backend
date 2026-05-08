using MailKit.Net.Smtp;
using Microsoft.Extensions.Configuration;
using MimeKit;
using PayTrack.Application.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PayTrack.Infrastructure.Services
{
    public class EmailService : IEmailService
    {
        private readonly IConfiguration _configuration;

        public EmailService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task SendVerificationEmailAsync(
            string email,
            string token)
        {
            var message = new MimeMessage();

            // Sender
            message.From.Add(new MailboxAddress(
                "PayTrack System",
                _configuration["EmailSettings:Email"]
            ));

            // Receiver
            message.To.Add(MailboxAddress.Parse(email));

            message.Subject = "Verify Your Email Address";

            // IMPORTANT: replace localhost in production
            var verificationLink =
                $"https://localhost:7221/api/auth/verify-email?token={token}";

            message.Body = new TextPart("html")
            {
                Text = $@"
                    <h2>Email Verification</h2>
                    <p>Click the button below to verify your account:</p>

                    <a href='{verificationLink}'
                       style='display:inline-block;
                              padding:10px 20px;
                              background:#007bff;
                              color:#fff;
                              text-decoration:none;
                              border-radius:5px;'>
                        Verify Email
                    </a>

                    <p>If you did not create an account, ignore this email.</p>
                "
            };

            using var smtp = new SmtpClient();

            await smtp.ConnectAsync(
                _configuration["EmailSettings:Host"],
                int.Parse(_configuration["EmailSettings:Port"]),
                MailKit.Security.SecureSocketOptions.StartTls
            );

            await smtp.AuthenticateAsync(
                _configuration["EmailSettings:Email"],
                _configuration["EmailSettings:Password"]
            );

            await smtp.SendAsync(message);

            await smtp.DisconnectAsync(true);
        }
    }
}

