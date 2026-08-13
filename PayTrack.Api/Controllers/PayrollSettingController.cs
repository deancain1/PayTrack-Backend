using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PayTrack.Application.Features.PayrollSettings.Commands;
using PayTrack.Application.Features.PayrollSettings.Queries;

namespace PayTrack.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PayrollSettingController : ControllerBase
    {
        private readonly IMediator _mediator;
        public PayrollSettingController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost("payroll-setting")]
        public async Task<IActionResult> SetPayrollSetting(SetPayrollSettingCommand command)
        {
            var result = await _mediator.Send(command);

            return Ok(result);
        }

        [HttpGet("payroll-setting")]
        public async Task<IActionResult> GetPayrollSetting()
        {
            var result =
                await _mediator.Send(
                    new GetPayrollSettingQuery());

            return Ok(result);
        }
    }

}
