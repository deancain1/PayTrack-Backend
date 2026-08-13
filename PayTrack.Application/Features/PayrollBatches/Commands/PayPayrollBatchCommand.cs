using MediatR;
using PayTrack.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PayTrack.Application.Features.PayrollBatches.Commands
{
    public record PayPayrollBatchCommand(
     Guid BatchId,
     PaymentMethod PaymentMethod,
     string? ReferenceNumber
 ) : IRequest<bool>;
}
