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
            // Validate current user
            if (string.IsNullOrWhiteSpace(_currentUser.UserId))
            {
                throw new UnauthorizedAccessException(
                    "User not authenticated.");
            }

            // Validate schedule name
            if (string.IsNullOrWhiteSpace(request.ScheduleName))
            {
                throw new ArgumentException(
                    "Schedule name is required.");
            }

            // Validate required hours
            if (request.RequiredHours <= 0)
            {
                throw new ArgumentException(
                    "Required hours must be greater than zero.");
            }

            // Validate grace period
            if (request.GracePeriodMinutes < 0)
            {
                throw new ArgumentException(
                    "Grace period cannot be negative.");
            }

            // Validate schedule type
            if (!Enum.IsDefined(
                typeof(ScheduleType),
                request.ScheduleType))
            {
                throw new ArgumentException(
                    "Invalid schedule type.");
            }

            var scheduleType =
                (ScheduleType)request.ScheduleType;

            // Validate Fixed schedule
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

                if (request.EndTime.Value <= request.StartTime.Value)
                {
                    throw new ArgumentException(
                        "End time must be later than start time.");
                }
            }

            // Validate Hybrid schedule
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

                if (request.EndTime.Value <= request.StartTime.Value)
                {
                    throw new ArgumentException(
                        "End time must be later than start time.");
                }
            }

            // Validate Flexible schedule
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

            // Validate earliest and latest start time
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

            // Validate start time against allowed range
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

            // Validate absent cutoff time
            if (
                request.AbsentCutoffTime.HasValue &&
                request.StartTime.HasValue
            )
            {
                if (
                    request.AbsentCutoffTime.Value <
                    request.StartTime.Value
                )
                {
                    throw new ArgumentException(
                        "Absent cutoff time cannot be earlier than start time.");
                }
            }

            // Create work schedule
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

            // Save schedule
            await _workScheduleRepository.AddAsync(
                schedule);

            // Return DTO
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