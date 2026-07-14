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

        [HttpPost("scan")]
        public async Task<IActionResult> ScanEmployeeQr(
            [FromBody] ScanEmployeeQrForAdvanceQuery query)
        {
            var result = await _mediator.Send(query);

            return Ok(result);
        }

        [HttpPost("salary-advance")]
        public async Task<IActionResult> Create(CreateSalaryAdvanceCommand command)
        {
            var id = await _mediator.Send(command);

            return Ok(new
            {
                Success = true,
                SalaryAdvanceId = id,
                Message = "Salary advance recorded successfully."
            });
        }
    }
}
