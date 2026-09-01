using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PayTrack.Application.Features.Users.Commands;
using PayTrack.Application.Features.Users.Queries;

namespace PayTrack.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly IMediator _mediator;
        public UsersController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet("current")]
        public async Task<IActionResult> GetCurrentUser()
        {
            var query = new GetCurrentUserQuery();
            var result = await _mediator.Send(query);
            return Ok(result);

        }

        [HttpPut("update-password")]
        public async Task<ActionResult<bool>> UpdatePassword(UpdatePasswordCommand command)
        {
            var success = await _mediator.Send(command);
            return Ok(success);
        }

        [HttpPut("update-profile")]
        public async Task<IActionResult> UpdateProfile([FromBody] UpdateProfileCommand command)
        {
            await _mediator.Send(command);

            return NoContent();
        }

        [HttpPut("deactivate-account")]
        public async Task<IActionResult> DeactivateAccount()
        {
            await _mediator.Send(new DeactivateAccountCommand());

            return Ok(new
            {
                Message = "Account has been deactivated successfully."
            });
        }
    }
}
