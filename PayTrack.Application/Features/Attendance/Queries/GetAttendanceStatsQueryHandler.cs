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
    internal class GetAttendanceStatsQueryHandler : IRequestHandler<GetAttendanceStatsQuery, AttendanceStatsDTO>
    {
        private readonly IAttendanceRepository _attendanceRepository;
        private readonly IEmployeesRepository _employeesRepository;
        private readonly ICurrentUserService _currentUserService;
        
        public GetAttendanceStatsQueryHandler(IAttendanceRepository attendanceRepository, IEmployeesRepository employeesRepository, ICurrentUserService currentUserService)
        {
            _attendanceRepository = attendanceRepository;
            _employeesRepository = employeesRepository;
            _currentUserService = currentUserService;
        }
        public async Task<AttendanceStatsDTO> Handle(GetAttendanceStatsQuery request, CancellationToken cancellationToken)
        {
            var companyAdminId = _currentUserService.UserId;

            var presentToday = await _attendanceRepository
                .GetPresentTodayCountAsync(companyAdminId);

            var lateToday = await _attendanceRepository
                .GetLateTodayCountAsync(companyAdminId);

            var absentToday = await _attendanceRepository
                .GetAbsentTodayCountAsync(companyAdminId);

            return new AttendanceStatsDTO(
                PresentToday: presentToday,
                LateToday: lateToday,
                AbsentToday: absentToday
            );
        }
    }
}
