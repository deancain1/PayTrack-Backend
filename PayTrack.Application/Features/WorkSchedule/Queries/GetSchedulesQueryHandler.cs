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

namespace PayTrack.Application.Features.WorkSchedule.Queries
{
    public class GetSchedulesQueryHandler
        : IRequestHandler<GetSchedulesQuery, PagedResult<WorkSchedulesDTO>>
    {
        private readonly IWorkScheduleRepository _repository;
        private readonly ICurrentUserService _currentUser;

        private const int DefaultPageSize = 5;
        private const int MaxPageSize = 20;

        public GetSchedulesQueryHandler(
            IWorkScheduleRepository repository,
            ICurrentUserService currentUser)
        {
            _repository = repository;
            _currentUser = currentUser;
        }

        public async Task<PagedResult<WorkSchedulesDTO>> Handle(
            GetSchedulesQuery request,
            CancellationToken cancellationToken)
        {
            var query =
                _repository.GetSchedulesByCompanyAdminQuery(
                    _currentUser.UserId);

            // Search
            if (!string.IsNullOrWhiteSpace(request.Search))
            {
                var term = request.Search.Trim();

                query = query.Where(x =>
                    EF.Functions.Like(
                        x.ScheduleName,
                        $"%{term}%"));
            }

            var totalCount =
                await query.CountAsync(cancellationToken);

            int pageSize = request.PageSize.HasValue
                ? Math.Min(request.PageSize.Value, MaxPageSize)
                : DefaultPageSize;

            int pageNumber = request.PageNumber <= 0
                ? 1
                : request.PageNumber;

            var items = await query
                .OrderBy(x => x.ScheduleName)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .Select(x => new WorkSchedulesDTO(
                    x.Id,
                    x.ScheduleName,
                    (int)x.ScheduleType,
                    x.StartTime,
                    x.EndTime,
                    x.RequiredHours,
                    x.GracePeriodMinutes,
                    x.AbsentCutoffTime,
                    x.WorkingDays
                ))
                .ToListAsync(cancellationToken);

            return new PagedResult<WorkSchedulesDTO>(
                items,
                totalCount,
                pageNumber,
                pageSize
            );
        }
    }
}
