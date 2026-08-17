using MediatR;
using PayTrack.Application.DTOs;
using PayTrack.Application.Interfaces.Repositories;
using PayTrack.Application.Interfaces.Services;
using PayTrack.Domain.Entities;
using PayTrack.Domain.Enums;

namespace PayTrack.Application.Features.LeaveRequests.Commands
{
    public class CreateLeaveRequestCommandHandler
        : IRequestHandler<CreateLeaveRequestCommand, LeaveRequestDTO>
    {
        private readonly ILeaveRequestRepository _leaveRequestRepository;
        private readonly IEmployeesRepository _employeesRepository;
        private readonly ICurrentUserService _currentUserService;

        public CreateLeaveRequestCommandHandler(
            ILeaveRequestRepository leaveRequestRepository,
            IEmployeesRepository employeesRepository,
            ICurrentUserService currentUserService)
        {
            _leaveRequestRepository = leaveRequestRepository;
            _employeesRepository = employeesRepository;
            _currentUserService = currentUserService;
        }
        public async Task<LeaveRequestDTO> Handle(CreateLeaveRequestCommand request, CancellationToken cancellationToken)
        {
            if (request.StartDate.Date > request.EndDate.Date)
            {
                throw new ArgumentException(
                    "Start date cannot be later than end date.");
            }

            if (string.IsNullOrWhiteSpace(request.Reason))
            {
                throw new ArgumentException(
                    "Leave reason is required.");
            }

            if (!Enum.TryParse<LeaveType>(
                    request.LeaveType,
                    true,
                    out var leaveType))
            {
                throw new ArgumentException(
                    "Invalid leave type.");
            }

            var userId = _currentUserService.UserId;

            if (string.IsNullOrEmpty(userId))
            {
                throw new UnauthorizedAccessException(
                    "User is not authenticated.");
            }

            var employee =
                await _employeesRepository
                    .GetCurrentEmployeeByApplicationUserIdAsync(
                        userId);

            if (employee == null)
            {
                throw new KeyNotFoundException(
                    "Employee record not found.");
            }

            var startDate = request.StartDate.Date;
            var endDate = request.EndDate.Date;

            var hasOverlap =
                await _leaveRequestRepository
                    .HasOverlappingLeaveAsync(
                        employee.Id,
                        startDate,
                        endDate);

            if (hasOverlap)
            {
                throw new InvalidOperationException(
                    "You already have a pending or approved leave within the selected dates.");
            }

            var totalDays =
                (endDate - startDate).Days + 1;

            // SAVE ATTACHMENT

            string? attachmentUrl = null;

            if (request.Attachment != null &&
                request.Attachment.Length > 0)
            {
                var uploadsFolder = Path.Combine(
                    Directory.GetCurrentDirectory(),
                    "wwwroot",
                    "uploads",
                    "leaves"
                );

                Directory.CreateDirectory(uploadsFolder);

                var extension =
                    Path.GetExtension(
                        request.Attachment.FileName);

                var fileName =
                    $"{Guid.NewGuid()}{extension}";

                var filePath =
                    Path.Combine(
                        uploadsFolder,
                        fileName);

                using var stream =
                    new FileStream(
                        filePath,
                        FileMode.Create);

                await request.Attachment.CopyToAsync(
                    stream,
                    cancellationToken);

                attachmentUrl =
                    $"/uploads/leaves/{fileName}";
            }

            var leaveRequest = new LeaveRequest
            {
                Id = Guid.NewGuid(),

                EmployeeId = employee.Id,

                LeaveType = leaveType,

                StartDate = startDate,

                EndDate = endDate,

                TotalDays = totalDays,

                Reason = request.Reason.Trim(),

                AttachmentUrl = attachmentUrl,

                Status = LeaveStatus.Pending,

                CreatedAt = DateTime.Now
            };

            await _leaveRequestRepository.AddAsync(
                leaveRequest);

            var employeeName =
                $"{employee.ApplicationUser.FullName}";

            return new LeaveRequestDTO(
                leaveRequest.Id,
                leaveRequest.EmployeeId,

                employeeName,

                leaveRequest.Employee.EmployeeNumber,

                leaveRequest.Employee.Department != null
                    ? leaveRequest.Employee.Department.DepartmentName
                    : null,

                leaveRequest.LeaveType.ToString(),
                leaveRequest.StartDate,
                leaveRequest.EndDate,
                leaveRequest.TotalDays,
                leaveRequest.Reason,
                leaveRequest.AttachmentUrl,
                leaveRequest.Status.ToString(),
                leaveRequest.AdminRemarks,
                leaveRequest.ReviewedBy,
                leaveRequest.ReviewedAt,
                leaveRequest.CreatedAt
            );
        }
    }
}