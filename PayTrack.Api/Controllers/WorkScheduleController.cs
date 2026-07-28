using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PayTrack.Application.DTOs;
using PayTrack.Application.Features.Department.Commands;
using PayTrack.Application.Features.Employee.Queries;
using PayTrack.Application.Features.WorkSchedule.Commands;
using PayTrack.Application.Features.WorkSchedule.Queries;
using System.Security.Claims;

namespace PayTrack.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class WorkScheduleController : ControllerBase
    {
        private readonly IMediator _mediator;
        public WorkScheduleController(IMediator mediator)
        {
            _mediator = mediator;
        }
        [HttpPost("work-schedule")]
        public async Task<IActionResult> Create(CreateWorkScheduleCommand command)
        {
            var id = await _mediator.Send(command);

            return Ok(id);
        }

        [HttpGet]
        public async Task<IActionResult> GetSchedules([FromQuery] GetSchedulesQuery query)
        {
            var result = await _mediator.Send(query);
            return Ok(result);
        }

        [HttpPut("assign-schedule")]
        public async Task<IActionResult> AssignScheduleToEmployee(AssignWorkScheduleToEmployeeCommand command)
        {
            await _mediator.Send(command);
            return NoContent();
        }

        [HttpGet("lookup")]
        public async Task<IActionResult> GetLookup()
        {
            var result = await _mediator.Send(
                new GetSchedulesLookupQuery());

            return Ok(result);
        }
        [HttpDelete("bulk-delete")]
        public async Task<IActionResult> BulkDelete([FromBody] DeleteWorkSchedulesCommand command)
        {
            await _mediator.Send(command);

            return NoContent();
        }
    }
}

