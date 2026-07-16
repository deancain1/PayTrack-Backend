using MediatR;
using Microsoft.EntityFrameworkCore;
using PayTrack.Application.DTOs;
using PayTrack.Application.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PayTrack.Application.Features.SalaryAdvance.Queries
{
    public class GetCurrentEmployeeSalaryAdvancesQueryHandler : IRequestHandler<GetCurrentEmployeeSalaryAdvancesQuery, PagedResult<SalaryAdvancesDTO>>
    {
        private readonly ISalaryAdvanceRepository _salaryAdvanceRepository;
        private readonly ICurrentUserService _currentUserService;
        private const int DefaultPageSize = 7;
        private const int MaxPageSize = 20;

        public GetCurrentEmployeeSalaryAdvancesQueryHandler(
            ISalaryAdvanceRepository salaryAdvanceRepository,
            ICurrentUserService currentUserService)
        {
            _salaryAdvanceRepository = salaryAdvanceRepository;
            _currentUserService = currentUserService;
        }
        public async  Task<PagedResult<SalaryAdvancesDTO>> Handle(GetCurrentEmployeeSalaryAdvancesQuery request, CancellationToken cancellationToken)
        {
            var query = _salaryAdvanceRepository.GetSalaryAdvancesbyEmployee(_currentUserService.UserId);

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
            .OrderBy(s => s.Employee.ApplicationUser.FullName)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .Select(s => new SalaryAdvancesDTO(
             s.Id,
             s.Employee.ApplicationUser.FullName,
             s.Amount,
             s.Remarks,
             s.RequestDate
             ))
            .ToListAsync(cancellationToken);

            return new PagedResult<SalaryAdvancesDTO>(
                items,
                totalCount,
                pageNumber,
                pageSize
            );
        }
    }
}
