using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using PayTrack.Application.Interfaces.Services;

namespace PayTrack.Infrastructure.Services
{
    public class EmailService : IEmailService
    {
        private readonly IConfiguration _configuration;
        private readonly HttpClient _httpClient;
        private readonly ILogger<EmailService> _logger;

        public EmailService(
            IConfiguration configuration,
            HttpClient httpClient,
            ILogger<EmailService> logger)
        {
            _configuration = configuration;
            _httpClient = httpClient;
            _logger = logger;
        }

        // EMAIL VERIFICATION

        public async Task SendVerificationEmailAsync(
            string email,
            string token)
        {
            var verificationLink =
                $"https://paytrack-backend-1-i20d.onrender.com/api/auth/verify-email?token={Uri.EscapeDataString(token)}";

            var htmlContent = $@"
                <div style='
                    font-family: Arial, sans-serif;
                    max-width: 600px;
                    margin: auto;
                    padding: 20px;
                    color: #333;'>

                    <h2 style='color:#062b52;'>
                        Email Verification
                    </h2>

                    <p>
                        Click the button below to verify your
                        PayTrack account.
                    </p>

                    <div style='margin:30px 0;'>
                        <a href='{verificationLink}'
                           style='
                               display:inline-block;
                               padding:10px 20px;
                               background:#062b52;
                               color:#fff;
                               text-decoration:none;
                               border-radius:5px;'>
                            Verify Email
                        </a>
                    </div>

                    <p>
                        If you did not create an account,
                        you can ignore this email.
                    </p>

                    <br/>

                    <p>
                        Regards,<br/>
                        <strong>PayTrack Team</strong>
                    </p>

                </div>";

            await SendEmailAsync(
                email,
                "Verify Your Email Address",
                htmlContent);
        }

        // EMPLOYEE VERIFICATION

        public async Task SendEmployeeVerificationEmailAsync(
            string email,
            string fullName,
            string companyName,
            string token)
        {
            var verificationLink =
                $"https://paytrack-backend-1-i20d.onrender.com/api/auth/verify-email?token={Uri.EscapeDataString(token)}";

            var htmlContent = $@"
                <div style='
                    font-family: Arial, sans-serif;
                    max-width: 600px;
                    margin: auto;
                    padding: 20px;
                    color: #333;'>

                    <h2 style='color:#062b52;'>
                        Welcome to {companyName}!
                    </h2>

                    <p>
                        Hello <strong>{fullName}</strong>,
                    </p>

                    <p>
                        You have been registered as an employee of
                        <strong>{companyName}</strong> on PayTrack.
                    </p>

                    <p>
                        To activate your PayTrack account, please verify
                        your email address by clicking the button below.
                    </p>

                    <div style='margin:30px 0;'>
                        <a href='{verificationLink}'
                           style='
                               display:inline-block;
                               padding:12px 24px;
                               background:#062b52;
                               color:#fff;
                               text-decoration:none;
                               border-radius:5px;
                               font-weight:bold;'>
                            Verify Email
                        </a>
                    </div>

                    <p>
                        After verifying your email, you will be able to
                        access your PayTrack employee account.
                    </p>

                    <p>
                        If you did not expect this account, please contact
                        your company administrator.
                    </p>

                    <br/>

                    <p>
                        Regards,<br/>
                        <strong>PayTrack Team</strong>
                    </p>

                </div>";

            await SendEmailAsync(
                email,
                $"Welcome to {companyName} - Verify Your PayTrack Account",
                htmlContent);
        }

        // LOGIN OTP

        public async Task SendLoginOtpEmailAsync(
            string email,
            string fullName,
            string otp)
        {
            var htmlContent = $@"
                <div style='
                    font-family: Arial, sans-serif;
                    max-width: 600px;
                    margin: auto;
                    padding: 20px;
                    color: #333;'>

                    <h2 style='color:#062b52;'>
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
                        color:#062b52;
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

                </div>";

            await SendEmailAsync(
                email,
                "Your PayTrack Login Verification Code",
                htmlContent);
        }

        // PASSWORD RESET OTP

        public async Task SendOtpAsync(
            string email,
            string otp)
        {
            var htmlContent = $@"
                <div style='
                    font-family: Arial, sans-serif;
                    max-width: 600px;
                    margin: auto;
                    padding: 20px;
                    color: #333;'>

                    <h2 style='color:#062b52;'>
                        PayTrack Password Reset
                    </h2>

                    <p>
                        We received a request to reset your
                        PayTrack password.
                    </p>

                    <p>
                        Your password reset verification code is:
                    </p>

                    <div style='
                        font-size:32px;
                        font-weight:bold;
                        letter-spacing:8px;
                        text-align:center;
                        padding:20px;
                        background:#f5f5f5;
                        color:#062b52;
                        border-radius:8px;
                        margin:20px 0;'>
                        {otp}
                    </div>

                    <p>
                        This code will expire in
                        <strong>5 minutes</strong>.
                    </p>

                    <p>
                        If you did not request a password reset,
                        please ignore this email.
                    </p>

                    <br/>

                    <p>
                        Regards,<br/>
                        <strong>PayTrack Team</strong>
                    </p>

                </div>";

            await SendEmailAsync(
                email,
                "Your PayTrack Password Reset Code",
                htmlContent);
        }

        // LEAVE APPROVED

