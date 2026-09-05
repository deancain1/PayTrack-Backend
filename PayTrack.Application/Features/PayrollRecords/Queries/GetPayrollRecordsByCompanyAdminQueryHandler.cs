using MediatR;
using Microsoft.EntityFrameworkCore;
using PayTrack.Application.DTOs;
using PayTrack.Application.Interfaces.Repositories;
using PayTrack.Application.Interfaces.Services;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace PayTrack.Application.Features.PayrollRecords.Queries
{
    public class GetPayrollRecordsByCompanyAdminQueryHandler: IRequestHandler<GetPayrollRecordsByCompanyAdminQuery, PagedResult<PayrollRecordDTO>>
    {
        private readonly IPayrollRecordRepository _payrollRecordRepository;
        private readonly ICurrentUserService _currentUserService;

        private const int DefaultPageSize = 7;
        private const int MaxPageSize = 20;

        public GetPayrollRecordsByCompanyAdminQueryHandler(
            IPayrollRecordRepository payrollRecordRepository,
            ICurrentUserService currentUserService)
        {
            _payrollRecordRepository = payrollRecordRepository;
            _payrollRecordRepository = payrollRecordRepository;
            _currentUserService = currentUserService;
        }

        public async Task<PagedResult<PayrollRecordDTO>> Handle(GetPayrollRecordsByCompanyAdminQuery request, CancellationToken cancellationToken)
        {
            var query =
                _payrollRecordRepository
                    .GetPayrollRecordByCompanyAdminsync(
                        _currentUserService.UserId);

            // MONTH / YEAR FILTER

            var month =
                request.Month.HasValue
                    ? request.Month.Value
                    : DateTime.UtcNow.Month;

            var year =
                request.Year.HasValue
                    ? request.Year.Value
                    : DateTime.UtcNow.Year;

            if (month < 1 || month > 12)
            {
                throw new ArgumentException(
                    "Month must be between 1 and 12.");
            }

            if (year < 1)
            {
                throw new ArgumentException(
                    "Invalid year.");
            }

          

            var startDate = new DateTime(
                year,
                month,
                1,
                0,
                0,
                0,
                DateTimeKind.Utc);

            // START OF NEXT MONTH

            var endDate = startDate.AddMonths(1);

            // FILTER PAYROLL RECORDS BY MONTH / YEAR

            query = query.Where(x =>
                x.Attendance.TimeIn.HasValue &&
                x.Attendance.TimeIn.Value >= startDate &&
                x.Attendance.TimeIn.Value < endDate
            );

            // SEARCH FILTER

            if (!string.IsNullOrWhiteSpace(request.Search))
            {
                var term = request.Search.Trim();

                query = query.Where(s =>
                    EF.Functions.Like(
                        s.Employee.ApplicationUser.FullName,
                        $"%{term}%")
                    ||
                    EF.Functions.Like(
                        s.Employee.EmployeeNumber,
                        $"%{term}%"));
            }

            // TOTAL COUNT

            var totalCount =
                await query.CountAsync(
                    cancellationToken);

            // PAGE SIZE

            int pageSize =
                request.PageSize.HasValue
                    ? Math.Min(
                        request.PageSize.Value,
                        MaxPageSize)
                    : DefaultPageSize;

            // PREVENT INVALID PAGE SIZE

            if (pageSize <= 0)
            {
                pageSize = DefaultPageSize;
            }

            // PAGE NUMBER

            int pageNumber =
                request.PageNumber <= 0
                    ? 1
                    : request.PageNumber;

            // GET PAGED RECORDS

            var items =
                await query
                    .OrderBy(
                        s =>
                            s.Employee
                                .ApplicationUser
                                .FullName)

                    .Skip(
                        (pageNumber - 1) *
                        pageSize)

                    .Take(pageSize)

                    .Select(
                        p =>
                            new PayrollRecordDTO(
                                p.Id,
                                p.EmployeeId,

                                p.Employee
                                    .ApplicationUser
                                    .FullName,

                                p.Employee
                                    .EmployeeNumber,

                                p.Employee.Department != null
                                    ? p.Employee
                                        .Department
                                        .DepartmentName
                                    : null,

                                p.Attendance.TimeIn,

                                p.Attendance.TimeOut,

                                p.Attendance.HoursWorked,

                                p.AmountEarned
                            )
                    )

                    .ToListAsync(
                        cancellationToken);

            // RETURN PAGED RESULT

            return new PagedResult<PayrollRecordDTO>(
                items,
                totalCount,
                pageNumber,
                pageSize);
        }
    }
}