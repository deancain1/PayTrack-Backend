using MediatR;
using Microsoft.AspNetCore.Mvc;
using PayTrack.Application.Features.AttendancePolicies.Commands;
using PayTrack.Application.Features.AttendancePolicies.Queries;
using PayTrack.Application.Features.Employee.Queries;
using PayTrack.Application.Features.WorkSchedule.Commands;

namespace PayTrack.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AttendancePolicyController : ControllerBase
    {
        private readonly IMediator _mediator;

        public AttendancePolicyController(IMediator mediator)
        {
            _mediator = mediator;
        }



        [HttpGet("attendance-policy-lookup")]
        public async Task<IActionResult> GetAttendancePolicyLookup()
        {
            var result = await _mediator.Send(
                new GetAttendancePolicyLookupQuery());

            return Ok(result);
        }


        [HttpPost]
        public async Task<IActionResult> Create(
            [FromBody] CreateAttendancePolicyCommand command)
        {
            var id = await _mediator.Send(command);

            return Ok(id);
        }

        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            await _mediator.Send(
                new DeleteAttendancePolicyCommand(id));

            return NoContent();
        }

        [HttpPut("assign-attendance-policy")]
        public async Task<IActionResult> AssignAttendancePolicyToEmployee(AssignAttendancePolicyCommand command)
        {
            await _mediator.Send(command);
            return NoContent();
        }
    }
}