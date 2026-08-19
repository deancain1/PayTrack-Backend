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
    public class GetSchedulesLookupQueryHandler : IRequestHandler<GetSchedulesLookupQuery, List<WorkSchedulesDTO>>
    {
        private readonly IWorkScheduleRepository _workScheduleRepository;
        private readonly ICurrentUserService _currentUserService;
        public GetSchedulesLookupQueryHandler(IWorkScheduleRepository workScheduleRepository, ICurrentUserService currentUserService)
        {
            _workScheduleRepository = workScheduleRepository;
            _currentUserService = currentUserService;
        }
        public async Task<List<WorkSchedulesDTO>> Handle(GetSchedulesLookupQuery request, CancellationToken cancellationToken)
        {
            return await _workScheduleRepository
           .GetWorkSchedulesQuery(_currentUserService.UserId)
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
        }
    }
}
