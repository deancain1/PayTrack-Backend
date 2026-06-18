using MediatR;
using Microsoft.EntityFrameworkCore;
using PayTrack.Application.DTOs;
using PayTrack.Application.Interfaces;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace PayTrack.Application.Features.Salary.Queries
{
    public class GetEmployeeSalaryQueryHandler : IRequestHandler<GetEmployeeSalaryQuery, PagedResult<SalaryDTO>>
    {
        private readonly ISalaryRepository _salaryRepository;

        private const int DefaultPageSize = 7;
        private const int MaxPageSize = 20;

        public GetEmployeeSalaryQueryHandler(ISalaryRepository salaryRepository)
        {
            _salaryRepository = salaryRepository;
        }

        public async Task<PagedResult<SalaryDTO>> Handle(
            GetEmployeeSalaryQuery request,
            CancellationToken cancellationToken)
        {
            var query = _salaryRepository.GetEmployeeSalaryQuery();

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
            .Select(s => new SalaryDTO(
             s.EmployeeId,
             s.Employee.ApplicationUser.FullName,
             s.Employee.EmployeeNumber,
             s.Employee.Department,
             s.Rate,
             s.RequiredHours,
             s.Employee.ApplicationUser.ProfilePicture != null
             ? $"data:image/png;base64,{Convert.ToBase64String(s.Employee.ApplicationUser.ProfilePicture)}"
             : null,
             s.Policy
             ))
            .ToListAsync(cancellationToken);

            return new PagedResult<SalaryDTO>(
                items,
                totalCount,
                pageNumber,
                pageSize
            );
        }
    }
}