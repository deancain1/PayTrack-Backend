using MediatR;
using Microsoft.EntityFrameworkCore;
using PayTrack.Application.DTOs;
using PayTrack.Application.Interfaces.Repositories;
using PayTrack.Application.Interfaces.Services;

namespace PayTrack.Application.Features.Attendance.Queries
{
    public class GetMyAttendanceSummaryQueryHandler
        : IRequestHandler<GetMyAttendanceSummaryQuery, MyAttendanceSummaryDTO>
    {
        private readonly IAttendanceRepository _attendanceRepository;
        private readonly ICurrentUserService _currentUserService;
        private readonly IEmployeesRepository _employeesRepository;

        public GetMyAttendanceSummaryQueryHandler(
            IAttendanceRepository attendanceRepository,
            ICurrentUserService currentUserService,
            IEmployeesRepository employeesRepository)
        {
            _attendanceRepository = attendanceRepository;
            _currentUserService = currentUserService;
            _employeesRepository = employeesRepository;
        }

        public async Task<MyAttendanceSummaryDTO> Handle(GetMyAttendanceSummaryQuery request, CancellationToken cancellationToken)
        {
       
            var employee =
                await _employeesRepository
                    .GetCurrentEmployeeByApplicationUserIdAsync(
                        _currentUserService.UserId);

            if (employee == null)
            {
                throw new InvalidOperationException(
                    "Employee record not found.");
            }

         

            if (request.Month < 1 || request.Month > 12)
            {
                throw new ArgumentException(
                    "Month must be between 1 and 12.");
            }

            if (request.Year < 1)
            {
                throw new ArgumentException(
                    "Invalid year.");
            }

            // Create UTC date range for PostgreSQL timestamp with time zone

            var startDate = new DateTime(
                request.Year,
                request.Month,
                1,
                0,
                0,
                0,
                DateTimeKind.Utc);

            var endDate = startDate.AddMonths(1);

       

            var records = await _attendanceRepository
                .GetAttendancesByEmployeeId(employee.Id)
                .Where(x =>
                    x.TimeIn.HasValue &&
                    x.TimeIn.Value >= startDate &&
                    x.TimeIn.Value < endDate)
                .ToListAsync(cancellationToken);

     

            var totalPresent = records.Count(x =>
                x.Status.ToString()
                    .Equals(
                        "Present",
                        StringComparison.OrdinalIgnoreCase));

    

            var totalLate = records.Count(x =>
                x.Status.ToString()
                    .Equals(
                        "Late",
                        StringComparison.OrdinalIgnoreCase));

    
            var totalAbsent = records.Count(x =>
                x.Status.ToString()
                    .Equals(
                        "Absent",
                        StringComparison.OrdinalIgnoreCase));

     

            var hours = records
                .Where(x =>
                    x.TimeIn.HasValue &&
                    x.TimeOut.HasValue)
                .Select(x =>
                    (x.TimeOut!.Value - x.TimeIn!.Value)
                    .TotalHours)
                .Where(x => x > 0)
                .ToList();

            var averageHours =
                hours.Count > 0
                    ? hours.Average()
                    : 0;


            var totalAttendanceDays =
                totalPresent +
                totalLate +
                totalAbsent;

            var attendanceRate =
                totalAttendanceDays > 0
                    ? Math.Round(
                        ((double)(totalPresent + totalLate)
                        / totalAttendanceDays) * 100,
                        2)
                    : 0;

  

            return new MyAttendanceSummaryDTO(
                totalPresent,
                totalLate,
                totalAbsent,
                Math.Round(averageHours, 2)
            );
        }
    }
}