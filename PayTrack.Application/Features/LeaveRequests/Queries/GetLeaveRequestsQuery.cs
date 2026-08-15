using MediatR;
using PayTrack.Application.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PayTrack.Application.Features.LeaveRequests.Queries
{
    public record GetLeaveRequestsQuery(string? Search, int PageNumber = 1, int? PageSize = 7) : IRequest<PagedResult<LeaveRequestDTO>>;
}
