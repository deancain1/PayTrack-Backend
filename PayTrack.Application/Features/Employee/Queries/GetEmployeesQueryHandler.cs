using MediatR;
using Microsoft.EntityFrameworkCore;
using PayTrack.Application.DTOs;
using PayTrack.Application.Interfaces;
using PayTrack.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PayTrack.Application.Features.Employee.Queries
{
    public class GetEmployeesQueryHandler
      : IRequestHandler<GetEmployeesQuery, PagedResult<EmployeeDTO>>
    {
        private readonly IEmployeesRepository _employeeRepository;

        private const int DefaultPageSize = 7;
        private const int MaxPageSize = 20;

        public GetEmployeesQueryHandler(
            IEmployeesRepository employeeRepository)
        {
            _employeeRepository = employeeRepository;
        }

        public async Task<PagedResult<EmployeeDTO>> Handle(
            GetEmployeesQuery request,
            CancellationToken cancellationToken)
        {
            var query = _employeeRepository.GetEmployeesQuery();

            // Search
            if (!string.IsNullOrWhiteSpace(request.Search))
            {
                var term = request.Search.Trim();

                query = query.Where(e =>
                    EF.Functions.Like(e.ApplicationUser.FullName, $"%{term}%") ||
                    EF.Functions.Like(e.EmployeeNumber, $"%{term}%") ||
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
                .Select(e => new EmployeeDTO(
                    e.Id,
                    e.ApplicationUserId,
                    e.ApplicationUser.FullName,
                    e.ApplicationUser.Email,
                    e.EmployeeNumber,
                    e.Department,
                    e.ApplicationUser.PhoneNumber,
                    e.ApplicationUser.ProfilePicture != null
                    ? $"data:image/png;base64,{Convert.ToBase64String(e.ApplicationUser.ProfilePicture)}": null
                ))
                .ToListAsync(cancellationToken);

            return new PagedResult<EmployeeDTO>(
                items,
                totalCount,
                pageNumber,
                pageSize
            );
        }
    }
}
