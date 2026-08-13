using MediatR;
using PayTrack.Application.DTOs;
using PayTrack.Application.Interfaces.Repositories;
using PayTrack.Application.Interfaces.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PayTrack.Application.Features.PayrollRecords.Queries
{
    internal class GetPayrollRecordSummaryQueryHandler : IRequestHandler<GetPayrollRecordSummaryQuery, PayrollRecordSummaryDTO>
    {
        private readonly IPayrollRecordRepository _payrollRepository;
        private readonly IAttendanceRepository _attendanceRepository;
        private readonly ICurrentUserService _currentUser;

        public GetPayrollRecordSummaryQueryHandler(
            IPayrollRecordRepository payrollRepository,
            IAttendanceRepository attendanceRepository,
            ICurrentUserService currentUser)
        {
            _payrollRepository = payrollRepository;
            _attendanceRepository = attendanceRepository;
            _currentUser = currentUser;
        }

        public async Task<PayrollRecordSummaryDTO> Handle(
            GetPayrollRecordSummaryQuery request,
            CancellationToken cancellationToken)
        {
            var companyAdminId = _currentUser.UserId!;

            var totalPayroll=
              await  _payrollRepository.GetTotalPayrollTodayAsync(
                    companyAdminId);

            var averageDailyPayout =
                await _payrollRepository.GetAverageDailyPayoutAsync(
                    companyAdminId);

            var activeTracked =
                await _attendanceRepository.GetActiveTrackedAsync(
                    companyAdminId);

          
            return new PayrollRecordSummaryDTO(
             totalPayroll,
             averageDailyPayout,
             activeTracked
            );
        }
    }
}
