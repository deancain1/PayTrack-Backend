using MediatR;
using Microsoft.EntityFrameworkCore;
using PayTrack.Application.DTOs;
using PayTrack.Application.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PayTrack.Application.Features.PayrollRecords.Queries
{
    public class GetPayrollRecordsQueryHandler : IRequestHandler<GetPayrollRecordsQuery, PagedResult<PayrollRecordDTO>>
    {
        private readonly IPayrollRecordRepository _payrollRecordRepository;
        private readonly ICurrentUserService _currentUserService;
        private const int DefaultPageSize = 7;
        private const int MaxPageSize = 20;
        public GetPayrollRecordsQueryHandler(IPayrollRecordRepository payrollRecordRepository, ICurrentUserService currentUserService)
        {
            _payrollRecordRepository = payrollRecordRepository;
            _currentUserService = currentUserService;
        }
        public async Task<PagedResult<PayrollRecordDTO>> Handle(GetPayrollRecordsQuery request, CancellationToken cancellationToken)
        {
            var query = _payrollRecordRepository.GetPayrollRecordQueryByEmployeeAsync(_currentUserService.UserId);

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
            .Select(p => new PayrollRecordDTO(
             p.Id,
             p.EmployeeId,
             p.Attendance.TimeIn,
             p.Attendance.TimeOut,
             p.Employee.ApplicationUser.FullName,
             p.AmountEarned
             ))
            .ToListAsync(cancellationToken);

            return new PagedResult<PayrollRecordDTO>(
                items,
                totalCount,
                pageNumber,
                pageSize
            );
        }
    }
}
