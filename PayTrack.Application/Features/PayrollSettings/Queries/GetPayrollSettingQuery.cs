using MediatR;
using PayTrack.Application.DTOs;
using PayTrack.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PayTrack.Application.Features.PayrollSettings.Queries
{
    public record GetPayrollSettingQuery()
      : IRequest<PayrollSettingDTO?>;
}