        public async Task SendLeaveApprovedEmailAsync(
            string email,
            string fullName,
            DateTime startDate,
            DateTime endDate,
            string? remarks)
        {
            var remarksSection =
                string.IsNullOrWhiteSpace(remarks)
                    ? ""
                    : $@"
                        <p>
                            <strong>Admin Remarks:</strong>
                            {remarks}
                        </p>";

            var htmlContent = $@"
                <div style='
                    font-family: Arial, sans-serif;
                    max-width: 600px;
                    margin: auto;
                    padding: 20px;
                    color: #333;'>

                    <h2 style='color:#062b52;'>
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

                </div>";

            await SendEmailAsync(
                email,
                "Leave Request Approved",
                htmlContent);
        }

        // LEAVE REJECTED

        public async Task SendLeaveRejectedEmailAsync(
            string email,
            string fullName,
            DateTime startDate,
            DateTime endDate,
            string remarks)
        {
            var htmlContent = $@"
                <div style='
                    font-family: Arial, sans-serif;
                    max-width: 600px;
                    margin: auto;
                    padding: 20px;
                    color: #333;'>

                    <h2 style='color:#062b52;'>
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

                </div>";

            await SendEmailAsync(
                email,
                "Leave Request Rejected",
                htmlContent);
        }

        // TIME OUT REMINDER

        public async Task SendTimeOutReminderEmailAsync(
            string email,
            string fullName,
            TimeSpan? scheduledEndTime,
            TimeSpan? overtimeEndTime)
        {
            var effectiveEndTime =
                overtimeEndTime ?? scheduledEndTime;

            if (!effectiveEndTime.HasValue)
            {
                throw new ArgumentException(
                    "Either scheduled end time or overtime end time must be provided.");
            }

            var endTimeText =
                DateTime.Today
                    .Add(effectiveEndTime.Value)
                    .ToString("h:mm tt");

            var overtimeSection =
                overtimeEndTime.HasValue
                    ? $@"
                <div style='
                    background:#fff3cd;
                    padding:15px;
                    border-radius:8px;
                    margin:20px 0;'>

                    <p>
                        <strong>Overtime:</strong>
                        You have approved overtime scheduled until
                        <strong>{endTimeText}</strong>.
                    </p>

                </div>"
                    : $@"
                <div style='
                    background:#f5f5f5;
                    padding:15px;
                    border-radius:8px;
                    margin:20px 0;'>

                    <p>
                        <strong>Scheduled End Time:</strong>
                        {endTimeText}
                    </p>

                </div>";

            var htmlContent = $@"
                <div style='
                    font-family: Arial, sans-serif;
                    max-width: 600px;
                    margin: auto;
                    padding: 20px;
                    color: #333;'>

                    <h2 style='color:#062b52;'>
                        Time Out Reminder
                    </h2>

                    <p>
                        Hello <strong>{fullName}</strong>,
                    </p>

                    <p>
                        This is a friendly reminder that you have not yet
                        recorded your <strong>Time Out</strong> for today.
                    </p>

                    {overtimeSection}

                    <p>
                        Please remember to time out when you have finished
                        your work for today so your attendance and working
                        hours can be recorded accurately.
                    </p>

                    <p>
                        If you have already timed out, please disregard
                        this email.
                    </p>

                    <br/>

                    <p>
                        Regards,<br/>
                        <strong>PayTrack Team</strong>
                    </p>

                </div>";

            await SendEmailAsync(
                email,
                "Reminder: Please Time Out",
                htmlContent);
        }

        // COMMON BREVO API METHOD

        private async Task SendEmailAsync(
            string recipientEmail,
            string subject,
            string htmlContent)
        {
            var apiKey =
                _configuration["Brevo:ApiKey"];

            var senderEmail =
                _configuration["Brevo:SenderEmail"];

            var senderName =
                _configuration["Brevo:SenderName"];

            if (string.IsNullOrWhiteSpace(apiKey))
            {
                throw new Exception(
                    "Brevo:ApiKey is missing.");
            }

            if (string.IsNullOrWhiteSpace(senderEmail))
            {
                throw new Exception(
                    "Brevo:SenderEmail is missing.");
            }

            if (string.IsNullOrWhiteSpace(senderName))
            {
                senderName = "PayTrack System";
            }

            if (string.IsNullOrWhiteSpace(recipientEmail))
            {
                throw new ArgumentException(
                    "Recipient email is required.");
            }

            var requestBody = new
            {
                sender = new
                {
                    email = senderEmail,
                    name = senderName
                },

                to = new[]
                {
                    new
                    {
                        email = recipientEmail
                    }
                },

                subject = subject,

                htmlContent = htmlContent
            };

            var json =
                JsonSerializer.Serialize(requestBody);

            using var request =
                new HttpRequestMessage(
                    HttpMethod.Post,
                    "https://api.brevo.com/v3/smtp/email");

            request.Headers.Add(
                "api-key",
                apiKey);

            request.Content =
                new StringContent(
                    json,
                    Encoding.UTF8,
                    "application/json");

            _logger.LogInformation(
                "Sending email through Brevo to {Email}",
                recipientEmail);

            try
            {
                var response =
                    await _httpClient.SendAsync(request);

                var responseContent =
                    await response.Content.ReadAsStringAsync();

                if (!response.IsSuccessStatusCode)
                {
                    _logger.LogError(
                        "Brevo email failed. Status: {StatusCode}. Response: {Response}",
                        response.StatusCode,
                        responseContent);

                    throw new Exception(
                        $"Brevo email failed: {responseContent}");
                }

                _logger.LogInformation(
                    "Email successfully sent through Brevo to {Email}",
                    recipientEmail);
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "Error sending email through Brevo to {Email}",
                    recipientEmail);

                throw;
            }
        }
    }
}