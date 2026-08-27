using MediatR;
using Microsoft.EntityFrameworkCore;
using PayTrack.Application.DTOs;
using PayTrack.Application.Interfaces.Repositories;
using PayTrack.Application.Interfaces.Services;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace PayTrack.Application.Features.Attendance.Queries
{
    public class GetMyAttendanceQueryHandler
        : IRequestHandler<GetMyAttendanceQuery, PagedResult<AttendanceDTO>>
    {
        private readonly IAttendanceRepository _attendanceRepository;
        private readonly ICurrentUserService _currentUserService;
        private readonly IEmployeesRepository _employeesRepository;

        private const int DefaultPageSize = 7;
        private const int MaxPageSize = 20;

        public GetMyAttendanceQueryHandler(
            IAttendanceRepository attendanceRepository,
            ICurrentUserService currentUserService,
            IEmployeesRepository employeesRepository)
        {
            _attendanceRepository = attendanceRepository;
            _currentUserService = currentUserService;
            _employeesRepository = employeesRepository;
        }

        public async Task<PagedResult<AttendanceDTO>> Handle(
            GetMyAttendanceQuery request,
            CancellationToken cancellationToken)
        {
            // ============================================================
            // GET CURRENT EMPLOYEE
            // ============================================================

            var employee =
                await _employeesRepository
                    .GetCurrentEmployeeByApplicationUserIdAsync(
                        _currentUserService.UserId);

            if (employee == null)
            {
                throw new InvalidOperationException(
                    "Employee record not found");
            }


            // ============================================================
            // GET ATTENDANCE QUERY
            // ============================================================

            var query =
                _attendanceRepository
                    .GetAttendancesByEmployeeId(employee.Id);


            // ============================================================
            // MONTH / YEAR FILTER
            // ============================================================

            // If month/year are not supplied,
            // use the current month and year.

            var month =
                request.Month.HasValue
                    ? request.Month.Value
                    : DateTime.Now.Month;

            var year =
                request.Year.HasValue
                    ? request.Year.Value
                    : DateTime.Now.Year;


            // Validate month

            if (month < 1 || month > 12)
            {
                throw new ArgumentException(
                    "Month must be between 1 and 12.");
            }


            // Validate year

            if (year < 1)
            {
                throw new ArgumentException(
                    "Invalid year.");
            }


            // Start of selected month

            var startDate =
                new DateTime(
                    year,
                    month,
                    1);


            // Start of next month

            var endDate =
                startDate.AddMonths(1);


            // Filter attendance by selected month

            query = query.Where(x =>
                (
                    x.TimeIn.HasValue &&
                    x.TimeIn.Value >= startDate &&
                    x.TimeIn.Value < endDate
                )
                ||
                (
                    !x.TimeIn.HasValue &&
                    x.TimeOut.HasValue &&
                    x.TimeOut.Value >= startDate &&
                    x.TimeOut.Value < endDate
                )
            );


            // ============================================================
            // SEARCH
            // ============================================================

            if (!string.IsNullOrWhiteSpace(request.Search))
            {
                var term =
                    request.Search.Trim();

                query = query.Where(x =>
                    EF.Functions.Like(
                        x.Employee.ApplicationUser.FullName,
                        $"%{term}%"
                    )
                );
            }


            // ============================================================
            // TOTAL COUNT
            // ============================================================

            var totalCount =
                await query.CountAsync(
                    cancellationToken);


            // ============================================================
            // PAGE SIZE
            // ============================================================

            int pageSize =
                request.PageSize.HasValue
                    ? Math.Min(
                        request.PageSize.Value,
                        MaxPageSize)
                    : DefaultPageSize;


            // Prevent invalid page size

            if (pageSize <= 0)
            {
                pageSize = DefaultPageSize;
            }


            // ============================================================
            // PAGE NUMBER
            // ============================================================

            int pageNumber =
                request.PageNumber <= 0
                    ? 1
                    : request.PageNumber;


            // ============================================================
            // PAGINATION + PROJECTION
            // ============================================================

            var items =
                await query
                    .OrderByDescending(x => x.TimeIn)
                    .ThenByDescending(x => x.TimeOut)

                    .Skip(
                        (pageNumber - 1) *
                        pageSize)

                    .Take(pageSize)

                    .Select(x =>
                        new AttendanceDTO(
                            x.Id,
                            x.TimeIn.HasValue
                                ? x.TimeIn.Value.Date
                                : x.TimeOut.HasValue
                                    ? x.TimeOut.Value.Date
                                    : DateTime.MinValue,

                            x.Employee.ApplicationUser.ProfilePicture != null
                                ? $"data:image/png;base64,{Convert.ToBase64String(
                                    x.Employee.ApplicationUser.ProfilePicture
                                )}"
                                : null,
                            x.Employee.ApplicationUser.FullName,
                            x.Employee.Department != null
                                ? x.Employee.Department.DepartmentName
                                : null,
                            x.Employee.EmployeeNumber,
                            x.TimeIn,
                            x.TimeOut,

                            x.Status.ToString()
                        )
                    )

                    .ToListAsync(
                        cancellationToken);


            return new PagedResult<AttendanceDTO>(
                items,
                totalCount,
                pageNumber,
                pageSize
            );
        }
    }
}