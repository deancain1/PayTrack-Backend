using MediatR;
using PayTrack.Application.DTOs;
using PayTrack.Application.Interfaces.Repositories;
using PayTrack.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PayTrack.Application.Features.WorkSchedule.Commands
{
    internal class UpdateWorkScheduleCommandHandler : IRequestHandler<UpdateWorkScheduleCommand, WorkSchedulesDTO>
    {
        private readonly IWorkScheduleRepository _workScheduleRepository;

        public UpdateWorkScheduleCommandHandler(IWorkScheduleRepository workScheduleRepository)
        {
            _workScheduleRepository = workScheduleRepository;
        }
        public async Task<WorkSchedulesDTO> Handle(UpdateWorkScheduleCommand request, CancellationToken cancellationToken)
        {
           var workSchedule = await _workScheduleRepository.GetByIdAsync(request.Id);
            if (workSchedule == null)
            
               throw new KeyNotFoundException($"Work schedule with ID {request.Id} not found.");


            workSchedule.ScheduleName = request.ScheduleName;
            workSchedule.ScheduleType = (ScheduleType)request.ScheduleType;
            workSchedule.StartTime = request.StartTime;
            workSchedule.EndTime = request.EndTime;
            workSchedule.RequiredHours = request.RequiredHours;
            workSchedule.GracePeriodMinutes = request.GracePeriodMinutes;
            workSchedule.AbsentCutoffTime = request.AbsentCutoffTime;
             _workScheduleRepository.Update(workSchedule);
            await _workScheduleRepository.SaveChangesAsync();
            return new WorkSchedulesDTO(
                workSchedule.Id,
                workSchedule.ScheduleName,
               (int)workSchedule.ScheduleType,
                workSchedule.StartTime,
                workSchedule.EndTime,
                workSchedule.RequiredHours,
                workSchedule.GracePeriodMinutes,
                workSchedule.AbsentCutoffTime
            );
        }
    }
}
