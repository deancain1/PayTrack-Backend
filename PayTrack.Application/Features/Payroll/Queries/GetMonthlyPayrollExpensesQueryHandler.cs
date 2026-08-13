using MediatR;
using PayTrack.Application.DTOs;
using PayTrack.Application.Features.Payroll.Queries;
using PayTrack.Application.Interfaces.Repositories;
using PayTrack.Application.Interfaces.Services;
using System.Globalization;

public class GetMonthlyPayrollExpensesQueryHandler
    : IRequestHandler<GetMonthlyPayrollExpensesQuery, List<MonthlyPayrollExpenseDTO>>
{
    private readonly IPayrollRepository _payrollRepository;
    private readonly ICurrentUserService _currentUserService;

    public GetMonthlyPayrollExpensesQueryHandler(
        IPayrollRepository payrollRepository,
        ICurrentUserService currentUserService)
    {
        _payrollRepository = payrollRepository;
        _currentUserService = currentUserService;
    }

    public async Task<List<MonthlyPayrollExpenseDTO>> Handle(GetMonthlyPayrollExpensesQuery request, CancellationToken cancellationToken)
    {
        var payrolls = await _payrollRepository.GetPayrollsByCompanyAndYearAsync(
            _currentUserService.UserId,
            DateTime.Now.Year);

        var groupedPayrolls = payrolls
            .GroupBy(x => x.PeriodEnd.Month)
            .ToDictionary(g => g.Key);

        var result = new List<MonthlyPayrollExpenseDTO>();

        for (int month = 1; month <= 12; month++)
        {
            if (groupedPayrolls.TryGetValue(month, out var payrollGroup))
            {
                result.Add(new MonthlyPayrollExpenseDTO(
                    Month: CultureInfo.CurrentCulture.DateTimeFormat.GetAbbreviatedMonthName(month),
                    GrossPayroll: payrollGroup.Sum(x => x.TotalAmount),
                    SalaryAdvanceDeductions: payrollGroup.Sum(x => x.SalaryAdvanceDeduction),
                    NetPayrollPaid: payrollGroup.Sum(x => x.NetAmount)
                ));
            }
            else
            {
                result.Add(new MonthlyPayrollExpenseDTO(
                    Month: CultureInfo.CurrentCulture.DateTimeFormat.GetAbbreviatedMonthName(month),
                    GrossPayroll: 0,
                    SalaryAdvanceDeductions: 0,
                    NetPayrollPaid: 0
                ));
            }
        }

        return result;
    }
}