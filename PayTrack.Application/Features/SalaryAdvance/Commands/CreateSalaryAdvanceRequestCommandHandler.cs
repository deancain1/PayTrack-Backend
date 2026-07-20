using MediatR;
using PayTrack.Application.Interfaces;
using PayTrack.Domain.Entities;
using PayTrack.Domain.Enums;

namespace PayTrack.Application.Features.SalaryAdvance.Commands;

public class CreateSalaryAdvanceRequestCommandHandler
    : IRequestHandler<CreateSalaryAdvanceRequestCommand, Guid>
{
    private readonly IEmployeesRepository _employeesRepository;
    private readonly ISalaryAdvanceRepository _salaryAdvanceRepository;
    private readonly IPayrollRecordRepository _payrollRecordRepository;
    private readonly ICurrentUserService _currentUserService;

    public CreateSalaryAdvanceRequestCommandHandler(
        IEmployeesRepository employeesRepository,
        ISalaryAdvanceRepository salaryAdvanceRepository,
        IPayrollRecordRepository payrollRecordRepository,
        ICurrentUserService currentUserService)
    {
        _employeesRepository = employeesRepository;
        _salaryAdvanceRepository = salaryAdvanceRepository;
        _payrollRecordRepository = payrollRecordRepository;
        _currentUserService = currentUserService;
    }

    public async Task<Guid> Handle(CreateSalaryAdvanceRequestCommand request, CancellationToken cancellationToken)
    {
        var applicationUserId = _currentUserService.UserId;

        var employee =
            await _employeesRepository.GetCurrentEmployeeAsync(applicationUserId);

        if (employee == null)
            throw new Exception("Employee not found.");

        var hasPending =
            await _salaryAdvanceRepository.HasPendingRequestAsync(employee.Id);

        if (hasPending)
            throw new Exception("You already have a pending salary advance request.");

        var startDate = new DateTime(
            DateTime.Today.Year,
            DateTime.Today.Month,
            1);

        var endDate = DateTime.Today;

        var currentEarnings =
            await _payrollRecordRepository.GetTotalEarningsAsync(
                employee.Id,
                startDate,
                endDate);

        var approvedAdvances =
            await _salaryAdvanceRepository.GetTotalAdvancesAsync(
                employee.Id,
                startDate,
                endDate);

        var availableAdvance =
            currentEarnings - approvedAdvances;

        if (availableAdvance < 0)
            availableAdvance = 0;

        if (request.Amount > availableAdvance)
            throw new Exception(
                $"Maximum available salary advance is ₱{availableAdvance:N2}");

        var salaryAdvance = new SalaryAdvances
        {
            Id = Guid.NewGuid(),

            EmployeeId = employee.Id,

            ApplicationUserId = applicationUserId,

            Amount = request.Amount,

            Remarks = request.Remarks,

            RequestDate = DateTime.UtcNow,

            Status = SalaryAdvanceStatus.Pending
        };

        await _salaryAdvanceRepository.AddAsync(salaryAdvance);

        return salaryAdvance.Id;
    }
}