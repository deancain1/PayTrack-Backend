using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PayTrack.Application.DTOs;
using PayTrack.Application.Features.Attendance.Commands;

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
        public async Task<IActionResult> Scan(AttendanceDTO attendanceDTO)
        {
            var attendanceId =
                await _mediator.Send(
                    new RecordAttendanceCommand(
                        attendanceDTO.Id,
                        attendanceDTO.EmployeeNumber,
                        attendanceDTO.PhotoBase64));

            return Ok(new
            {
                AttendanceId = attendanceId,
                Message = "Attendance recorded successfully."
            });
        }
    }
}
