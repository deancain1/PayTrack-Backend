using MediatR;
using PayTrack.Application.DTOs;
using PayTrack.Application.Interfaces.Repositories;
using PayTrack.Application.Interfaces.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace PayTrack.Application.Features.Payroll.Queries
{
    public class GetMyPayrollsQueryHandler
        : IRequestHandler<GetMyPayrollsQuery, List<EmployeePayslipDTO>>
    {
        private readonly IPayrollRepository _payrollRepository;
        private readonly IEmployeesRepository _employeesRepository;
        private readonly ICurrentUserService _currentUserService;

        public GetMyPayrollsQueryHandler(
            IPayrollRepository payrollRepository,
            IEmployeesRepository employeesRepository,
            ICurrentUserService currentUserService)
        {
            _payrollRepository = payrollRepository;
            _employeesRepository = employeesRepository;
            _currentUserService = currentUserService;
        }

        public async Task<List<EmployeePayslipDTO>> Handle(GetMyPayrollsQuery request, CancellationToken cancellationToken)
        {

            var userId = _currentUserService.UserId;

            if (string.IsNullOrWhiteSpace(userId))
            {
                throw new UnauthorizedAccessException(
                    "User is not authenticated.");
            }



            var employee =
                await _employeesRepository
                    .GetCurrentEmployeeByApplicationUserIdAsync(userId);

            if (employee == null)
            {
                throw new KeyNotFoundException(
                    "Employee record not found.");
            }



            var payrolls =
                await _payrollRepository
                    .GetEmployeePayrollsAsync(
                        employee.Id);

            return payrolls
                .Select(payroll => new EmployeePayslipDTO(
                    payroll.Id,

                    payroll.EmployeeId,

                    $"{payroll.Employee.ApplicationUser.FullName}" ,
                    payroll.PeriodStart,
                    payroll.PeriodEnd,

                    payroll.TotalAmount -
                    payroll.OvertimeAmount,

                    payroll.OvertimeAmount,

                    payroll.TotalAmount,

                    payroll.SalaryAdvanceDeduction,
                    payroll.PhilHealthDeduction,
                    payroll.PagIbigDeduction,
                    payroll.SSSDeduction,

                    payroll.NetAmount,

                    payroll.Status,

                    payroll.ReleasedAt,

                    payroll.PaidAt
                ))
                .ToList();
        }
    }
}