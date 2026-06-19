using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PayTrack.Application.Features.Payroll.Commands;

namespace PayTrack.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PayrollController : ControllerBase
    {
        private readonly IMediator _mediator;
        public PayrollController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost("generate")]
        public async Task<IActionResult> Generate([FromBody] GeneratePayrollCommand command)
        {
            var result = await _mediator.Send(command);
            return Ok(result);
        }
    }
}
