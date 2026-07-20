using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PayTrack.Application.Features.SalaryAdvance.Commands;
using PayTrack.Application.Features.SalaryAdvance.Queries;

namespace PayTrack.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SalaryAdvanceController : ControllerBase
    {
        private readonly IMediator _mediator;

        public SalaryAdvanceController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost("request")]
        public async Task<IActionResult> RequestSalaryAdvance( CreateSalaryAdvanceRequestCommand command)
        {
            var id = await _mediator.Send(command);

            return Ok(id);
        }

        [HttpGet("pending-requests")]
        public async Task<IActionResult> GetPendingSalaryAdvances([FromQuery] GetPendingSalaryAdvanceRequestsQuery query)
        {
            var result = await _mediator.Send(query);
            return Ok(result);
        }
        [HttpGet]
        public async Task<IActionResult> GetSalaryAdvancesByEmployee([FromQuery] GetCurrentEmployeeSalaryAdvancesQuery query)
        {
            var result = await _mediator.Send(query);
            return Ok(result);
        }


        [HttpPost("scan")]
        public async Task<IActionResult> ScanEmployeeQr(
            [FromBody] ScanEmployeeQrForAdvanceQuery query)
        {
            var result = await _mediator.Send(query);

            return Ok(result);
        }

        [HttpPut("{id}/approve")]
        public async Task<IActionResult> Approve(Guid id)
        {

            var result =
                await _mediator.Send(
                    new ApproveSalaryAdvanceCommand(id));


            return Ok(new
            {
                message = "Salary advance approved.",
                success = result
            });


        }

    }
}
