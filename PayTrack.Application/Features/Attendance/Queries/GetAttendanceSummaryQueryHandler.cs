using MediatR;
using PayTrack.Application.DTOs;
using PayTrack.Application.Interfaces.Repositories;
using PayTrack.Application.Interfaces.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PayTrack.Application.Features.Attendance.Queries
{
    public class GetAttendanceSummaryQueryHandler : IRequestHandler<GetAttendanceSummaryQuery, AttendanceSummaryDTO>
    {
        private readonly IAttendanceRepository _attendanceRepository;
        private readonly ICurrentUserService _currentUserService;
        public GetAttendanceSummaryQueryHandler(IAttendanceRepository attendanceRepository, ICurrentUserService currentUserService)
        {
            _attendanceRepository = attendanceRepository;
            _currentUserService = currentUserService;
        }
        public async Task<AttendanceSummaryDTO> Handle(GetAttendanceSummaryQuery request, CancellationToken cancellationToken)
        {
            var result = await _attendanceRepository.GetAttendanceSummaryAsync(_currentUserService.UserId, request.Month, request.Year);

            var totalEmployees =
                result.Present +
                result.Late +
                result.Absent;

            var attendanceRate = totalEmployees == 0
                ? 0
                : Math.Round(
                    ((decimal)(result.Present + result.Late) / totalEmployees) * 100,
                    2);

            return new AttendanceSummaryDTO(
                result.Present,
                result.Late,
                result.Absent,
                attendanceRate);
        }
    }
}
