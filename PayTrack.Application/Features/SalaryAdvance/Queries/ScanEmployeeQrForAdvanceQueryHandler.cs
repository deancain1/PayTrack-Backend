using MediatR;
using PayTrack.Application.DTOs;
using PayTrack.Application.Interfaces.Repositories;
using PayTrack.Application.Interfaces.Services;

namespace PayTrack.Application.Features.SalaryAdvance.Queries
{
    public class ScanEmployeeQrForAdvanceQueryHandler
        : IRequestHandler<
            ScanEmployeeQrForAdvanceQuery,
            ScanEmployeeForAdvanceDTO>
    {
        private readonly IEmployeesRepository _employeesRepository;
        private readonly ISalaryAdvanceEligibilityService
            _salaryAdvanceEligibilityService;

        public ScanEmployeeQrForAdvanceQueryHandler(
            IEmployeesRepository employeesRepository,
            ISalaryAdvanceEligibilityService salaryAdvanceEligibilityService)
        {
            _employeesRepository = employeesRepository;
            _salaryAdvanceEligibilityService =
                salaryAdvanceEligibilityService;
        }

        public async Task<ScanEmployeeForAdvanceDTO> Handle(
            ScanEmployeeQrForAdvanceQuery request,
            CancellationToken cancellationToken)
        {
            var employee =
                await _employeesRepository
                    .GetByIdWithInformationAsync(request.EmployeeId);

            if (employee is null)
            {
                throw new Exception("Employee not found.");
            }

            var eligibility =
                await _salaryAdvanceEligibilityService
                    .GetEligibilityAsync(employee.Id);

            return new ScanEmployeeForAdvanceDTO(
                employee.Id,
                employee.ApplicationUserId,
                employee.EmployeeNumber,
                employee.ApplicationUser.FullName,
                employee.Department?.DepartmentName ?? string.Empty,
                eligibility.GrossEarnings,
                eligibility.ExistingSalaryAdvances,
                eligibility.AvailableSalaryAdvance
            );
        }
    }
}