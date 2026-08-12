using PayTrack.Application.DTOs;
using PayTrack.Application.Interfaces.Repositories;
using PayTrack.Application.Interfaces.Services;

namespace PayTrack.Infrastructure.Services
{
    public class SalaryAdvanceEligibilityService
        : ISalaryAdvanceEligibilityService
    {
        private readonly ISalaryRepository _salaryRepository;

        private readonly IPayrollRecordRepository
            _payrollRecordRepository;

        private readonly ISalaryAdvanceRepository
            _salaryAdvanceRepository;

        private readonly ICompanyPayrollSettingRepository
            _companyPayrollSettingRepository;

        private readonly ISSSService _sssService;

        private readonly IPhilHealthService
            _philHealthService;

        private readonly IPagIbigService
            _pagIbigService;

        private readonly IEmployeesRepository
            _employeesRepository;


        public SalaryAdvanceEligibilityService(
            ISalaryRepository salaryRepository,
            IPayrollRecordRepository payrollRecordRepository,
            ISalaryAdvanceRepository salaryAdvanceRepository,
            ICompanyPayrollSettingRepository companyPayrollSettingRepository,
            IEmployeesRepository employeesRepository,
            ISSSService sssService,
            IPhilHealthService philHealthService,
            IPagIbigService pagIbigService)
        {
            _salaryRepository = salaryRepository;

            _payrollRecordRepository =
                payrollRecordRepository;

            _salaryAdvanceRepository =
                salaryAdvanceRepository;

            _companyPayrollSettingRepository =
                companyPayrollSettingRepository;

            _employeesRepository =
                employeesRepository;

            _sssService = sssService;

            _philHealthService =
                philHealthService;

            _pagIbigService =
                pagIbigService;
        }


        public async Task<SalaryAdvanceEligibilityDTO>
            GetEligibilityAsync(Guid employeeId)
        {
            // 1. GET EMPLOYEE

            var employee =
                await _employeesRepository
                    .GetByIdAsync(employeeId);

            if (employee == null)
            {
                throw new Exception(
                    "Employee not found.");
            }


            // 2. GET EMPLOYEE SALARY

            var employeeSalary =
                await _salaryRepository
                    .GetByEmployeeIdAsync(employeeId);

            if (employeeSalary == null)
            {
                throw new Exception(
                    "Employee salary has not been configured.");
            }

            var monthlyRate =
                employeeSalary.MonthlyRate;


            // 3. GET COMPANY ADMIN ID

            var companyAdminId =
                employee.CompanyAdminId;


            // 4. GET PAYROLL SETTINGS

            var payrollSetting =
                await _companyPayrollSettingRepository
                    .GetByCompanyAdminIdAsync(
                        companyAdminId);

            if (payrollSetting == null)
            {
                throw new Exception(
                    "Payroll settings have not been configured.");
            }

            var payrollFrequency =
                payrollSetting.Frequency;


            // 5. GET UNPAID EARNINGS

            var employeeApplicationUserId =
                employee.ApplicationUserId;

            var grossEarnings =
                await _payrollRecordRepository
                    .GetTotalUnpaidEarningsAsync(
                        employeeApplicationUserId);


            // 6. CALCULATE SSS

            var sss =
                await _sssService
                    .ComputeEmployeeContributionAsync(
                        monthlyRate,
                        payrollFrequency);


            // 7. CALCULATE PHILHEALTH

            var philHealth =
                await _philHealthService
                    .ComputeEmployeeContributionAsync(
                        monthlyRate,
                        payrollFrequency);


            // 8. CALCULATE PAG-IBIG

            var pagIbig =
                await _pagIbigService
                    .ComputeEmployeeContributionAsync(
                        monthlyRate,
                        payrollFrequency);


            // 9. TOTAL STATUTORY DEDUCTIONS

            var totalStatutoryDeductions =
                sss +
                philHealth +
                pagIbig;


            // 10. GET EXISTING UNDEDUCTED ADVANCES

            var existingAdvances =
                await _salaryAdvanceRepository
                    .GetTotalUndeductedAdvancesAsync(
                        employeeId);


            // 11. CALCULATE AVAILABLE ADVANCE

            var availableSalaryAdvance =
                grossEarnings
                - totalStatutoryDeductions
                - existingAdvances;


            // 12. PREVENT NEGATIVE VALUE

            if (availableSalaryAdvance < 0)
            {
                availableSalaryAdvance = 0;
            }

            // 13. DETERMINE ELIGIBILITY

            var canRequest =
                availableSalaryAdvance > 0;


            // 14. RETURN RESULT

            return new SalaryAdvanceEligibilityDTO(
                grossEarnings,
                sss,
                philHealth,
                pagIbig,
                existingAdvances,
                totalStatutoryDeductions,
                availableSalaryAdvance,
                canRequest
            );
        }
    }
}