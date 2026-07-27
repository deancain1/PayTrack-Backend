using MediatR;
using PayTrack.Application.DTOs;
using PayTrack.Application.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PayTrack.Application.Features.Attendance.Queries
{
    public class GetAttendanceTodayQueryHandler : IRequestHandler<GetAttendanceTodayQuery, List<AttendanceDTO>>
    {
        private readonly IAttendanceRepository _attendanceRepository;
        private readonly ICurrentUserService _currentUserService;

        public GetAttendanceTodayQueryHandler(IAttendanceRepository attendanceRepository, ICurrentUserService currentUserService)
        {
            _attendanceRepository = attendanceRepository;
            _currentUserService = currentUserService;
        }

        public async Task<List<AttendanceDTO>> Handle(GetAttendanceTodayQuery request, CancellationToken cancellationToken)
        {
            var companyAdminId = _currentUserService.UserId;

            var result = await _attendanceRepository
            .GetTodayAttendancesAsync(companyAdminId);
          

            return result.Select(a => new AttendanceDTO(
                a.Id,
                a.Date,
                a.Employee.ApplicationUser.ProfilePicture != null
                    ? $"data:image/png;base64,{Convert.ToBase64String(a.Employee.ApplicationUser.ProfilePicture)}"
                    : null,

                a.Employee.ApplicationUser.FullName,
                a.Employee.Department != null ? a.Employee.Department.DepartmentName : null,
                a.Employee.EmployeeNumber,
                a.TimeIn,
                a.TimeOut,
                a.Status
            )).ToList();
        }
    }
}