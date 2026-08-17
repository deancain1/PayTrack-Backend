using MailKit.Net.Smtp;
using Microsoft.Extensions.Configuration;
using MimeKit;
using PayTrack.Application.Interfaces.Services;

namespace PayTrack.Infrastructure.Services
{
    public class EmailService : IEmailService
    {
        private readonly IConfiguration _configuration;

        public EmailService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task SendVerificationEmailAsync(string email, string token)
        {
            var message = new MimeMessage();

            message.From.Add(new MailboxAddress(
                "PayTrack System",
                _configuration["EmailSettings:Email"]
            ));

            message.To.Add(MailboxAddress.Parse(email));

            message.Subject = "Verify Your Email Address";

            var verificationLink =
                $"https://localhost:7221/api/auth/verify-email?token={token}";

            message.Body = new TextPart("html")
            {
                Text = $@"
                    <div style='
                        font-family: Arial, sans-serif;
                        max-width: 600px;
                        margin: auto;
                        padding: 20px;
                        color: #333;'>

                        <h2 style='color:#661F1F;'>
                            Email Verification
                        </h2>

                        <p>
                            Click the button below to verify your
                            PayTrack account.
                        </p>

                        <a href='{verificationLink}'
                           style='display:inline-block;
                                  padding:10px 20px;
                                  background:#661F1F;
                                  color:#fff;
                                  text-decoration:none;
                                  border-radius:5px;'>
                            Verify Email
                        </a>

                        <p>
                            If you did not create an account,
                            you can ignore this email.
                        </p>

                        <br/>

                        <p>
                            Regards,<br/>
                            <strong>PayTrack Team</strong>
                        </p>

                    </div>"
            };

            await SendEmailAsync(message);
        }

        public async Task SendLoginOtpEmailAsync(string email, string fullName, string otp)
        {
            var message = new MimeMessage();

            message.From.Add(new MailboxAddress(
                "PayTrack System",
                _configuration["EmailSettings:Email"]
            ));

            message.To.Add(MailboxAddress.Parse(email));

            message.Subject =
                "Your PayTrack Login Verification Code";

            message.Body = new TextPart("html")
            {
                Text = $@"
                    <div style='
                        font-family: Arial, sans-serif;
                        max-width: 600px;
                        margin: auto;
                        padding: 20px;
                        color: #333;'>

                        <h2 style='color:#661F1F;'>
                            PayTrack Login Verification
                        </h2>

                        <p>
                            Hello <strong>{fullName}</strong>,
                        </p>

                        <p>
                            Your verification code is:
                        </p>

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

                        <p>
                            This code will expire in
                            <strong>5 minutes</strong>.
                        </p>

                        <p>
                            If you did not attempt to sign in,
                            please ignore this email.
                        </p>

                        <br/>

                        <p>
                            Regards,<br/>
                            <strong>PayTrack Team</strong>
                        </p>

                    </div>"
            };

            await SendEmailAsync(message);
        }

        public async Task SendLeaveApprovedEmailAsync(string email, string fullName, DateTime startDate, DateTime endDate, string? remarks)
        {
            var message = new MimeMessage();

            message.From.Add(new MailboxAddress(
                "PayTrack System",
                _configuration["EmailSettings:Email"]));

            message.To.Add(MailboxAddress.Parse(email));

            message.Subject = "Leave Request Approved";

            var remarksSection = string.IsNullOrWhiteSpace(remarks)
                ? ""
                : $@"
            <p>
                <strong>Admin Remarks:</strong>
                {remarks}
            </p>";

            message.Body = new TextPart("html")
            {
                Text = $@"
            <div style='
                font-family: Arial, sans-serif;
                max-width: 600px;
                margin: auto;
                padding: 20px;
                color: #333;'>

                <h2 style='color:#661F1F;'>
                    Leave Request Approved
                </h2>

                <p>
                    Hello <strong>{fullName}</strong>,
                </p>

                <p>
                    Your leave request has been
                    <strong style='color:green;'>
                        approved
                    </strong>.
                </p>

                <div style='
                    background:#f5f5f5;
                    padding:15px;
                    border-radius:8px;
                    margin:20px 0;'>

                    <p>
                        <strong>Start Date:</strong>
                        {startDate:MMMM dd, yyyy}
                    </p>

                    <p>
                        <strong>End Date:</strong>
                        {endDate:MMMM dd, yyyy}
                    </p>

                    {remarksSection}

                </div>

                <p>
                    Please make sure to coordinate your leave
                    accordingly.
                </p>

                <br/>

                <p>
                    Regards,<br/>
                    <strong>PayTrack Team</strong>
                </p>

            </div>"
            };

            await SendEmailAsync(message);
        }

        public async Task SendLeaveRejectedEmailAsync(string email, string fullName, DateTime startDate, DateTime endDate, string remarks)
        {
            var message = new MimeMessage();

            message.From.Add(new MailboxAddress(
                "PayTrack System",
                _configuration["EmailSettings:Email"]
            ));

            message.To.Add(MailboxAddress.Parse(email));

            message.Subject = "Leave Request Rejected";

            message.Body = new TextPart("html")
            {
                Text = $@"
                    <div style='
                        font-family: Arial, sans-serif;
                        max-width: 600px;
                        margin: auto;
                        padding: 20px;
                        color: #333;'>

                        <h2 style='color:#661F1F;'>
                            Leave Request Rejected
                        </h2>

                        <p>
                            Hello <strong>{fullName}</strong>,
                        </p>

                        <p>
                            Unfortunately, your leave request has been
                            <strong style='color:#dc3545;'>
                                rejected
                            </strong>.
                        </p>

                        <div style='
                            background:#f5f5f5;
                            padding:15px;
                            border-radius:8px;
                            margin:20px 0;'>

                            <p>
                                <strong>Start Date:</strong>
                                {startDate:MMMM dd, yyyy}
                            </p>

                            <p>
                                <strong>End Date:</strong>
                                {endDate:MMMM dd, yyyy}
                            </p>

                            <p>
                                <strong>Reason:</strong>
                                {remarks}
                            </p>

                        </div>

                        <p>
                            If you have questions regarding this decision,
                            please contact your company administrator.
                        </p>

                        <br/>

                        <p>
                            Regards,<br/>
                            <strong>PayTrack Team</strong>
                        </p>

                    </div>"
            };

            await SendEmailAsync(message);
        }


        // =========================================================
        // COMMON SMTP METHOD
        // =========================================================

        private async Task SendEmailAsync(MimeMessage message)
        {
            using var smtp = new SmtpClient();

            await smtp.ConnectAsync(
                _configuration["EmailSettings:Host"],
                int.Parse(
                    _configuration["EmailSettings:Port"]!
                ),
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