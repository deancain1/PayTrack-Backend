using MediatR;
using PayTrack.Application.Interfaces;
using PayTrack.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PayTrack.Application.Features.WorkSchedule.Commands
{
    public class DeleteWorkSchedulesCommandHandler : IRequestHandler<DeleteWorkSchedulesCommand, Unit>
    {
        private readonly IWorkScheduleRepository _workScheduleRepository;
        private readonly ICurrentUserService _currentUserService;
        public DeleteWorkSchedulesCommandHandler(IWorkScheduleRepository workScheduleRepository, ICurrentUserService currentUserService)
        {
            _workScheduleRepository = workScheduleRepository;
            _currentUserService = currentUserService;
        }
        public async Task<Unit> Handle(DeleteWorkSchedulesCommand request, CancellationToken cancellationToken)
        {
           var schedules = await _workScheduleRepository.GetByIdsAsync(request.WorkScheduleIds);

            if (!schedules.Any())
                throw new Exception("No work schedules found.");

            var currentUserId = _currentUserService.UserId;
            if (!schedules.Any())
                throw new Exception("No work schedules found.");

            await _workScheduleRepository.DeleteRangeAsync(schedules);
            return Unit.Value; 

        }
    }
}
