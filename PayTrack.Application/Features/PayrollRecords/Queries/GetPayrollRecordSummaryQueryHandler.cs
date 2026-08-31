using MediatR;
using PayTrack.Application.DTOs;
using PayTrack.Application.Features.PayrollRecords.Queries;
using PayTrack.Application.Interfaces.Repositories;

namespace PayTrack.Application.Features.Payroll.Queries;

public class GetPayrollRecordSummaryQueryHandler
    : IRequestHandler<GetPayrollRecordSummaryQuery, PayrollRecordSummaryDTO>
{
    private readonly IPayrollRecordRepository _payrollRecordRepository;
    private readonly IAttendanceRepository _attendanceRepository;

    public GetPayrollRecordSummaryQueryHandler(
        IPayrollRecordRepository payrollRecordRepository,
        IAttendanceRepository attendanceRepository)
    {
        _payrollRecordRepository = payrollRecordRepository;
        _attendanceRepository = attendanceRepository;
    }

    public async Task<PayrollRecordSummaryDTO> Handle(GetPayrollRecordSummaryQuery request, CancellationToken cancellationToken)
    {
        // Get payroll records for selected month/year
        var payrollRecords =
            await _payrollRecordRepository.GetPayrollRecordsByMonthAsync(
                request.Month,
                request.Year);

        var attendances =
            await _attendanceRepository.GetAttendanceByMonthAsync(
                request.Month,
                request.Year);

        decimal totalPayroll = payrollRecords.Sum(x =>
            x.AmountEarned
        );

        decimal totalHours = attendances.Sum(x =>
            x.HoursWorked
        );

        return new PayrollRecordSummaryDTO(
            Math.Round(totalPayroll, 2),
            Math.Round(totalHours, 2)
        );
    }
}