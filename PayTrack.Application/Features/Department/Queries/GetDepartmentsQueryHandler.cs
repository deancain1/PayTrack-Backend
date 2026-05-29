using MediatR;
using Microsoft.EntityFrameworkCore;
using PayTrack.Application.DTOs;
using PayTrack.Application.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PayTrack.Application.Features.Department.Queries
{
    public class GetDepartmentsQueryHandler : IRequestHandler<GetDepartmentsQuery, PagedResult<DepartmentDTO>>
    {
        private readonly IDepartmentRepository _departmentRepository;
        private const int DefaultPageSize = 7;
        private const int MaxPageSize = 20;

        public GetDepartmentsQueryHandler(IDepartmentRepository departmentRepository)
        {
            _departmentRepository = departmentRepository;
        }

        public async Task<PagedResult<DepartmentDTO>> Handle(GetDepartmentsQuery request, CancellationToken cancellationToken)
        {
            var query = _departmentRepository.GetDepartmentsQuery();

            // 🔍 Search
            if (!string.IsNullOrWhiteSpace(request.Search))
            {
                var term = request.Search.Trim();
                query = query.Where(x =>
                    EF.Functions.Like(x.DepartmentName, $"%{term}%") ||
                    EF.Functions.Like(x.Description, $"%{term}%"));
            }

            // 🔹 Total count
            var totalCount = await query.CountAsync(cancellationToken);

            // 🔹 Determine page size
            int pageSize = request.PageSize.HasValue
                ? Math.Min(request.PageSize.Value, MaxPageSize) // enforce max
                : DefaultPageSize;                             // default if not provided

            int pageNumber = request.PageNumber <= 0 ? 1 : request.PageNumber;

            // 🔹 Pagination
            var items = await query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .Select(x => new DepartmentDTO(
                    x.Id,
                    x.ApplicationUserId,
                    x.DepartmentName,
                    x.Description

                ))
                .ToListAsync(cancellationToken);

            return new PagedResult<DepartmentDTO>(
                items,
                totalCount,
                pageNumber,
                pageSize
            );
        }
    }
}
