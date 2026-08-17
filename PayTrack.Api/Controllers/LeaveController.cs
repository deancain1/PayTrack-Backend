using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PayTrack.Application.DTOs;
using PayTrack.Application.Features.LeaveRequests.Commands;
using PayTrack.Application.Features.LeaveRequests.Queries;
using PayTrack.Application.Features.PayrollRecords.Queries;

namespace PayTrack.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LeaveController : ControllerBase
    {
        private readonly IMediator _mediator;

        public LeaveController(IMediator mediator)
        {
            _mediator = mediator;
        }

        // Employee
        [HttpPost("request")]
        public async Task<IActionResult> CreateRequest(
            [FromForm] CreateLeaveRequestCommand command)
        {
            var result = await _mediator.Send(command);

            return Ok(result);
        }

        [HttpGet("my-leaves")]
        public async Task<ActionResult<PagedResult<PayrollRecordDTO>>> GetParollRecordByEmployeeAsync([FromQuery] string? search, [FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 7)
        {
            var query = new GetMyLeavesQuery
            {
                Search = search,
                PageNumber = pageNumber,
                PageSize = pageSize
            };

            var result = await _mediator.Send(query);

            return Ok(result);

        }

        //[HttpPut("{id}/cancel")]
        //public async Task<IActionResult> Cancel(
        //    Guid id,
        //    CancellationToken cancellationToken)
        //{
        //    var result = await _mediator.Send(
        //        new CancelLeaveRequestCommand(id),
        //        cancellationToken);

        //    return Ok(new
        //    {
        //        message = "Leave request cancelled successfully.",
        //        success = result
        //    });
        //}

        // CompanyAdmin
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(
            Guid id,
            CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(
                new GetLeaveRequestByIdQuery(id),
                cancellationToken);

            return Ok(result);
        }

        [HttpGet]
        public async Task<IActionResult> GetLeaveRequests([FromQuery] string? search, [FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 7)
        {
            var result = await _mediator.Send(
                new GetLeaveRequestsQuery(
                    search,
                    pageNumber,
                    pageSize
                ));

            return Ok(result);
        }

        [HttpPut("{id}/approve")]
        public async Task<IActionResult> Approve(Guid id, [FromBody] ApproveLeaveRequestCommand command)
        {
            var result = await _mediator.Send(
                command with
                {
                    LeaveRequestId = id
                });

            return Ok(new
            {
                message = "Leave request approved successfully.",
                success = result
            });
        }

        [HttpPut("{id}/reject")]
        public async Task<IActionResult> Reject(Guid id, [FromBody] RejectLeaveRequestCommand command)
        {
            var result = await _mediator.Send(
                command with
                {
                    LeaveRequestId = id
                });

            return Ok(new
            {
                message = "Leave request rejected successfully.",
                success = result
            });
        }

        [HttpGet("summary")]
        public async Task<IActionResult> GetLeaveRequestSummary()
        {
            var result = await _mediator.Send(
                new GetLeaveRequestSummaryQuery());

            return Ok(result);
        }
    }
}
