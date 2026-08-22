using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PayTrack.Application.Features.Overtime.Commands;
using PayTrack.Application.Features.Overtime.Queries;

namespace PayTrack.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OvertimeController : ControllerBase
    {
        private readonly IMediator _mediator;

        public OvertimeController(IMediator mediator)
        {
            _mediator = mediator;
        }
        //Company Admin 
        [HttpPost("admin")]
        public async Task<IActionResult> CreateByAdmin(
            [FromBody] CreateCompanyAdminOvertimeCommand command)
        {
            var overtimeId = await _mediator.Send(command);

            return Ok(new
            {
                id = overtimeId,
                message = "Overtime request sent to the employee."
            });
        }

        [HttpGet("admin")]
        public async Task<IActionResult> GetAdminOvertime([FromQuery] GetOvertimeByCompanyAdminQuery query)
        {
            var result = await _mediator.Send(query);

            return Ok(result);
        }

        [HttpPut("{id}/approve")]
        public async Task<IActionResult> Approve(Guid id)
        {
            await _mediator.Send(
                new ApproveOvertimeCommand(id));

            return Ok(new
            {
                message = "Overtime request approved successfully."
            });
        }

        [HttpPut("{id}/reject")]
        public async Task<IActionResult> Reject(
            Guid id,
            [FromBody] RejectOvertimeCommand command)
        {
            if (id != command.Id)
            {
                return BadRequest(
                    "Overtime ID does not match.");
            }

            await _mediator.Send(command);

            return Ok(new
            {
                message = "Overtime request rejected successfully."
            });
        }


        //Employee
        [HttpPost("employee")]
        public async Task<IActionResult> CreateByEmployee(
            [FromBody] CreateEmployeeOvertimeCommand command)
        {
            var overtimeId = await _mediator.Send(command);

            return Ok(new
            {
                id = overtimeId,
                message = "Overtime request submitted successfully."
            });
        }

        [HttpPost("{id}/respond")]
        public async Task<IActionResult> Respond(
            Guid id,
            [FromBody] RespondToOvertimeCommand command)
        {
            if (id != command.OvertimeId)
            {
                return BadRequest(
                    "Overtime ID does not match.");
            }

            await _mediator.Send(command);

            return Ok(new
            {
                message = command.Accept
                    ? "Overtime request accepted successfully."
                    : "Overtime request rejected successfully."
            });
        }



        //[HttpPost("{id}/cancel")]
        //[Authorize]
        //public async Task<IActionResult> Cancel(
        //    Guid id,
        //    [FromBody] CancelOvertimeCommand command)
        //{
        //    if (id != command.OvertimeId)
        //    {
        //        return BadRequest(
        //            "Overtime ID does not match.");
        //    }

        //    await _mediator.Send(command);

        //    return Ok(new
        //    {
        //        message = "Overtime request cancelled successfully."
        //    });
        //}
    }
}

