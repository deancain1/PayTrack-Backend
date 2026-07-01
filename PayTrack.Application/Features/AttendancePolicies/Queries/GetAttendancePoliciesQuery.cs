using MediatR;
using PayTrack.Application.DTOs;
using PayTrack.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PayTrack.Application.Features.AttendancePolicies.Queries
{
    public record GetAttendancePoliciesQuery(string CompanyAdminId): IRequest<List<AttendancePolicyDTO>>;
}
