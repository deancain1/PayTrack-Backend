using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PayTrack.Application.DTOs;
using PayTrack.Application.Features.Attendance.Commands;
using PayTrack.Application.Features.Attendance.Queries;
using PayTrack.Application.Features.AttendancePolicies.Queries;

namespace PayTrack.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AttendanceController : ControllerBase
    {
        private readonly IMediator _mediator;
        public AttendanceController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost("scan")]
        public async Task<IActionResult> ScanAttendance(
            [FromBody] RecordAttendanceCommand command)
        {
            try
            {
                var attendanceId =
                    await _mediator.Send(command);

                return Ok(new
                {
                    Success = true,
                    AttendanceId = attendanceId,
                    Message = "Attendance recorded successfully."
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    Success = false,
                    Message = ex.Message
                });
            } 
        }

        [HttpGet("my-attendance")]
        public async Task<IActionResult> GetMyAttendance([FromQuery] GetMyAttendanceQuery query)
        {
            var result = await _mediator.Send(query);
            return Ok(result);
        }

        [HttpGet("my-summary")]
        public async Task<IActionResult> GetMyAttendanceSummary([FromQuery] int month, [FromQuery] int year)
        {
            var result = await _mediator.Send(
                new GetMyAttendanceSummaryQuery(month, year));

            return Ok(result);
        }



        [HttpGet]
        public async Task<IActionResult> GetAttendance([FromQuery] GetAttendanceQuery query)
        {
            var result = await _mediator.Send(query);
            return Ok(result);

        }
        [HttpGet("today")]
        public async Task<IActionResult> GetTodayAttendance(CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(new GetAttendanceTodayQuery(), cancellationToken);
            return Ok(result);
        }

        [HttpGet("attendance-stats")]
        public async Task<IActionResult> GetAttendnceStats()
        {
            var result = await _mediator.Send(
                new GetAttendanceStatsQuery());

            return Ok(result);
        }
        [HttpGet("summary")]
        public async Task<IActionResult> GetAttendanceSummary([FromQuery] int month,[FromQuery] int year)
        {
            var result = await _mediator.Send(
                new GetAttendanceSummaryQuery(month, year));

            return Ok(result);
        }

        [HttpGet("my-weekly-progress")]
        public async Task<IActionResult> GetMyWeeklyProgress()
        {
            var result = await _mediator.Send(
                new GetMyWeeklyProgressQuery());

            return Ok(result);
        }
        //[HttpGet("missing-timein-reminders")]
        //public async Task<IActionResult> GetMissingTimeInReminder()
        //{
        //    var result = await _mediator.Send(
        //        new GetMissingTimeInReminderQuery());

        //    return Ok(result);
        //}
    }
}
