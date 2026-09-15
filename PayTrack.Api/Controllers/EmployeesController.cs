using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PayTrack.Application.Features.Employee.Commands;
using PayTrack.Application.Features.Employee.Queries;

namespace PayTrack.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EmployeesController : ControllerBase
    {
        private readonly IMediator _mediator;

        public EmployeesController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [Authorize(Roles = "CompanyAdmin")]
        [HttpPost("{id}/generate-qr")]
        public async Task<IActionResult> GenerateQr(Guid id)
        {
            var result = await _mediator.Send(
                new GenerateEmployeeQrCommand(id));

            return Ok(result);
        }

        [Authorize(Roles = "CompanyAdmin")]
        [HttpGet]
        public async Task<IActionResult> GetEmployees([FromQuery] GetEmployeesQuery query)
        {
            var result = await _mediator.Send(query);

            return Ok(result);
        }

        [Authorize(Roles = "CompanyAdmin")]
        [HttpGet("lookup")]
        public async Task<IActionResult> GetEmployeesLookup()
        {
            var result = await _mediator.Send(
                new GetEmployeesLookupQuery());

            return Ok(result);
        }

        [Authorize(Roles = "CompanyAdmin")]
        [HttpGet("employee-summary")]
        public async Task<IActionResult> GetEmployeeSummary()
        {
            var result = await _mediator.Send(
                new GetTotalEmployeesQuery());

            return Ok(result);
        }

        [Authorize(Roles = "Employee")]
        [HttpGet("me")]
        public async Task<IActionResult> GetCurrentEmployee()
        {
            var employee = await _mediator.Send(
                new GetCurrentEmployeeInformationQuery());

            return Ok(employee);
        }

        [Authorize(Roles = "CompanyAdmin")]
        [HttpPut("{id:guid}")]
        public async Task<IActionResult> UpdateEmployee(Guid id, [FromBody] UpdateEmployeeCommand command, CancellationToken cancellationToken)
        {
            if (id != command.Id)
            {
                return BadRequest(new
                {
                    message = "Employee ID does not match the request."
                });
            }

            try
            {
                var result = await _mediator.Send(
                    command,
                    cancellationToken);

                return Ok(result);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new
                {
                    message = ex.Message
                });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new
                {
                    message = ex.Message
                });
            }
        }
    }
}