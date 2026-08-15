using MediatR;
using PayTrack.Application.DTOs;
using PayTrack.Application.Interfaces.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PayTrack.Application.Features.LeaveRequests.Queries
{
    public class GetLeaveRequestByIdQueryHandler : IRequestHandler<GetLeaveRequestByIdQuery, LeaveRequestDTO>
    {
        private  readonly ILeaveRequestRepository _leaveRequestRepository;

        public GetLeaveRequestByIdQueryHandler(ILeaveRequestRepository leaveRequestRepository)
        {
            _leaveRequestRepository = leaveRequestRepository;
        }
        public async Task<LeaveRequestDTO> Handle(GetLeaveRequestByIdQuery request, CancellationToken cancellationToken)
        {
           var leaveRequest = await _leaveRequestRepository.GetLeavesWithEmployeeByIdAsync(request.Id);
            if (leaveRequest == null)
                throw new KeyNotFoundException($"Leave request with ID {request.Id} not found.");

            return new LeaveRequestDTO(
     leaveRequest.Id,
     leaveRequest.EmployeeId,

     leaveRequest.Employee.ApplicationUser.FullName,

     leaveRequest.Employee.EmployeeNumber,

     leaveRequest.Employee.Department != null
         ? leaveRequest.Employee.Department.DepartmentName
         : null,

     leaveRequest.LeaveType.ToString(),
     leaveRequest.StartDate,
     leaveRequest.EndDate,
     leaveRequest.TotalDays,
     leaveRequest.Reason,
     leaveRequest.AttachmentUrl,
     leaveRequest.Status.ToString(),
     leaveRequest.AdminRemarks,
     leaveRequest.ReviewedBy,
     leaveRequest.ReviewedAt,
     leaveRequest.CreatedAt
 );
        }
    }
}
