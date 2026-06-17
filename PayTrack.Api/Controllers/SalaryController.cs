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
        public async Task<IActionResult> SetupSalary(
            [FromBody] CreateSalaryDTO createSalaryDTO)
        {
            var result = await _mediator.Send(
                new CreateOrUpdateSalaryCommand(createSalaryDTO));

            return Ok(new
            {
                Success = true,
                SalaryId = result,
                Message = "Salary setup saved successfully."
            });
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
    }
}

