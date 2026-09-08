using MediatR;
using PayTrack.Application.DTOs;
using PayTrack.Application.Interfaces.Repositories;
using PayTrack.Application.Interfaces.Services;
using PayTrack.Domain.Entities;
using PayTrack.Domain.Enums;

namespace PayTrack.Application.Features.WorkSchedule.Commands
{
    public class CreateWorkScheduleCommandHandler
        : IRequestHandler<CreateWorkScheduleCommand, WorkSchedulesDTO>
    {
        private readonly IWorkScheduleRepository _workScheduleRepository;
        private readonly ICurrentUserService _currentUser;

        public CreateWorkScheduleCommandHandler(
            IWorkScheduleRepository workScheduleRepository,
            ICurrentUserService currentUser)
        {
            _workScheduleRepository = workScheduleRepository;
            _currentUser = currentUser;
        }

        public async Task<WorkSchedulesDTO> Handle(
            CreateWorkScheduleCommand request,
            CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(_currentUser.UserId))
            {
                throw new UnauthorizedAccessException(
                    "User not authenticated.");
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

            if (scheduleType == ScheduleType.Fixed)
            {
                if (!request.StartTime.HasValue)
                {
                    throw new ArgumentException(
                        "Start time is required for a fixed schedule.");
                }

                if (!request.EndTime.HasValue)
                {
                    throw new ArgumentException(
                        "End time is required for a fixed schedule.");
                }

                if (request.EndTime.Value ==
                    request.StartTime.Value)
                {
                    throw new ArgumentException(
                        "End time cannot be the same as start time.");
                }
            }

            if (scheduleType == ScheduleType.Hybrid)
            {
                if (!request.StartTime.HasValue)
                {
                    throw new ArgumentException(
                        "Start time is required for a hybrid schedule.");
                }

                if (!request.EndTime.HasValue)
                {
                    throw new ArgumentException(
                        "End time is required for a hybrid schedule.");
                }

                if (request.EndTime.Value ==
                    request.StartTime.Value)
                {
                    throw new ArgumentException(
                        "End time cannot be the same as start time.");
                }
            }

            if (scheduleType == ScheduleType.Flexible)
            {
                if (!request.EarliestStartTime.HasValue)
                {
                    throw new ArgumentException(
                        "Earliest start time is required for a flexible schedule.");
                }

                if (!request.LatestStartTime.HasValue)
                {
                    throw new ArgumentException(
                        "Latest start time is required for a flexible schedule.");
                }

                if (request.LatestStartTime.Value <
                    request.EarliestStartTime.Value)
                {
                    throw new ArgumentException(
                        "Latest start time must be later than or equal to earliest start time.");
                }
            }

            if (
                request.EarliestStartTime.HasValue &&
                request.LatestStartTime.HasValue
            )
            {
                if (
                    request.LatestStartTime.Value <
                    request.EarliestStartTime.Value
                )
                {
                    throw new ArgumentException(
                        "Latest start time must be later than or equal to earliest start time.");
                }
            }

            if (
                request.StartTime.HasValue &&
                request.EarliestStartTime.HasValue
            )
            {
                if (
                    request.StartTime.Value <
                    request.EarliestStartTime.Value
                )
                {
                    throw new ArgumentException(
                        "Start time cannot be earlier than the earliest allowed start time.");
                }
            }

            if (
                request.StartTime.HasValue &&
                request.LatestStartTime.HasValue
            )
            {
                if (
                    request.StartTime.Value >
                    request.LatestStartTime.Value
                )
                {
                    throw new ArgumentException(
                        "Start time cannot be later than the latest allowed start time.");
                }
            }

            if (
                request.AbsentCutoffTime.HasValue &&
                request.StartTime.HasValue &&
                request.EndTime.HasValue
            )
            {
                var startTime =
                    request.StartTime.Value;

                var endTime =
                    request.EndTime.Value;

                var cutoffTime =
                    request.AbsentCutoffTime.Value;

                if (endTime > startTime)
                {
                    if (
                        cutoffTime < startTime ||
                        cutoffTime > endTime
                    )
                    {
                        throw new ArgumentException(
                            "Absent cutoff time must be between start time and end time.");
                    }
                }
                else
                {
                    if (
                        cutoffTime > startTime &&
                        cutoffTime < endTime
                    )
                    {
                        throw new ArgumentException(
                            "Absent cutoff time must fall within the overnight schedule.");
                    }
                }
            }

            var schedule = new WorkSchedules
            {
                Id = Guid.NewGuid(),

                ScheduleName =
                    request.ScheduleName.Trim(),

                ScheduleType =
                    scheduleType,

                StartTime =
                    request.StartTime,

                EndTime =
                    request.EndTime,

                EarliestStartTime =
                    request.EarliestStartTime,

                LatestStartTime =
                    request.LatestStartTime,

                RequiredHours =
                    request.RequiredHours,

                GracePeriodMinutes =
                    request.GracePeriodMinutes,

                AbsentCutoffTime =
                    request.AbsentCutoffTime,

                WorkingDays =
                    request.WorkingDays,

                CompanyAdminId =
                    _currentUser.UserId
            };

            await _workScheduleRepository.AddAsync(
                schedule);

            return new WorkSchedulesDTO(
                schedule.Id,
                schedule.ScheduleName,
                (int)schedule.ScheduleType,
                schedule.StartTime,
                schedule.EndTime,
                schedule.EarliestStartTime,
                schedule.LatestStartTime,
                schedule.RequiredHours,
                schedule.GracePeriodMinutes,
                schedule.AbsentCutoffTime,
                schedule.WorkingDays
            );
        }
    }
}