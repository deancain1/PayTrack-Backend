using MediatR;
using PayTrack.Application.Interfaces.Repositories;
using PayTrack.Application.Interfaces.Services;
using PayTrack.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PayTrack.Application.Features.PayrollBatches.Commands
{
    public class PayPayrollBatchCommandHandler : IRequestHandler<PayPayrollBatchCommand, bool>
    {
        private readonly IPayrollBatchRepository _payrollBatchRepository;
        private readonly ICurrentUserService _currentUserService;
        private readonly IPayrollRecordRepository _payrollRecordRepository;

        public PayPayrollBatchCommandHandler(IPayrollBatchRepository payrollBatchRepository, ICurrentUserService currentUserService, IPayrollRecordRepository payrollRecordRepository)
        {
            _payrollBatchRepository = payrollBatchRepository;
            _currentUserService = currentUserService;
            _payrollRecordRepository = payrollRecordRepository;
        }
        public async Task<bool> Handle(PayPayrollBatchCommand request, CancellationToken cancellationToken)
        {
          var batch = await _payrollBatchRepository.GetByIdWithPayrollsAsync(request.BatchId);

            if(batch == null)
                throw new Exception("Payroll batch not found");

            if(batch.Status == Domain.Enums.PayrollBatchStatus.Paid)
                throw new Exception("Payroll batch already paid.");

            batch.Status = PayrollBatchStatus.Paid;

            batch.PaidAt = DateTime.UtcNow;

            batch.PaymentMethod = request.PaymentMethod;

            batch.ReferenceNumber =
                request.PaymentMethod == PaymentMethod.Cash
                    ? null
                    : request.ReferenceNumber;

            batch.PaidBy = _currentUserService.UserId;

            foreach (var payroll in batch.Payrolls)
            {
                payroll.Status = PayrollStatus.Paid;
                payroll.PaidAt = DateTime.UtcNow;

                //Updates the payroll records to mark them as paid
                await _payrollRecordRepository.MarkRecordsAsPaidAsync(
                   payroll.EmployeeId,
                   payroll.PeriodStart,
                   payroll.PeriodEnd);
            }

             _payrollBatchRepository.Update(batch);
             await _payrollBatchRepository.SaveChangesAsync();
            return true;
        }
    }
}
