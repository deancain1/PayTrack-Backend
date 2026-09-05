using MediatR;
using PayTrack.Application.DTOs;
using PayTrack.Application.Interfaces.Repositories;
using PayTrack.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace PayTrack.Application.Features.WorkSchedule.Commands
{
    public class UpdateWorkScheduleCommandHandler: IRequestHandler<UpdateWorkScheduleCommand, WorkSchedulesDTO>
    {
        private readonly IWorkScheduleRepository _workScheduleRepository;

        public UpdateWorkScheduleCommandHandler(
            IWorkScheduleRepository workScheduleRepository)
        {
            _workScheduleRepository = workScheduleRepository;
        }

        public async Task<WorkSchedulesDTO> Handle(UpdateWorkScheduleCommand request, CancellationToken cancellationToken)
        {
            var workSchedule =
                await _workScheduleRepository.GetByIdAsync(request.Id);

            if (workSchedule == null)
            {
                throw new KeyNotFoundException(
                    $"Work schedule with ID {request.Id} not found.");
            }

            if (string.IsNullOrWhiteSpace(request.ScheduleName))
            {
                throw new ArgumentException(
                    "Schedule name is required.");
            }

            if (request.RequiredHours <= 0)
            {
                throw new ArgumentException(
                    "Required hours must be greater than zero.");
            }

            if (request.GracePeriodMinutes < 0)
            {
                throw new ArgumentException(
                    "Grace period cannot be negative.");
            }

            if (!Enum.IsDefined(
                    typeof(ScheduleType),
                    request.ScheduleType))
            {
                throw new ArgumentException(
                    "Invalid schedule type.");
            }

            var scheduleType =
                (ScheduleType)request.ScheduleType;

            // Fixed schedules require both start and end times
            if (scheduleType == ScheduleType.Fixed)
            {
                if (!request.StartTime.HasValue)
                {
                    throw new ArgumentException(
                        "Fixed schedules require a start time.");
                }

                if (!request.EndTime.HasValue)
                {
                    throw new ArgumentException(
                        "Fixed schedules require an end time.");
                }

                if (request.EndTime.Value <= request.StartTime.Value)
                {
                    throw new ArgumentException(
                        "End time must be later than start time.");
                }
            }

            // Hybrid schedules require a start time
            if (scheduleType == ScheduleType.Hybrid)
            {
                if (!request.StartTime.HasValue)
                {
                    throw new ArgumentException(
                        "Hybrid schedules require a start time.");
                }

                if (request.EndTime.HasValue &&
                    request.EndTime.Value <= request.StartTime.Value)
                {
                    throw new ArgumentException(
                        "End time must be later than start time.");
                }
            }

            // Flexible schedules should not require fixed start/end times
            if (scheduleType == ScheduleType.Flexible)
            {
                if (request.StartTime.HasValue ||
                    request.EndTime.HasValue)
                {
                    throw new ArgumentException(
                        "Flexible schedules should not have a fixed start or end time.");
                }
            }

            // Validate allowed start-time range
            if (request.EarliestStartTime.HasValue &&
                request.LatestStartTime.HasValue)
            {
                if (request.LatestStartTime.Value <
                    request.EarliestStartTime.Value)
                {
                    throw new ArgumentException(
                        "Latest start time must be later than or equal to earliest start time.");
                }
            }

            // If both are configured, the start time must be inside the allowed range
            if (request.StartTime.HasValue &&
                request.EarliestStartTime.HasValue &&
                request.StartTime.Value < request.EarliestStartTime.Value)
            {
                throw new ArgumentException(
                    "Start time cannot be earlier than the earliest allowed start time.");
            }

            if (request.StartTime.HasValue &&
                request.LatestStartTime.HasValue &&
                request.StartTime.Value > request.LatestStartTime.Value)
            {
                throw new ArgumentException(
                    "Start time cannot be later than the latest allowed start time.");
            }

            // Update entity
            workSchedule.ScheduleName =
                request.ScheduleName.Trim();

            workSchedule.ScheduleType =
                scheduleType;

            workSchedule.StartTime =
                request.StartTime;

            workSchedule.EndTime =
                request.EndTime;

            workSchedule.EarliestStartTime =
                request.EarliestStartTime;

            workSchedule.LatestStartTime =
                request.LatestStartTime;

            workSchedule.RequiredHours =
                request.RequiredHours;

            workSchedule.GracePeriodMinutes =
                request.GracePeriodMinutes;

            workSchedule.AbsentCutoffTime =
                request.AbsentCutoffTime;

            workSchedule.WorkingDays =
                request.WorkingDays;

            _workScheduleRepository.Update(workSchedule);

            await _workScheduleRepository.SaveChangesAsync();

            return new WorkSchedulesDTO(
                workSchedule.Id,
                workSchedule.ScheduleName,
                (int)workSchedule.ScheduleType,
                workSchedule.StartTime,
                workSchedule.EndTime,
                workSchedule.EarliestStartTime,
                workSchedule.LatestStartTime,
                workSchedule.RequiredHours,
                workSchedule.GracePeriodMinutes,
                workSchedule.AbsentCutoffTime,
                workSchedule.WorkingDays
            );
        }
    }
}