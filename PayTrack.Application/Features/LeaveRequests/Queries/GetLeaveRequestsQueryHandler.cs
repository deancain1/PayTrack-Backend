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
    public class GetLeaveRequestsQueryHandler : IRequestHandler<GetLeaveRequestsQuery, PagedResult<LeaveRequestDTO>>
    {
        private readonly ILeaveRequestRepository _leaveRequestRepository;
        private readonly ICurrentUserService _currentUserService;
        private const int DefaultPageSize = 7;
        private const int MaxPageSize = 20;
        
        public GetLeaveRequestsQueryHandler(ILeaveRequestRepository leaveRequestRepository, ICurrentUserService currentUserService)
        {
            _leaveRequestRepository = leaveRequestRepository;
            _currentUserService = currentUserService;
        }
        public async Task<PagedResult<LeaveRequestDTO>> Handle(GetLeaveRequestsQuery request, CancellationToken cancellationToken)
        {
           var companyAdmindId = _currentUserService.UserId;
            if (string.IsNullOrWhiteSpace(companyAdmindId))
            {
                throw new UnauthorizedAccessException(
                    "User is not authenticated.");
            }

            // Get only this admin's company leaves
            var query =
                _leaveRequestRepository
                    .GetLeaveRequestsByCompanyAdmin(
                        companyAdmindId);

            // Search

            if (!string.IsNullOrWhiteSpace(request.Search))
            {
                var term =
                    request.Search.Trim();

                query = query.Where(l =>
                    EF.Functions.Like(
                        l.Employee.ApplicationUser.FullName,
                        $"%{term}%")

                    ||

                    EF.Functions.Like(
                        l.Employee.EmployeeNumber,
                        $"%{term}%")

                    ||

                    EF.Functions.Like(
                        l.Status.ToString(),
                        $"%{term}%")

                    ||

                    EF.Functions.Like(
                        l.LeaveType.ToString(),
                        $"%{term}%")
                );
            }

            var totalCount =
                await query.CountAsync(
                    cancellationToken);

            var pageSize =
                request.PageSize.HasValue
                    ? Math.Min(
                        request.PageSize.Value,
                        MaxPageSize)
                    : DefaultPageSize;

            var pageNumber =
                request.PageNumber <= 0
                    ? 1
                    : request.PageNumber;


            var items =
                await query
                    .OrderByDescending(
                        l => l.CreatedAt)

                    .Skip(
                        (pageNumber - 1) *
                        pageSize)

                    .Take(pageSize)

                    .Select(l =>
                        new LeaveRequestDTO(
                            l.Id,
                            l.EmployeeId,
                            l.Employee.ApplicationUser.FullName,
                            l.Employee.EmployeeNumber,
                            l.Employee.Department != null
                                ? l.Employee.Department.DepartmentName
                                : null,
                            l.LeaveType.ToString(),
                            l.StartDate,
                            l.EndDate,
                            l.TotalDays,
                            l.Reason,
                            l.AttachmentUrl,
                            l.Status.ToString(),
                            l.AdminRemarks,
                            l.ReviewedBy,
                            l.ReviewedAt,
                            l.CreatedAt
                        ))
                    .ToListAsync(
                        cancellationToken);


            // =====================================================
            // Return
            // =====================================================

            return new PagedResult<LeaveRequestDTO>(
                items,
                totalCount,
                pageNumber,
                pageSize
            );
        }
    }
}
