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
    public class GetOvertimeByEmployeeQueryHandler : IRequestHandler<GetOvertimeByEmployeeQuery, PagedResult<EmployeeOvertimeDTO>>
    {
        private readonly IOvertimeRepository _overtimeRepository;
        private readonly ICurrentUserService _currentUserService;
        private const int DefaultPageSize = 7;
        private const int MaxPageSize = 20;
        public GetOvertimeByEmployeeQueryHandler(IOvertimeRepository overtimeRepository, ICurrentUserService currentUserService )
        {
            _overtimeRepository = overtimeRepository;
            _currentUserService = currentUserService;
        }
        public async Task<PagedResult<EmployeeOvertimeDTO>> Handle(GetOvertimeByEmployeeQuery request, CancellationToken cancellationToken)
        {
            var query = _overtimeRepository.GetOvertimeQueryByEmployee(_currentUserService.UserId);

            // Search
            if (!string.IsNullOrWhiteSpace(request.Search))
            {
                var term = request.Search.Trim();

                query = query.Where(s =>
                    EF.Functions.Like(
                        s.Employee.ApplicationUser.FullName,
                        $"%{term}%")
                    ||
                    EF.Functions.Like(
                        s.Employee.ApplicationUser.Email!,
                        $"%{term}%"));
            }

            var totalCount = await query.CountAsync(cancellationToken);

            // Pagination
            int pageSize = request.PageSize.HasValue
                ? Math.Min(request.PageSize.Value, MaxPageSize)
                : DefaultPageSize;

            int pageNumber = request.PageNumber <= 0
                ? 1
                : request.PageNumber;

            var items = await query
            .OrderBy(s => s.Employee.ApplicationUser.FullName)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .Select(o => new EmployeeOvertimeDTO(
                        o.Id,
                        o.Date,
                        o.StartTime,
                        o.EndTime,
                        o.RequestedHours,
                        o.Reason,
                        o.Source,
                        o.EmployeeResponse,
                        o.Status,
                        o.EmployeeRemarks,
                        o.AdminRemarks,
                        o.CreatedAt,
                        o.RespondedAt
                    ))
                    .ToListAsync(cancellationToken);

            return new PagedResult<EmployeeOvertimeDTO>(
                items,
                totalCount,
                pageNumber,
                pageSize
            );
        }
    }
}
