using MediatR;
using PayTrack.Application.Features.Notification.Events;
using PayTrack.Application.Interfaces.Repositories;
using PayTrack.Application.Interfaces.Services;
using PayTrack.Domain.Entities;
using PayTrack.Domain.Enums;

namespace PayTrack.Application.Features.SalaryAdvance.Commands
{
    public class CreateSalaryAdvanceRequestCommandHandler
        : IRequestHandler<CreateSalaryAdvanceRequestCommand, Guid>
    {
        private readonly IEmployeesRepository _employeesRepository;
        private readonly ISalaryAdvanceRepository _salaryAdvanceRepository;
        private readonly ICurrentUserService _currentUserService;
        private readonly IMediator _mediator;
        private readonly ISalaryAdvanceEligibilityService
            _salaryAdvanceEligibilityService;

        public CreateSalaryAdvanceRequestCommandHandler(
            IEmployeesRepository employeesRepository,
            ISalaryAdvanceRepository salaryAdvanceRepository,
            ICurrentUserService currentUserService,
            IMediator mediator,
            ISalaryAdvanceEligibilityService salaryAdvanceEligibilityService)
        {
            _employeesRepository = employeesRepository;
            _salaryAdvanceRepository = salaryAdvanceRepository;
            _currentUserService = currentUserService;
            _mediator = mediator;
            _salaryAdvanceEligibilityService =
                salaryAdvanceEligibilityService;
        }

        public async Task<Guid> Handle(CreateSalaryAdvanceRequestCommand request, CancellationToken cancellationToken)
        {
            // 1. GET CURRENT EMPLOYEE

            var applicationUserId =
                _currentUserService.UserId;

            var employee =
                await _employeesRepository
                    .GetCurrentEmployeeByApplicationUserIdAsync(applicationUserId);

            if (employee == null)
            {
                throw new Exception("Employee not found.");
            }

            // 2. CHECK PENDING SALARY ADVANCE

            var hasPending =
                await _salaryAdvanceRepository
                    .HasPendingRequestAsync(employee.Id);

            if (hasPending)
            {
                throw new Exception(
                    "You already have a pending salary advance request.");
            }


            // 3. CHECK SALARY ADVANCE ELIGIBILITY

            var eligibility =
                await _salaryAdvanceEligibilityService
                    .GetEligibilityAsync(employee.Id);


            // 4. CHECK IF EMPLOYEE CAN REQUEST

            if (!eligibility.CanRequest)
            {
                throw new Exception(
                    "You do not have enough available earnings " +
                    "for a salary advance after statutory deductions.");
            }


            // 5. CHECK REQUESTED AMOUNT

            if (request.Amount <= 0)
            {
                throw new Exception(
                    "Salary advance amount must be greater than zero.");
            }

            if (request.Amount >
                eligibility.AvailableSalaryAdvance)
            {
                throw new Exception(
                    $"Maximum available salary advance is " +
                    $"₱{eligibility.AvailableSalaryAdvance:N2}");
            }


            // 6. CREATE SALARY ADVANCE

            var salaryAdvance = new SalaryAdvances
            {
                Id = Guid.NewGuid(),

                EmployeeId =
                    employee.Id,

                ApplicationUserId =
                    applicationUserId,

                Amount =
                    request.Amount,

                Remarks =
                    request.Remarks,

                RequestDate =
                    DateTime.UtcNow,

                Status =
                    SalaryAdvanceStatus.Pending
            };


            await _salaryAdvanceRepository
                .AddAsync(salaryAdvance);

            // 7. SEND NOTIFICATION

            await _mediator.Publish(
                new CompanyAdminNotificationCreatedEvent(
                    Guid.NewGuid(),

                    employee.CompanyAdminId,

                    CompanyAdminNotificationType.SalaryAdvance,

                    $"{employee.ApplicationUser.FullName} " +
                    $"requested a salary advance.",

                    DateTime.UtcNow
                ),
                cancellationToken);

            return salaryAdvance.Id;
        }
    }
}