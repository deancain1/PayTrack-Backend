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

namespace PayTrack.Application.Features.LeaveRequests.Queries
{
    public class GetMyLeavesQueryHandler : IRequestHandler<GetMyLeavesQuery, PagedResult<LeaveRequestDTO>>
    {
        private readonly ILeaveRequestRepository _leaveRequestRepository;
        private readonly IEmployeesRepository _employeesRepository;
        private readonly ICurrentUserService _currentUserService;
        private const int DefaultPageSize = 7;
        private const int MaxPageSize = 20;
        public GetMyLeavesQueryHandler(ILeaveRequestRepository leaveRequestRepository, IEmployeesRepository employeesRepository, ICurrentUserService currentUserService)
        {
            _leaveRequestRepository = leaveRequestRepository;
            _employeesRepository = employeesRepository;
            _currentUserService = currentUserService;
        }
        public async Task<PagedResult<LeaveRequestDTO>> Handle(GetMyLeavesQuery request, CancellationToken cancellationToken)
        {
           var employee = await _employeesRepository.GetCurrentEmployeeByApplicationUserIdAsync(_currentUserService.UserId);
            if (employee == null)
            {
                throw new InvalidOperationException("Employee record not found");
            }

            var query = _leaveRequestRepository.GetLeavesByEmployeeId(employee.Id);

            if (!string.IsNullOrWhiteSpace(
               request.Search))
            {
                var term =
                    request.Search.Trim();

                query = query.Where(x =>
                    EF.Functions.Like(
                        x.LeaveType.ToString(),
                        $"%{term}%"
                    )
                    ||
                    EF.Functions.Like(
                        x.Reason,
                        $"%{term}%"
                    )
                    ||
                    EF.Functions.Like(
                        x.Status.ToString(),
                        $"%{term}%"
                    )
                );
            }


            // Total Count

            var totalCount =
                await query.CountAsync(
                    cancellationToken);


            // Page Size

            int pageSize =
                request.PageSize.HasValue
                    ? Math.Min(
                        request.PageSize.Value,
                        MaxPageSize)
                    : DefaultPageSize;


            // Page Number

            int pageNumber =
                request.PageNumber <= 0
                    ? 1
                    : request.PageNumber;


            // Pagination + Projection

            var items =
                await query
                    .OrderByDescending(
                        x => x.CreatedAt)

                    .Skip(
                        (pageNumber - 1) *
                        pageSize)

                    .Take(pageSize)

                    .Select(x =>
                        new LeaveRequestDTO(
                            x.Id,
                            x.EmployeeId,
                            x.Employee.ApplicationUser.FullName,
                            x.Employee.EmployeeNumber,
                            x.Employee.Department != null?
                            x.Employee.Department.DepartmentName: null,
                            x.LeaveType.ToString(),
                            x.StartDate,
                            x.EndDate,
                            x.TotalDays,
                            x.Reason,
                            x.AttachmentUrl,
                            x.Status.ToString(),
                            x.AdminRemarks,
                            x.ReviewedBy,
                            x.ReviewedAt,
                            x.CreatedAt
                        )
                    )

                    .ToListAsync(
                        cancellationToken);


            // Return paginated result

            return new PagedResult<LeaveRequestDTO>(
                items,
                totalCount,
                pageNumber,
                pageSize
            );
        }
    }
}
