using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
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

        [HttpGet("by-role/{roleName}")]
        public async Task<IActionResult> GetUsersByRole([FromRoute] string roleName, [FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 5, [FromQuery] string? search = null)
        {
            var result = await _mediator.Send(
                new GetUserByRoleQuery(roleName, pageNumber, pageSize, search)
            );

            return Ok(result);
        }
    }
}
