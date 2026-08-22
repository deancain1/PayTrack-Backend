using MediatR;
using PayTrack.Application.Interfaces.Repositories;
using PayTrack.Application.Interfaces.Services;
using PayTrack.Domain.Enums;

namespace PayTrack.Application.Features.PayrollBatches.Commands
{
    public class ReleasePayrollBatchCommandHandler
        : IRequestHandler<ReleasePayrollBatchCommand, bool>
    {
        private readonly IPayrollBatchRepository _payrollBatchRepository;
        private readonly ICurrentUserService _currentUserService;

        public ReleasePayrollBatchCommandHandler(
            IPayrollBatchRepository payrollBatchRepository,
            ICurrentUserService currentUserService)
        {
            _payrollBatchRepository = payrollBatchRepository;
            _currentUserService = currentUserService;
        }

        public async Task<bool> Handle(ReleasePayrollBatchCommand request, CancellationToken cancellationToken)
        {
            var batch =
                await _payrollBatchRepository
                    .GetByIdWithPayrollsAsync(request.Id);

            if (batch == null)
            {
                throw new InvalidOperationException(
                    "Payroll batch not found.");
            }

            if (batch.Status != PayrollBatchStatus.Pending)
            {
                throw new InvalidOperationException(
                    $"Payroll batch cannot be released because its current status is {batch.Status}.");
            }


            if (batch.Payrolls == null ||
                !batch.Payrolls.Any())
            {
                throw new InvalidOperationException(
                    "Payroll batch has no payrolls to release.");
            }

            var releasedAt = DateTime.UtcNow;

            batch.Status = PayrollBatchStatus.Released;

            batch.ReleasedAt = releasedAt;

            batch.ReleasedBy = _currentUserService.UserId; 


            foreach (var payroll in batch.Payrolls)
            {
                payroll.Status = PayrollStatus.Released;

                payroll.ReleasedAt = releasedAt;
            }

            _payrollBatchRepository.Update(batch);

            await _payrollBatchRepository.SaveChangesAsync();

            return true;
        }
    }
}