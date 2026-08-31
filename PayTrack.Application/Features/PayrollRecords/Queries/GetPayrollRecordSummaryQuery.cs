using MediatR;
using PayTrack.Application.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PayTrack.Application.Features.PayrollRecords.Queries
{
    public record GetPayrollRecordSummaryQuery(int Month, int Year) : IRequest<PayrollRecordSummaryDTO>;
    
}
