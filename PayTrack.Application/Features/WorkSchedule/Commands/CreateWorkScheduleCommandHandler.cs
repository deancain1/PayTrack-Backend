using MediatR;
using PayTrack.Application.DTOs;
using PayTrack.Application.Features.WorkSchedule.Commands;
using PayTrack.Application.Interfaces;
using PayTrack.Domain.Entities;
using PayTrack.Domain.Enums;

public class CreateWorkScheduleCommandHandler
    : IRequestHandler<CreateWorkScheduleCommand, WorkSchedulesDTO>
{
    private readonly IWorkScheduleRepository _repository;
    private readonly ICurrentUserService _currentUser;

    public CreateWorkScheduleCommandHandler(
        IWorkScheduleRepository repository,
        ICurrentUserService currentUser)
    {
        _repository = repository;
        _currentUser = currentUser;
    }

    public async Task<WorkSchedulesDTO> Handle(
        CreateWorkScheduleCommand request,
        CancellationToken cancellationToken)
    {
        if (string.IsNullOrEmpty(_currentUser.UserId))
        {
            throw new UnauthorizedAccessException(
                "User not authenticated");
        }

        var schedule = new WorkSchedules
        {
            Id = Guid.NewGuid(),
            ScheduleName = request.ScheduleName,

            ScheduleType =
                (ScheduleType)request.ScheduleType,

            StartTime = request.StartTime,
            EndTime = request.EndTime,

            RequiredHours =
                request.RequiredHours,

            GracePeriodMinutes =
                request.GracePeriodMinutes,

            CompanyAdminId =
                _currentUser.UserId
        };

         await _repository.CreateAsync(schedule);

        return new WorkSchedulesDTO(
                schedule.Id,
                schedule.ScheduleName,
                (int)schedule.ScheduleType,
                schedule.StartTime,
                schedule.EndTime,
                schedule.RequiredHours,
                schedule.GracePeriodMinutes
            );
    }
}