using MediatR;
using Microsoft.EntityFrameworkCore;
using PayTrack.Application.DTOs;
using PayTrack.Application.Interfaces;

namespace PayTrack.Application.Features.SalaryAdvance.Queries
{
    public class GetPendingSalaryAdvanceRequestsQueryHandler
        : IRequestHandler<GetPendingSalaryAdvanceRequestsQuery, PagedResult<SalaryAdvanceRequestDTO>>
    {
        private readonly ISalaryAdvanceRepository _salaryAdvanceRepository;
        private readonly ICurrentUserService _currentUserService;

        private const int DefaultPageSize = 7;
        private const int MaxPageSize = 20;

        public GetPendingSalaryAdvanceRequestsQueryHandler(
            ISalaryAdvanceRepository salaryAdvanceRepository,
            ICurrentUserService currentUserService)
        {
            _salaryAdvanceRepository = salaryAdvanceRepository;
            _currentUserService = currentUserService;
        }

        public async Task<PagedResult<SalaryAdvanceRequestDTO>> Handle(
            GetPendingSalaryAdvanceRequestsQuery request,
            CancellationToken cancellationToken)
        {
            var query = _salaryAdvanceRepository
                .GetPendingSalaryAdvanceRequestsQueryByCompanyAdmin(_currentUserService.UserId);

            // Search
            if (!string.IsNullOrWhiteSpace(request.Search))
            {
                var term = request.Search.Trim();

                query = query.Where(x =>
                    EF.Functions.Like(
                        x.Employee.ApplicationUser.FullName,
                        $"%{term}%")
                    ||
                    EF.Functions.Like(
                        x.Employee.EmployeeNumber,
                        $"%{term}%")
                    ||
                    EF.Functions.Like(
                        x.Remarks,
                        $"%{term}%"));
            }

            // Total Count
            var totalCount = await query.CountAsync(cancellationToken);

            // Pagination
            int pageSize = request.PageSize.HasValue
                ? Math.Min(request.PageSize.Value, MaxPageSize)
                : DefaultPageSize;

            int pageNumber = request.PageNumber <= 0
                ? 1
                : request.PageNumber;

            var items = await query
                .OrderByDescending(x => x.RequestDate)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .Select(x => new SalaryAdvanceRequestDTO(
                    x.Id,
                    x.EmployeeId,
                    x.Employee.EmployeeNumber,
                    x.Employee.ApplicationUser.FullName,
                    x.Amount,
                    x.Remarks,
                    x.RequestDate
                ))
                .ToListAsync(cancellationToken);

            return new PagedResult<SalaryAdvanceRequestDTO>(
                items,
                totalCount,
                pageNumber,
                pageSize
            );
        }
    }
}