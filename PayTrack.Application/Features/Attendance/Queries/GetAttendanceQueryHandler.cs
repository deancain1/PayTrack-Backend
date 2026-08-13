using MediatR;
using Microsoft.EntityFrameworkCore;
using PayTrack.Application.DTOs;
using PayTrack.Application.Features.Employee.Queries;
using PayTrack.Application.Interfaces.Repositories;
using PayTrack.Application.Interfaces.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PayTrack.Application.Features.Attendance.Queries
{
    public class GetAttendanceQueryHandler : IRequestHandler<GetAttendanceQuery, PagedResult<AttendanceDTO>>
    {
        private readonly IAttendanceRepository _attendanceRepository;
        private readonly ICurrentUserService _currentUserService;

        private const int DefaultPageSize = 7;
        private const int MaxPageSize = 20;

        public GetAttendanceQueryHandler(IAttendanceRepository attendanceRepository, ICurrentUserService currentUserService)
        {
            _attendanceRepository = attendanceRepository;
            _currentUserService = currentUserService;
        }

        public async Task<PagedResult<AttendanceDTO>> Handle(GetAttendanceQuery request, CancellationToken cancellationToken)
        {
            var query = _attendanceRepository.GetAttendancesByCompanyAdminQuery(_currentUserService.UserId);

            // Search
            if (!string.IsNullOrWhiteSpace(request.Search))
            {
                var term = request.Search.Trim();

                query = query.Where(e =>
                    EF.Functions.Like(e.ApplicationUser.FullName, $"%{term}%") ||
                    EF.Functions.Like(e.ApplicationUser.Email!, $"%{term}%"));
            }

            // Total Count
            var totalCount = await query.CountAsync(cancellationToken);

            // Page Size
            int pageSize = request.PageSize.HasValue
                ? Math.Min(request.PageSize.Value, MaxPageSize)
                : DefaultPageSize;

            int pageNumber = request.PageNumber <= 0
                ? 1
                : request.PageNumber;

            // Pagination
            var items = await query

                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .Select(a => new AttendanceDTO(
                    a.Id,
                    a.Date,
                    a.Employee.ApplicationUser.ProfilePicture != null
                    ? $"data:image/png;base64,{Convert.ToBase64String(a.Employee.ApplicationUser.ProfilePicture)}"
                    : null,
                    a.Employee.ApplicationUser.FullName,
                    a.Employee.Department != null? a.Employee.Department.DepartmentName: null,
                    a.Employee.EmployeeNumber,
                    a.TimeIn,
                    a.TimeOut,
                    a.Status
                ))
                .ToListAsync(cancellationToken);

            return new PagedResult<AttendanceDTO>(
                items,
                totalCount,
                pageNumber,
                pageSize
            );
        }
    }
}
