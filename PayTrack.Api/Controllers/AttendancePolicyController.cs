using MediatR;
using Microsoft.AspNetCore.Mvc;
using PayTrack.Application.Features.AttendancePolicies.Commands;
using PayTrack.Application.Features.AttendancePolicies.Queries;

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

        // ==========================================
        // Get all attendance policies of a company
        // ==========================================
        [HttpGet("{companyAdminId}")]
        public async Task<IActionResult> GetAll(string companyAdminId)
        {
            var result = await _mediator.Send(
                new GetAttendancePoliciesQuery(companyAdminId));

            return Ok(result);
        }

        // ==========================================
        // Get attendance policy by Id
        // ==========================================
        [HttpGet("details/{id:guid}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var result = await _mediator.Send(
                new GetAttendancePolicyByIdQuery(id));

            if (result == null)
                return NotFound();

            return Ok(result);
        }

        // ==========================================
        // Lookup for Employee Dropdown
        // ==========================================
        [HttpGet("lookup/{companyAdminId}")]
        public async Task<IActionResult> Lookup(string companyAdminId)
        {
            var result = await _mediator.Send(
                new GetAttendancePolicyLookupQuery(companyAdminId));

            return Ok(result);
        }

        // ==========================================
        // Create Attendance Policy
        // ==========================================
        [HttpPost]
        public async Task<IActionResult> Create(
            [FromBody] CreateAttendancePolicyCommand command)
        {
            var id = await _mediator.Send(command);

            return Ok(id);
        }

        // ==========================================
        // Update Attendance Policy
        // ==========================================
        [HttpPut]
        public async Task<IActionResult> Update(
            [FromBody] UpdateAttendancePolicyCommand command)
        {
            await _mediator.Send(command);

            return NoContent();
        }

        // ==========================================
        // Delete Attendance Policy
        // ==========================================
        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            await _mediator.Send(
                new DeleteAttendancePolicyCommand(id));

            return NoContent();
        }
    }
}