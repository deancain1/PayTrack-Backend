using MailKit.Net.Smtp;
using Microsoft.Extensions.Configuration;
using MimeKit;
using PayTrack.Application.Interfaces.Services;
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

        public async Task SendLoginOtpEmailAsync(
    string email,
    string fullName,
    string otp)
        {
            var message = new MimeMessage();

            message.From.Add(new MailboxAddress(
                "PayTrack System",
                _configuration["EmailSettings:Email"]));

            message.To.Add(MailboxAddress.Parse(email));

            message.Subject = "Your PayTrack Login Verification Code";

            message.Body = new TextPart("html")
            {
                Text = $@"
            <div style='font-family: Arial, sans-serif; max-width: 600px; margin: auto;'>

                <h2 style='color:#661F1F;'>PayTrack Login Verification</h2>

                <p>Hello <strong>{fullName}</strong>,</p>

                <p>Your verification code is:</p>

                <div style='
                    font-size:32px;
                    font-weight:bold;
                    letter-spacing:8px;
                    text-align:center;
                    padding:20px;
                    background:#f5f5f5;
                    border-radius:8px;
                    margin:20px 0;'>
                    {otp}
                </div>

                <p>This code will expire in <strong>5 minutes</strong>.</p>

                <p>If you did not attempt to sign in, please ignore this email.</p>

                <br/>

                <p>Regards,<br/><strong>PayTrack Team</strong></p>

            </div>"
            };

            using var smtp = new SmtpClient();

            await smtp.ConnectAsync(
                _configuration["EmailSettings:Host"],
                int.Parse(_configuration["EmailSettings:Port"]),
                MailKit.Security.SecureSocketOptions.StartTls);

            await smtp.AuthenticateAsync(
                _configuration["EmailSettings:Email"],
                _configuration["EmailSettings:Password"]);

            await smtp.SendAsync(message);

            await smtp.DisconnectAsync(true);
        }
    }
}

