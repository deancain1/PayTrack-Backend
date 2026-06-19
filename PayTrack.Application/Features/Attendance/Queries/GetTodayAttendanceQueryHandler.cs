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
    public class GetTodayAttendanceQueryHandler: IRequestHandler<GetTodayAttendanceQuery, List<AttendanceDTO>>
    {
        private readonly IAttendanceRepository _attendanceRepository;

        public GetTodayAttendanceQueryHandler(IAttendanceRepository attendanceRepository)
        {
            _attendanceRepository = attendanceRepository;
        }

        public async Task<List<AttendanceDTO>> Handle( GetTodayAttendanceQuery request, CancellationToken cancellationToken)
        {
            var result = await _attendanceRepository.GetTodayAttendancesAsync();

            return result.Select(a => new AttendanceDTO(
                a.Id,
                a.Employee.EmployeeNumber,

                a.Employee.ApplicationUser.ProfilePicture != null
                    ? $"data:image/png;base64,{Convert.ToBase64String(a.Employee.ApplicationUser.ProfilePicture)}"
                    : null,

                a.Employee.ApplicationUser.FullName,

                a.TimeIn.ToString("yyyy-MM-dd HH:mm:ss"),

                a.TimeOut.HasValue
                    ? a.TimeOut.Value.ToString("yyyy-MM-dd HH:mm:ss")
                    : "-"
            )).ToList();
        }
    }
    }

