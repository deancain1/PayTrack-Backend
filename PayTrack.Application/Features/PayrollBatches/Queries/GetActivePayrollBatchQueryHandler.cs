using MediatR;
using PayTrack.Application.DTOs;
using PayTrack.Application.Interfaces.Repositories;
using PayTrack.Application.Interfaces.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PayTrack.Application.Features.PayrollBatches.Queries
{
    public class GetActivePayrollBatchQueryHandler : IRequestHandler<GetActivePayrollBatchQuery, PayrollBatchDTO>
    {
        private readonly IPayrollBatchRepository _payrollBatchRepository;
        private readonly ICurrentUserService _currentUserService;

        public GetActivePayrollBatchQueryHandler(IPayrollBatchRepository payrollBatchRepository, ICurrentUserService currentUserService)
        {
            _payrollBatchRepository = payrollBatchRepository;
            _currentUserService = currentUserService;
        }

       
        public async Task<PayrollBatchDTO> Handle(GetActivePayrollBatchQuery request,CancellationToken cancellationToken)
        {
            var companyAdminId = _currentUserService.UserId;

            var batch = await _payrollBatchRepository.GetActiveBatchAsync(companyAdminId);

            if (batch == null)
                return null;

            return new PayrollBatchDTO(
                batch.Id,
                batch.PeriodStart,
                batch.PeriodEnd,
                batch.Status,
                batch.Payrolls.Count,
                batch.Payrolls.Sum(x => x.NetAmount)
            );
        }
    }
}
