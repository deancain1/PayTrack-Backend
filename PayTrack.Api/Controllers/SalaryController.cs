using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PayTrack.Application.DTOs;
using PayTrack.Application.Features.Salary.Commands;
using PayTrack.Application.Features.Salary.Queries;

namespace PayTrack.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SalaryController : ControllerBase
    {
        private readonly IMediator _mediator;

        public SalaryController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost("setup")]
        public async Task<ActionResult<SalaryDTO>> SetupSalary([FromBody] CreateSalaryCommand command)
        {
            var salary = await _mediator.Send(command);

            return Ok(salary);
        }
        [HttpGet]
        public async Task<ActionResult<PagedResult<SalaryDTO>>> GetSalaries([FromQuery] string? search, [FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 7)
        {
            var query = new GetEmployeeSalaryQuery
            {
                Search = search,
                PageNumber = pageNumber,
                PageSize = pageSize
            };

            var result = await _mediator.Send(query);

            return Ok(result);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(Guid id,[FromBody] UpdateEmployeeSalaryCommand command)
        {
            if (id != command.Id)
                return BadRequest("Salary ID mismatch.");

            var result = await _mediator.Send(command);

            if (!result)
                return NotFound();

            return NoContent();
        }
        [HttpGet("summary")]
        public async Task<IActionResult> GetSummary()
        {
            var result = await _mediator.Send(
                new GetSalarySummaryQuery());

            return Ok(result);
        }

        [HttpGet("daily-rate")]
        public async Task<IActionResult> GetDailyRate()
        {
            var result = await _mediator.Send(new GetDailyRateQuery());

            return Ok(result);
        }
        [HttpDelete("bulk-delete")]
        public async Task<IActionResult> BulkDelete([FromBody] DeleteEmployeeSalariesCommand command)
        {
            await _mediator.Send(command);

            return NoContent();
        }
    }
}

