using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PayTrack.Application.Features.PayrollBatches.Commands;
using PayTrack.Application.Features.PayrollBatches.Queries;

namespace PayTrack.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "CompanyAdmin")]
    public class PayrollBatchController : ControllerBase
    {
        private readonly IMediator _mediator;

        public PayrollBatchController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPut("{Id}/release")]
        public async Task<IActionResult> ReleasePayroll(Guid Id, CancellationToken cancellationToken)
        {
            var command = new ReleasePayrollBatchCommand(Id);

            var result = await _mediator.Send(
                command,
                cancellationToken);

            return Ok(result);
        }

        [HttpPut("{id:guid}/pay")]
        public async Task<IActionResult> PayBatch(Guid id, [FromBody] PayPayrollBatchCommand request)
        {
            var command = request with
            {
                BatchId = id
            };

            await _mediator.Send(command);

            return Ok();
        }

        [HttpGet("active")]
        public async Task<IActionResult> GetActiveBatch()
        {
            var result = await _mediator.Send(
                new GetActivePayrollBatchQuery());

            if (result == null)
                return NoContent();

            return Ok(result);
        }
    }
}