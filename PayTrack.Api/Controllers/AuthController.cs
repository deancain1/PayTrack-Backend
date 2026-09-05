using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using PayTrack.Application.DTOs;
using PayTrack.Application.Features.Auth.Commands;
using PayTrack.Application.Features.Auth.Queries;
using PayTrack.Application.Features.Employee.Commands;
using System.Security.Claims;

namespace PayTrack.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly IConfiguration _configuration;
        public AuthController(IMediator mediator, IConfiguration configuration)
        {
            _mediator = mediator;
            _configuration = configuration;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterCommand command, [FromServices] IValidator<RegisterCommand> validator)
        {
            var validationResult = await validator.ValidateAsync(command);
            if (!validationResult.IsValid)
            {
                return BadRequest(validationResult.Errors
                    .GroupBy(e => e.PropertyName)
                    .ToDictionary(
                        g => g.Key,
                        g => g.Select(e => e.ErrorMessage).ToArray()
                    ));
            }

            var result = await _mediator.Send(command);
            return Ok(result);
        }

        [HttpPost("login")]
        public async Task<ActionResult<AuthResponseDTO>> Login(LoginCommand command)
        {
            var result = await _mediator.Send(command);
            return Ok(result);
        }

        [HttpGet("google")]
        public IActionResult Google([FromQuery] string mode)
        {
            var properties = new AuthenticationProperties
            {
                RedirectUri = $"/api/Auth/google-callback?mode={mode}"
            };

            return Challenge(
                properties,
                GoogleDefaults.AuthenticationScheme
            );
        }


        [HttpGet("google-callback")]
        public async Task<IActionResult> GoogleCallback(
            [FromQuery] string mode)
        {
            
            var frontendUrl =
                _configuration["Frontend:Url"];

            if (string.IsNullOrWhiteSpace(frontendUrl))
            {
                return BadRequest(
                    "Frontend URL is not configured."
                );
            }

            // Authenticate the Google response
            var result =
                await HttpContext.AuthenticateAsync(
                    GoogleDefaults.AuthenticationScheme
                );

            if (!result.Succeeded)
            {
                return Redirect(
                    $"{frontendUrl}/auth?googleError=true"
                );
            }

            var principal = result.Principal;

           
            var email =
                principal.FindFirst(
                    ClaimTypes.Email
                )?.Value;

            
            var fullName =
                principal.FindFirst(
                    ClaimTypes.Name
                )?.Value;

        
            var providerKey =
                principal.FindFirst(
                    ClaimTypes.NameIdentifier
                )?.Value;


            if (string.IsNullOrWhiteSpace(email))
            {
                return Redirect(
                    $"{frontendUrl}/auth?googleError=true"
                );
            }

            // REGISTER

            if (mode == "register")
            {
                var redirectUrl =
                    $"{frontendUrl}/auth" +
                    "?googleRegister=true" +
                    $"&fullName={Uri.EscapeDataString(
                        fullName ?? "")}" +
                    $"&email={Uri.EscapeDataString(
                        email)}" +
                    $"&providerKey={Uri.EscapeDataString(
                        providerKey ?? "")}";

                return Redirect(redirectUrl);
            }

            // LOGIN

            if (mode == "login")
            {
                try
                {
                    var response =
                        await _mediator.Send(
                            new GoogleLoginCommand(
                                Provider: "Google",
                                ProviderKey: providerKey!,
                                Email: email,
                                FullName: fullName ?? ""
                            )
                        );

                    var redirectUrl =
                        $"{frontendUrl}/auth" +
                        "?googleLogin=true" +
                        $"&token={Uri.EscapeDataString(
                            response.Token)}" +
                        $"&refreshToken={Uri.EscapeDataString(
                            response.RefreshToken)}" +
                        $"&userId={Uri.EscapeDataString(
                            response.UserId)}" +
                        $"&role={Uri.EscapeDataString(
                            response.Role)}";

                    return Redirect(redirectUrl);
                }
                catch (UnauthorizedAccessException)
                {
                    return Redirect(
                        $"{frontendUrl}/auth" +
                        "?googleError=true" +
                        "&message=" +
                        Uri.EscapeDataString(
                            "Google account is not registered. Please register first."
                        )
                    );
                }
            }

            // INVALID MODE


            return Redirect(
                $"{frontendUrl}/auth"
            );
        }


        [HttpGet("verify-email")]
        public async Task<IActionResult> VerifyEmail([FromQuery] string token)
        {
            try
            {
                var result = await _mediator.Send(
                    new VerifyEmailCommand(token));

                if (result.StartsWith(
                        "EMPLOYEE_PASSWORD_SETUP|"))
                {
                    var parts = result.Split('|', 3);

                    var userId = parts[1];
                    var passwordToken = parts[2];

                    var frontendUrl =
                        _configuration["Frontend:Url"];

                    if (string.IsNullOrWhiteSpace(frontendUrl))
                    {
                        return BadRequest(
                            "Frontend URL is not configured.");
                    }

                    var redirectUrl =
                        $"{frontendUrl}/auth/set-password" +
                        $"?userId={Uri.EscapeDataString(userId)}" +
                        $"&token={passwordToken}";

                    return Redirect(redirectUrl);
                }

                return Ok(new
                {
                    message = result
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    message = ex.Message
                });
            }
        }
        [HttpPost("register-employee")]
        public async Task<ActionResult<AuthResponseDTO>> RegisterEmployee([FromBody] RegisterEmployeeCommand command)
        {
            try
            {
                var response = await _mediator.Send(command);

                return Ok(response);
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    Message = ex.Message,
                    InnerException = ex.InnerException?.Message,
                    StackTrace = ex.StackTrace
                });
            }
        }

        [HttpPost("set-employee-password")]
        public async Task<IActionResult> SetEmployeePassword([FromBody] SetEmployeePasswordCommand command)
        {
            try
            {
                var result = await _mediator.Send(command);

                return Ok(new
                {
                    message = result
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    message = ex.Message
                });
            }
        }
        [HttpPost("enable-2fa")]
        public async Task<IActionResult> EnableTwoFactor()
        {
            var result = await _mediator.Send(new EnableTwoFactorCommand());

            return Ok(result);
        }

        [HttpPost("disable-2fa")]
        public async Task<IActionResult> DisableTwoFactor()
        {
            var result = await _mediator.Send(new DisableTwoFactorCommand());

            return Ok(result);
        }

        [HttpGet("2fa-status")]
        public async Task<IActionResult> GetTwoFactorStatus()
        {
            var result = await _mediator.Send(new GetTwoFactorStatusQuery());

            return Ok(result);
        }

        [HttpPost("verify-email-otp")]
        public async Task<IActionResult> VerifyEmailOtp(VerifyEmailOtpCommand command)
        {
            var result = await _mediator.Send(command);

            return Ok(result);
        }
        [HttpDelete("account")]
        public async Task<IActionResult> DeleteAccount(CancellationToken cancellationToken)
        {
            await _mediator.Send(
                new DeleteAccountCommand(),
                cancellationToken
            );

            return NoContent();
        }
    }
}
