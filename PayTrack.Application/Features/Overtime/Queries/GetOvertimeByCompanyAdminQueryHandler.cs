using MediatR;
using Microsoft.EntityFrameworkCore;
using PayTrack.Application.DTOs;
using PayTrack.Application.Interfaces.Repositories;
using PayTrack.Application.Interfaces.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PayTrack.Application.Features.Overtime.Queries
{
    public class GetOvertimeByCompanyAdminQueryHandler
        : IRequestHandler<
            GetOvertimeByCompanyAdminQuery,
            PagedResult<OvertimeDTO>>
    {
        private readonly IOvertimeRepository _overtimeRepository;
        private readonly ICurrentUserService _currentUserService;

        private const int DefaultPageSize = 7;
        private const int MaxPageSize = 20;

        public GetOvertimeByCompanyAdminQueryHandler(
            IOvertimeRepository overtimeRepository,
            ICurrentUserService currentUserService)
        {
            _overtimeRepository = overtimeRepository;
            _currentUserService = currentUserService;
        }

        public async Task<PagedResult<OvertimeDTO>> Handle(
            GetOvertimeByCompanyAdminQuery request,
            CancellationToken cancellationToken)
        {
            // ==========================================
            // GET COMPANY ADMIN OVERTIME
            // ==========================================

            var query =
                _overtimeRepository
                    .GetOvertimeQueryByCompanyAdmin(
                        _currentUserService.UserId
                    );


            // ==========================================
            // SEARCH
            // ==========================================

            if (!string.IsNullOrWhiteSpace(request.Search))
            {
                var term = request.Search.Trim();

                query = query.Where(o =>

                    EF.Functions.Like(
                        o.Employee.ApplicationUser.FullName,
                        $"%{term}%"
                    )

                    ||

                    EF.Functions.Like(
                        o.Employee.EmployeeNumber,
                        $"%{term}%"
                    )

                    ||

                    (
                        o.Employee.Department != null
                        &&
                        EF.Functions.Like(
                            o.Employee.Department.DepartmentName,
                            $"%{term}%"
                        )
                    )

                );
            }


            // ==========================================
            // TOTAL COUNT
            // ==========================================

            var totalCount =
                await query.CountAsync(
                    cancellationToken
                );


            // ==========================================
            // PAGE SIZE
            // ==========================================

            int pageSize =
                request.PageSize.HasValue
                    ? Math.Min(
                        request.PageSize.Value,
                        MaxPageSize
                    )
                    : DefaultPageSize;


            // ==========================================
            // PAGE NUMBER
            // ==========================================

            int pageNumber =
                request.PageNumber <= 0
                    ? 1
                    : request.PageNumber;


            // ==========================================
            // PAGINATION + PROJECTION
            // ==========================================

            var items =
                await query

                    .OrderByDescending(
                        o => o.Date
                    )

                    .ThenByDescending(
                        o => o.StartTime
                    )

                    .ThenBy(
                        o =>
                            o.Employee
                                .ApplicationUser
                                .FullName
                    )

                    .Skip(
                        (pageNumber - 1) * pageSize
                    )

                    .Take(pageSize)

                    .Select(o =>
                        new OvertimeDTO(

                            o.Id,

                            o.EmployeeId,

                            o.Employee
                                .ApplicationUser
                                .FullName,

                            o.Employee
                                .EmployeeNumber,

                            o.Employee.Department != null
                                ? o.Employee
                                    .Department
                                    .DepartmentName
                                : "Unassigned",

                            o.Date,

                            o.StartTime,

                            o.EndTime,

                            o.RequestedHours,

                            o.ActualHours,

                            o.OvertimeRate,

                            o.OvertimePay,

                            o.Reason,

                            o.EmployeeRemarks,

                            o.AdminRemarks,

                            o.Source,

                            o.EmployeeResponse,

                            o.Status,

                            o.CreatedBy,

                            o.CreatedAt,

                            o.RespondedAt,

                            o.CompletedAt,

                            o.PayrollRecordId
                        )
                    )

                    .ToListAsync(
                        cancellationToken
                    );


            // ==========================================
            // RETURN
            // ==========================================

            return new PagedResult<OvertimeDTO>(
                items,
                totalCount,
                pageNumber,
                pageSize
            );
        }
    }
}
